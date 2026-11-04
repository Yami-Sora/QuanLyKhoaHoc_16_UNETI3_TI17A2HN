// Họ và tên: Nguyễn Văn Mạnh
// Mã sinh viên: 23103100096
// Nội dung thực hiện: Module 2 - Triển khai Repository pattern cho quản lý khóa học

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories
{
    public class KhoaHocRepository : IKhoaHocRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<KhoaHocRepository> _logger;

        public KhoaHocRepository(ApplicationDbContext context, ILogger<KhoaHocRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // =========================================================================
        // 1. Tự động đồng bộ vòng đời khóa học
        // =========================================================================
        public async Task DongBoVongDoiKhoaHocAsync()
        {
            try
            {
                var today = DateTime.Today;

                // Tự động chuyển trạng thái sang đang giảng dạy khi đã đến hoặc qua ngày khai giảng
                var khoaHocsCanBatDau = await _context.KhoaHocs
                    .Where(k => (k.TrangThai == "DangMo" || k.TrangThai == "SapMo") &&
                                k.NgayBatDau.Date <= today &&
                                k.NgayKetThuc.Date >= today)
                    .ToListAsync();

                // Tự động chuyển trạng thái sang đã kết thúc khi đã quá ngày kết thúc
                var khoaHocsCanKetThuc = await _context.KhoaHocs
                    .Where(k => k.TrangThai != "DaKetThuc" &&
                                k.TrangThai != "BiHuy" &&
                                k.NgayKetThuc.Date < today)
                    .ToListAsync();

                bool coThayDoi = false;
                if (khoaHocsCanBatDau.Count > 0)
                {
                    foreach (var k in khoaHocsCanBatDau)
                    {
                        k.TrangThai = "DangHoc";
                    }
                    coThayDoi = true;
                }

                if (khoaHocsCanKetThuc.Count > 0)
                {
                    foreach (var k in khoaHocsCanKetThuc)
                    {
                        k.TrangThai = "DaKetThuc";
                    }
                    coThayDoi = true;
                }

                // Lưu các thay đổi trạng thái vào cơ sở dữ liệu
                if (coThayDoi)
                {
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Ghi nhận lỗi nếu phát sinh sự cố trong quá trình đồng bộ
                _logger.LogError(ex, "Lỗi khi tự động đồng bộ vòng đời khóa học.");
            }
        }

        // =========================================================================
        // 2. Cổng tra cứu khóa học công khai
        // =========================================================================
        public async Task<(KhoaHocFilterViewModel Filter, SelectList MonHocList)> GetFilterViewModelAsync(
            KhoaHocFilterViewModel filter, int? page = null)
        {
            // Tự động đồng bộ trạng thái khóa học theo ngày thực tế
            await DongBoVongDoiKhoaHocAsync();

            // Thiết lập số trang hiện tại
            if (page.HasValue && page.Value > 0)
            {
                filter.PageIndex = page.Value;
            }

            // Lấy danh sách môn học cho dropdown bộ lọc
            var monHocs = await _context.MonHocs
                .Where(m => m.TrangThai)
                .OrderBy(m => m.TenMonHoc)
                .Select(m => new { m.MaMonHoc, Display = m.TenMonHoc + " (" + m.SoTinChi + " TC)" })
                .ToListAsync();

            var monHocList = new SelectList(monHocs, "MaMonHoc", "Display", filter.MaMonHoc);

            // Thống kê tổng quan cho các thẻ thống kê
            var allCoursesQuery = _context.KhoaHocs
                .Include(k => k.DangKyKhoaHocs)
                .AsNoTracking();

            filter.TongSoKhoaHoc = await allCoursesQuery.CountAsync();
            filter.SoKhoaDangMo = await allCoursesQuery.CountAsync(k => k.TrangThai == "DangMo");
            filter.SoKhoaConCho = await allCoursesQuery.CountAsync(k =>
                k.SoLuongToiDa > k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"));
            filter.SoKhoaKhaDung = await allCoursesQuery.CountAsync(k =>
                k.TrangThai == "DangMo" &&
                k.NgayBatDau >= DateTime.Today &&
                k.SoLuongToiDa > k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"));
            filter.HocPhiThapNhat = await allCoursesQuery.AnyAsync()
                ? await allCoursesQuery.MinAsync(k => k.HocPhi)
                : 0m;
            filter.HocPhiCaoNhat = await allCoursesQuery.AnyAsync()
                ? await allCoursesQuery.MaxAsync(k => k.HocPhi)
                : 0m;

            // Khởi tạo truy vấn danh sách khóa học kèm dữ liệu liên kết
            var query = _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.GiangVien)
                .Include(k => k.DangKyKhoaHocs)
                .AsNoTracking()
                .AsQueryable();

            // 6.3. Tìm kiếm theo tên khóa học, môn học, giảng viên
            if (!string.IsNullOrWhiteSpace(filter.SearchString))
            {
                var keyword = filter.SearchString.Trim().ToLower();
                query = query.Where(k =>
                    k.TenKhoaHoc.ToLower().Contains(keyword) ||
                    (k.MonHoc != null && k.MonHoc.TenMonHoc.ToLower().Contains(keyword)) ||
                    (k.GiangVien != null && k.GiangVien.HoTen.ToLower().Contains(keyword)));
            }

            // 6.4. Lọc kết hợp theo môn học
            if (filter.MaMonHoc.HasValue && filter.MaMonHoc.Value > 0)
            {
                query = query.Where(k => k.MaMonHoc == filter.MaMonHoc.Value);
            }

            // 6.4. Lọc kết hợp theo hình thức đào tạo
            if (!string.IsNullOrWhiteSpace(filter.HinhThuc))
            {
                query = query.Where(k => k.HinhThuc == filter.HinhThuc);
            }

            // 6.4. Lọc kết hợp theo trạng thái khóa học
            if (!string.IsNullOrWhiteSpace(filter.TrangThai))
            {
                query = query.Where(k => k.TrangThai == filter.TrangThai);
            }

            // 6.4. Lọc kết hợp theo khoảng học phí
            if (!string.IsNullOrWhiteSpace(filter.KhoangHocPhi))
            {
                query = filter.KhoangHocPhi switch
                {
                    "duoi_2tr" => query.Where(k => k.HocPhi < 2000000),
                    "2tr_4tr" => query.Where(k => k.HocPhi >= 2000000 && k.HocPhi <= 4000000),
                    "4tr_6tr" => query.Where(k => k.HocPhi > 4000000 && k.HocPhi <= 6000000),
                    "tren_6tr" => query.Where(k => k.HocPhi > 6000000),
                    _ => query
                };
            }

            // 6.4. Lọc kết hợp theo tình trạng chỗ (còn chỗ / hết chỗ)
            if (!string.IsNullOrWhiteSpace(filter.TinhTrangCho))
            {
                if (filter.TinhTrangCho == "con_cho")
                {
                    query = query.Where(k => k.SoLuongToiDa > k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"));
                }
                else if (filter.TinhTrangCho == "het_cho")
                {
                    query = query.Where(k => k.SoLuongToiDa <= k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"));
                }
            }

            // 6.5. Sắp xếp danh sách khóa học theo tiêu chí lựa chọn
            query = filter.SortBy switch
            {
                "ten_asc" => query.OrderBy(k => k.TenKhoaHoc),
                "ten_desc" => query.OrderByDescending(k => k.TenKhoaHoc),
                "ngay_asc" => query.OrderBy(k => k.NgayBatDau).ThenBy(k => k.TenKhoaHoc),
                "ngay_desc" => query.OrderByDescending(k => k.NgayBatDau).ThenBy(k => k.TenKhoaHoc),
                "hocphi_asc" => query.OrderBy(k => k.HocPhi).ThenBy(k => k.TenKhoaHoc),
                "hocphi_desc" => query.OrderByDescending(k => k.HocPhi).ThenBy(k => k.TenKhoaHoc),
                "cho_asc" => query.OrderBy(k => (k.SoLuongToiDa - k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"))).ThenBy(k => k.TenKhoaHoc),
                "cho_desc" => query.OrderByDescending(k => (k.SoLuongToiDa - k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"))).ThenBy(k => k.TenKhoaHoc),
                _ => query.OrderByDescending(k => k.MaKhoaHoc)
            };

            // 6.6. Phân trang dữ liệu bằng Skip() và Take() trên cơ sở dữ liệu
            filter.TotalItems = await query.CountAsync();

            if (filter.PageIndex < 1) filter.PageIndex = 1;
            if (filter.PageSize < 1) filter.PageSize = 6;

            var pagedEntities = await query
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            // 6.7. Hiển thị cho người dùng và kiểm tra tính khả dụng giao dịch
            filter.DanhSachKhoaHoc = pagedEntities.Select(k => new KhoaHocItemViewModel
            {
                MaKhoaHoc = k.MaKhoaHoc,
                TenKhoaHoc = k.TenKhoaHoc,
                MaMonHoc = k.MaMonHoc,
                TenMonHoc = k.MonHoc?.TenMonHoc ?? "Chưa xác định",
                SoTinChi = k.MonHoc?.SoTinChi ?? 0,
                MaGiangVien = k.MaGiangVien,
                TenGiangVien = k.GiangVien?.HoTen ?? "Chưa phân công",
                HocVi = k.GiangVien?.HocVi,
                ChuyenMon = k.GiangVien?.ChuyenMon,
                NgayBatDau = k.NgayBatDau,
                NgayKetThuc = k.NgayKetThuc,
                SoLuongToiDa = k.SoLuongToiDa,
                SoLuongDaDangKy = k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"),
                HocPhi = k.HocPhi,
                HinhThuc = k.HinhThuc,
                TrangThai = k.TrangThai,
                MoTa = k.MoTa
            }).ToList();

            return (filter, monHocList);
        }

        // =========================================================================
        // 3. Quản trị khóa học
        // =========================================================================
        public async Task<KhoaHocQuanLyResult> GetQuanLyDataAsync(
            string? searchString,
            int? maMonHoc,
            string? trangThai,
            string? hinhThuc,
            string? khoangHocPhi,
            string? tinhTrangCho,
            string? sortBy,
            int page,
            int pageSize)
        {
            // Tự động đồng bộ trạng thái khóa học theo ngày thực tế
            await DongBoVongDoiKhoaHocAsync();

            var result = new KhoaHocQuanLyResult();

            // Lấy danh sách môn học cho dropdown bộ lọc quản trị
            var monHocs = await _context.MonHocs
                .Where(m => m.TrangThai)
                .OrderBy(m => m.TenMonHoc)
                .Select(m => new { m.MaMonHoc, Display = m.TenMonHoc + " (" + m.SoTinChi + " TC)" })
                .ToListAsync();

            result.MonHocList = new SelectList(monHocs, "MaMonHoc", "Display", maMonHoc);

            // Thống kê số liệu cho 4 thẻ thống kê quản trị
            result.TotalCount = await _context.KhoaHocs.CountAsync();
            result.DangMoCount = await _context.KhoaHocs.CountAsync(k => k.TrangThai == "DangMo");
            result.DangHocCount = await _context.KhoaHocs.CountAsync(k => k.TrangThai == "DangHoc");
            result.DaKetThucCount = await _context.KhoaHocs.CountAsync(k => k.TrangThai == "DaKetThuc");
            result.AvgHocPhi = await _context.KhoaHocs.AnyAsync()
                ? (long)await _context.KhoaHocs.AverageAsync(k => (double)k.HocPhi)
                : 0;

            // Khởi tạo truy vấn quản trị kèm môn học, giảng viên và danh sách đăng ký
            var query = _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.GiangVien)
                .Include(k => k.DangKyKhoaHocs)
                .AsNoTracking()
                .AsQueryable();

            // 6.3. Tìm kiếm theo tên khóa học, môn học, giảng viên
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var keyword = searchString.Trim().ToLower();
                query = query.Where(k =>
                    k.TenKhoaHoc.ToLower().Contains(keyword) ||
                    (k.MonHoc != null && k.MonHoc.TenMonHoc.ToLower().Contains(keyword)) ||
                    (k.GiangVien != null && k.GiangVien.HoTen.ToLower().Contains(keyword)));
            }

            // 6.4. Lọc theo môn học
            if (maMonHoc.HasValue && maMonHoc.Value > 0)
            {
                query = query.Where(k => k.MaMonHoc == maMonHoc.Value);
            }

            // 6.4. Lọc theo trạng thái
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(k => k.TrangThai == trangThai);
            }

            // 6.4. Lọc theo hình thức đào tạo
            if (!string.IsNullOrWhiteSpace(hinhThuc))
            {
                query = query.Where(k => k.HinhThuc == hinhThuc);
            }

            // 6.4. Lọc theo khoảng học phí
            if (!string.IsNullOrWhiteSpace(khoangHocPhi))
            {
                query = khoangHocPhi switch
                {
                    "duoi_2tr" => query.Where(k => k.HocPhi < 2000000),
                    "2tr_4tr" => query.Where(k => k.HocPhi >= 2000000 && k.HocPhi <= 4000000),
                    "4tr_6tr" => query.Where(k => k.HocPhi > 4000000 && k.HocPhi <= 6000000),
                    "tren_6tr" => query.Where(k => k.HocPhi > 6000000),
                    _ => query
                };
            }

            // 6.4. Lọc theo tình trạng chỗ (còn chỗ / hết chỗ)
            if (!string.IsNullOrWhiteSpace(tinhTrangCho))
            {
                if (tinhTrangCho == "con_cho")
                {
                    query = query.Where(k => k.SoLuongToiDa > k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"));
                }
                else if (tinhTrangCho == "het_cho")
                {
                    query = query.Where(k => k.SoLuongToiDa <= k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"));
                }
            }

            // 6.5. Sắp xếp danh sách khóa học
            query = sortBy switch
            {
                "ten_asc" => query.OrderBy(k => k.TenKhoaHoc),
                "ten_desc" => query.OrderByDescending(k => k.TenKhoaHoc),
                "ngay_asc" => query.OrderBy(k => k.NgayBatDau).ThenBy(k => k.TenKhoaHoc),
                "ngay_desc" => query.OrderByDescending(k => k.NgayBatDau).ThenBy(k => k.TenKhoaHoc),
                "hocphi_asc" => query.OrderBy(k => k.HocPhi).ThenBy(k => k.TenKhoaHoc),
                "hocphi_desc" => query.OrderByDescending(k => k.HocPhi).ThenBy(k => k.TenKhoaHoc),
                "cho_asc" => query.OrderBy(k => (k.SoLuongToiDa - k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"))).ThenBy(k => k.TenKhoaHoc),
                "cho_desc" => query.OrderByDescending(k => (k.SoLuongToiDa - k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"))).ThenBy(k => k.TenKhoaHoc),
                "id_asc" => query.OrderBy(k => k.MaKhoaHoc),
                _ => query.OrderByDescending(k => k.MaKhoaHoc)
            };

            // 6.6. Phân trang dữ liệu với Skip() và Take()
            int totalFiltered = await query.CountAsync();
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            int totalPages = (int)Math.Ceiling(totalFiltered / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages) page = totalPages;

            result.PageIndex = page;
            result.PageSize = pageSize;
            result.TotalItems = totalFiltered;
            result.TotalPages = totalPages;

            result.Items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return result;
        }

        // =========================================================================
        // 4. Chi tiết khóa học
        // =========================================================================
        public async Task<KhoaHoc?> GetByIdWithDetailsAsync(int id)
        {
            // Tự động đồng bộ trạng thái khóa học theo ngày thực tế
            await DongBoVongDoiKhoaHocAsync();

            // Lấy chi tiết thông tin khóa học kèm môn học, giảng viên và danh sách đăng ký
            return await _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.GiangVien)
                .Include(k => k.DangKyKhoaHocs)
                    .ThenInclude(dk => dk.HocVien)
                .FirstOrDefaultAsync(k => k.MaKhoaHoc == id);
        }

        public async Task<bool> CheckHocVienDaDangKyAsync(int maKhoaHoc, int maTaiKhoan)
        {
            // Tìm thông tin học viên theo mã tài khoản đăng nhập
            var hocVien = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);
            if (hocVien == null) return false;

            // Kiểm tra học viên đã đăng ký khóa học này hay chưa
            return await _context.DangKyKhoaHocs
                .AnyAsync(dk => dk.MaKhoaHoc == maKhoaHoc && dk.MaHocVien == hocVien.MaHocVien && dk.TrangThai != "BiHuy");
        }

        public async Task<KhoaHoc?> GetByIdAsync(int id)
        {
            // Lấy thông tin khóa học theo mã để phục vụ chỉnh sửa
            return await _context.KhoaHocs.FindAsync(id);
        }

        // =========================================================================
        // 5. Lấy dữ liệu dropdown cho khóa ngoại
        // =========================================================================
        public async Task<(SelectList MonHocList, SelectList GiangVienList)> GetDropdownDataAsync(
            int? selectedMonHoc = null, int? selectedGiangVien = null)
        {
            // Lấy danh sách môn học đang mở từ cơ sở dữ liệu
            var monHocs = await _context.MonHocs
                .Where(m => m.TrangThai)
                .OrderBy(m => m.TenMonHoc)
                .Select(m => new { m.MaMonHoc, Display = m.TenMonHoc + " (" + m.SoTinChi + " TC)" })
                .ToListAsync();

            // Lấy danh sách giảng viên đang công tác từ cơ sở dữ liệu
            var giangViens = await _context.GiangViens
                .Where(g => g.TrangThai)
                .OrderBy(g => g.HoTen)
                .Select(g => new { g.MaGiangVien, Display = g.HoTen + " - " + (g.ChuyenMon ?? "N/A") })
                .ToListAsync();

            // Khởi tạo SelectList cho giao diện lựa chọn
            return (
                new SelectList(monHocs, "MaMonHoc", "Display", selectedMonHoc),
                new SelectList(giangViens, "MaGiangVien", "Display", selectedGiangVien)
            );
        }

        // =========================================================================
        // 6. Thêm mới khóa học
        // =========================================================================
        public async Task<(bool Success, string? ErrorMessage, int MaKhoaHoc)> CreateKhoaHocAsync(KhoaHoc khoaHoc)
        {
            // 6.2. Kiểm tra ràng buộc ngày kết thúc phải sau ngày bắt đầu
            if (khoaHoc.NgayKetThuc <= khoaHoc.NgayBatDau)
            {
                return (false, "Ngày kết thúc phải sau ngày bắt đầu khóa học.", 0);
            }

            // 6.2. Kiểm tra sĩ số tối đa phải lớn hơn 0
            if (khoaHoc.SoLuongToiDa <= 0)
            {
                return (false, "Sĩ số học viên tối đa phải lớn hơn 0.", 0);
            }

            // 6.2. Kiểm tra học phí không được âm
            if (khoaHoc.HocPhi < 0)
            {
                return (false, "Học phí khóa học không được âm.", 0);
            }

            try
            {
                // Thêm mới khóa học vào cơ sở dữ liệu và lưu thay đổi
                _context.KhoaHocs.Add(khoaHoc);
                await _context.SaveChangesAsync();
                return (true, null, khoaHoc.MaKhoaHoc);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm mới khóa học.");
                return (false, "Không thể lưu khóa học do lỗi kết nối cơ sở dữ liệu.", 0);
            }
        }

        // =========================================================================
        // 7. Cập nhật khóa học
        // =========================================================================
        public async Task<(bool Success, string? ErrorMessage)> UpdateKhoaHocAsync(KhoaHoc khoaHoc)
        {
            // 6.2. Kiểm tra ràng buộc ngày kết thúc phải sau ngày bắt đầu
            if (khoaHoc.NgayKetThuc <= khoaHoc.NgayBatDau)
            {
                return (false, "Ngày kết thúc phải sau ngày bắt đầu khóa học.");
            }

            // 6.2. Kiểm tra sĩ số tối đa phải lớn hơn 0
            if (khoaHoc.SoLuongToiDa <= 0)
            {
                return (false, "Sĩ số học viên tối đa phải lớn hơn 0.");
            }

            // 6.2. Kiểm tra học phí không được âm
            if (khoaHoc.HocPhi < 0)
            {
                return (false, "Học phí khóa học không được âm.");
            }

            try
            {
                // Cập nhật thông tin khóa học và lưu vào cơ sở dữ liệu
                _context.KhoaHocs.Update(khoaHoc);
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateConcurrencyException)
            {
                // Xử lý lỗi xung đột dữ liệu đồng thời
                bool exists = await _context.KhoaHocs.AnyAsync(e => e.MaKhoaHoc == khoaHoc.MaKhoaHoc);
                if (!exists)
                {
                    return (false, "Không tìm thấy khóa học này trên hệ thống.");
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật khóa học.");
                return (false, "Không thể cập nhật khóa học do sự cố CSDL.");
            }
        }

        // =========================================================================
        // 8. Xóa khóa học
        // =========================================================================
        public async Task<KhoaHoc?> GetByIdForDeleteAsync(int id)
        {
            // Lấy thông tin khóa học cùng các thực thể liên quan trước khi xóa
            return await _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.GiangVien)
                .Include(k => k.DangKyKhoaHocs)
                .FirstOrDefaultAsync(k => k.MaKhoaHoc == id);
        }

        public async Task<bool> HasDangKyAsync(int id)
        {
            // 6.2. Kiểm tra khóa học đã phát sinh lịch sử đăng ký học hay chưa
            return await _context.DangKyKhoaHocs.AnyAsync(dk => dk.MaKhoaHoc == id);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteKhoaHocAsync(int id)
        {
            try
            {
                // 6.2. Chặn xóa khóa học đã có học viên đăng ký để bảo vệ toàn vẹn dữ liệu
                bool daCoDangKy = await HasDangKyAsync(id);
                if (daCoDangKy)
                {
                    return (false, "Không thể xóa khóa học này vì đã có học viên đăng ký. Bạn có thể chuyển trạng thái sang 'Đã hủy' để bảo toàn dữ liệu!");
                }

                var khoaHoc = await _context.KhoaHocs.FindAsync(id);
                if (khoaHoc == null)
                {
                    return (false, "Không tìm thấy khóa học cần xóa.");
                }

                // Thực hiện xóa khóa học khỏi cơ sở dữ liệu khi thỏa mãn điều kiện
                _context.KhoaHocs.Remove(khoaHoc);
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException)
            {
                // Xử lý lỗi vi phạm ràng buộc toàn vẹn khóa ngoại trong CSDL
                return (false, "Không thể xóa khóa học này do đang được liên kết với dữ liệu khác trong hệ thống.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa khóa học.");
                return (false, "Đã xảy ra sự cố khi xóa khóa học. Vui lòng thử lại sau.");
            }
        }

        // =========================================================================
        // 9. Cập nhật trạng thái khóa học
        // =========================================================================
        public async Task<(bool Success, string? ErrorMessage, string StatusLabel)> UpdateStatusAsync(int id, string newStatus)
        {
            try
            {
                // Kiểm tra tính hợp lệ của trạng thái cần cập nhật
                var validStatuses = new[] { "SapMo", "DangMo", "DangHoc", "DaKetThuc", "BiHuy" };
                if (!validStatuses.Contains(newStatus))
                {
                    return (false, "Trạng thái cập nhật không hợp lệ.", string.Empty);
                }

                var khoaHoc = await _context.KhoaHocs.FindAsync(id);
                if (khoaHoc == null)
                {
                    return (false, "Không tìm thấy khóa học.", string.Empty);
                }

                // Cập nhật trạng thái mới cho khóa học và lưu vào cơ sở dữ liệu
                khoaHoc.TrangThai = newStatus;
                await _context.SaveChangesAsync();

                var label = FormatStatusLabel(newStatus);
                return (true, null, label);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái khóa học.");
                return (false, "Không thể cập nhật trạng thái khóa học do sự cố CSDL.", string.Empty);
            }
        }

        // =========================================================================
        // 10. Định dạng trạng thái khóa học
        // =========================================================================
        public string GetStatusLabel(string? trangThai) => FormatStatusLabel(trangThai);

        public static string FormatStatusLabel(string? trangThai)
        {
            // Chuyển đổi mã trạng thái sang nhãn tiếng Việt hiển thị trên giao diện
            return trangThai switch
            {
                "SapMo" => "Sắp mở",
                "DangMo" => "Đang mở đăng ký",
                "DangHoc" => "Đang giảng dạy",
                "DaKetThuc" => "Đã kết thúc",
                "BiHuy" => "Đã hủy",
                _ => trangThai ?? "Chưa xác định"
            };
        }
    }
}
