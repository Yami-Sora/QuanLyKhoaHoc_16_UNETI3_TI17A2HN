// Họ và tên: Nguyễn Văn Mạnh
// Mã sinh viên: 23103100096
// Nội dung thực hiện: Module 2 - Triển khai KhoaHocRepository (Repository Pattern)

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

        // 1. Đồng bộ vòng đời khóa học theo ngày thực tế
        public async Task DongBoVongDoiKhoaHocAsync()
        {
            try
            {
                var today = DateTime.Today;

                // Tự động chuyển sang đang giảng dạy khi đã đến ngày khai giảng
                var canDangHoc = await _context.KhoaHocs
                    .Where(k => (k.TrangThai == "SapMo" || k.TrangThai == "DangMo") && k.NgayBatDau.Date <= today && k.NgayKetThuc.Date >= today)
                    .ToListAsync();

                // Tự động chuyển sang đã kết thúc khi quá ngày kết thúc
                var canKetThuc = await _context.KhoaHocs
                    .Where(k => k.TrangThai != "DaKetThuc" && k.TrangThai != "BiHuy" && k.NgayKetThuc.Date < today)
                    .ToListAsync();

                bool coThayDoi = false;
                if (canDangHoc.Count > 0)
                {
                    canDangHoc.ForEach(k => k.TrangThai = "DangHoc");
                    coThayDoi = true;
                }
                if (canKetThuc.Count > 0)
                {
                    canKetThuc.ForEach(k => k.TrangThai = "DaKetThuc");
                    coThayDoi = true;
                }

                if (coThayDoi) await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đồng bộ vòng đời khóa học.");
            }
        }

        // 2. Tra cứu, lọc, sắp xếp và phân trang khóa học (công khai)
        public async Task<(KhoaHocFilterViewModel Filter, SelectList MonHocList)> GetFilterViewModelAsync(
            KhoaHocFilterViewModel filter, int? page = null)
        {
            await DongBoVongDoiKhoaHocAsync();

            if (page.HasValue && page.Value > 0)
                filter.PageIndex = page.Value;

            // Dropdown môn học
            var monHocList = await GetMonHocSelectListAsync(filter.MaMonHoc);

            // Thống kê tổng quan cho Stat Cards
            var allCourses = _context.KhoaHocs.AsNoTracking();
            filter.TongSoKhoaHoc = await allCourses.CountAsync();
            filter.SoKhoaDangMo = await allCourses.CountAsync(k => k.TrangThai == "DangMo");
            filter.SoKhoaConCho = await allCourses.CountAsync(k =>
                k.SoLuongToiDa > k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"));
            filter.SoKhoaKhaDung = await allCourses.CountAsync(k =>
                k.TrangThai == "DangMo" && k.NgayBatDau > DateTime.Today
                && k.SoLuongToiDa > k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"));
            filter.HocPhiThapNhat = await allCourses.AnyAsync() ? await allCourses.MinAsync(k => k.HocPhi) : 0m;
            filter.HocPhiCaoNhat = await allCourses.AnyAsync() ? await allCourses.MaxAsync(k => k.HocPhi) : 0m;

            // Truy vấn danh sách khóa học kèm dữ liệu liên kết
            var baseQuery = _context.KhoaHocs
                .Include(k => k.MonHoc).Include(k => k.GiangVien).Include(k => k.DangKyKhoaHocs)
                .AsNoTracking();

            var filteredQuery = ApplyFilterAndSort(baseQuery, filter.SearchString, filter.MaMonHoc,
                filter.TrangThai, filter.HinhThuc, filter.KhoangHocPhi, filter.TinhTrangCho, filter.SortBy);

            // Phân trang: Đảm bảo PageIndex luôn hợp lệ không vượt quá TotalPages khi kết hợp nhiều bộ lọc
            filter.TotalItems = await filteredQuery.CountAsync();
            if (filter.PageSize < 1) filter.PageSize = 6;
            if (filter.PageIndex < 1) filter.PageIndex = 1;
            if (filter.TotalPages > 0 && filter.PageIndex > filter.TotalPages)
                filter.PageIndex = filter.TotalPages;

            var pagedEntities = await filteredQuery
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            // Ánh xạ sang ViewModel
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

        // 3. Danh sách quản trị khóa học (tìm kiếm, lọc, phân trang) với stat cards
        public async Task<(List<KhoaHoc> Items, int TotalCount, int DangMoCount, int DangHocCount, int DaKetThucCount, long AvgHocPhi, int TotalFiltered, int TotalPages, SelectList MonHocList)> GetQuanLyDataAsync(
            string? searchString, int? maMonHoc, string? trangThai, string? hinhThuc,
            string? khoangHocPhi, string? tinhTrangCho, string? sortBy, int page, int pageSize)
        {
            await DongBoVongDoiKhoaHocAsync();

            // Dropdown môn học
            var monHocList = await GetMonHocSelectListAsync(maMonHoc);

            // Thống kê Stat Cards
            var totalCount = await _context.KhoaHocs.CountAsync();
            var dangMoCount = await _context.KhoaHocs.CountAsync(k => k.TrangThai == "DangMo");
            var dangHocCount = await _context.KhoaHocs.CountAsync(k => k.TrangThai == "DangHoc");
            var daKetThucCount = await _context.KhoaHocs.CountAsync(k => k.TrangThai == "DaKetThuc");
            var avgHocPhi = await _context.KhoaHocs.AnyAsync()
                ? (long)await _context.KhoaHocs.AverageAsync(k => (double)k.HocPhi) : 0L;

            // Truy vấn dữ liệu kèm quan hệ liên kết
            var baseQuery = _context.KhoaHocs
                .Include(k => k.MonHoc).Include(k => k.GiangVien).Include(k => k.DangKyKhoaHocs)
                .AsNoTracking();

            var filteredQuery = ApplyFilterAndSort(baseQuery, searchString, maMonHoc,
                trangThai, hinhThuc, khoangHocPhi, tinhTrangCho, sortBy);

            // Phân trang
            int totalFiltered = await filteredQuery.CountAsync();
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            int totalPages = (int)Math.Ceiling(totalFiltered / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages) page = totalPages;

            var items = await filteredQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount, dangMoCount, dangHocCount, daKetThucCount, avgHocPhi, totalFiltered, totalPages, monHocList);
        }

        // 4. Chi tiết khóa học kèm môn học, giảng viên và danh sách đăng ký
        public async Task<KhoaHoc?> GetByIdWithDetailsAsync(int id)
        {
            await DongBoVongDoiKhoaHocAsync();

            return await _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.GiangVien)
                .Include(k => k.DangKyKhoaHocs).ThenInclude(dk => dk.HocVien)
                .FirstOrDefaultAsync(k => k.MaKhoaHoc == id);
        }

        // 5. Kiểm tra học viên đã đăng ký khóa học chưa
        public async Task<bool> CheckHocVienDaDangKyAsync(int maKhoaHoc, int maTaiKhoan)
        {
            var hocVien = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);
            if (hocVien == null) return false;

            return await _context.DangKyKhoaHocs
                .AnyAsync(dk => dk.MaKhoaHoc == maKhoaHoc && dk.MaHocVien == hocVien.MaHocVien && dk.TrangThai != "BiHuy");
        }

        // 6. Lấy khóa học theo Id
        public async Task<KhoaHoc?> GetByIdAsync(int id)
        {
            return await _context.KhoaHocs.FindAsync(id);
        }

        // 7. Lấy dữ liệu dropdown môn học và giảng viên
        public async Task<(SelectList MonHocList, SelectList GiangVienList)> GetDropdownDataAsync(
            int? selectedMonHoc = null, int? selectedGiangVien = null)
        {
            var monHocs = await _context.MonHocs
                .Where(m => m.TrangThai).OrderBy(m => m.TenMonHoc)
                .Select(m => new { m.MaMonHoc, Display = $"{m.TenMonHoc} ({m.SoTinChi} TC)" })
                .ToListAsync();

            var giangViens = await _context.GiangViens
                .Where(g => g.TrangThai).OrderBy(g => g.HoTen)
                .Select(g => new { g.MaGiangVien, Display = $"{g.HoTen} - {g.ChuyenMon ?? "N/A"}" })
                .ToListAsync();

            return (
                new SelectList(monHocs, "MaMonHoc", "Display", selectedMonHoc),
                new SelectList(giangViens, "MaGiangVien", "Display", selectedGiangVien)
            );
        }

        // 8. Thêm mới khóa học
        public async Task<(bool Success, string? ErrorMessage, int MaKhoaHoc)> CreateKhoaHocAsync(KhoaHoc khoaHoc)
        {
            if (khoaHoc.NgayKetThuc <= khoaHoc.NgayBatDau)
                return (false, "Ngày kết thúc phải sau ngày bắt đầu khóa học.", 0);
            if (khoaHoc.SoLuongToiDa <= 0)
                return (false, "Sĩ số học viên tối đa phải lớn hơn 0.", 0);
            if (khoaHoc.HocPhi < 0)
                return (false, "Học phí khóa học không được âm.", 0);

            var today = DateTime.Today;

            // Kiểm tra ràng buộc trạng thái khi tạo mới
            if (khoaHoc.TrangThai == "SapMo" && khoaHoc.NgayBatDau.Date <= today)
                return (false, $"Không thể tạo khóa học ở trạng thái 'Sắp mở' vì ngày bắt đầu ({khoaHoc.NgayBatDau:dd/MM/yyyy}) đã đến hoặc đã qua.", 0);

            if (khoaHoc.TrangThai == "DangMo" && khoaHoc.NgayBatDau.Date <= today)
                return (false, $"Không thể tạo khóa học ở trạng thái 'Đang mở đăng ký' vì lớp học đã đến hoặc qua ngày bắt đầu giảng dạy ({khoaHoc.NgayBatDau:dd/MM/yyyy}).", 0);

            if (khoaHoc.TrangThai == "DaKetThuc" && khoaHoc.NgayKetThuc.Date > today)
                return (false, $"Không thể tạo khóa học ở trạng thái 'Đã kết thúc' vì chưa đến ngày kết thúc ({khoaHoc.NgayKetThuc:dd/MM/yyyy}).", 0);

            if (khoaHoc.TrangThai == "DangHoc" && khoaHoc.NgayBatDau.Date > today)
                return (false, $"Không thể tạo khóa học ở trạng thái 'Đang giảng dạy' vì chưa đến ngày khai giảng ({khoaHoc.NgayBatDau:dd/MM/yyyy}).", 0);

            if (khoaHoc.TrangThai == "BiHuy")
                return (false, "Không thể tạo mới khóa học ở trạng thái 'Đã hủy'.", 0);

            try
            {
                khoaHoc.TenKhoaHoc = khoaHoc.TenKhoaHoc.Trim();
                khoaHoc.MoTa = khoaHoc.MoTa?.Trim();
                _context.KhoaHocs.Add(khoaHoc);
                await _context.SaveChangesAsync();
                return (true, null, khoaHoc.MaKhoaHoc);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm mới khóa học.");
                return (false, "Không thể lưu khóa học do lỗi kết nối CSDL.", 0);
            }
        }

        // 9. Cập nhật thông tin khóa học
        public async Task<(bool Success, string? ErrorMessage)> UpdateKhoaHocAsync(KhoaHoc khoaHoc)
        {
            if (khoaHoc.NgayKetThuc <= khoaHoc.NgayBatDau)
                return (false, "Ngày kết thúc phải sau ngày bắt đầu khóa học.");
            if (khoaHoc.SoLuongToiDa <= 0)
                return (false, "Sĩ số học viên tối đa phải lớn hơn 0.");
            if (khoaHoc.HocPhi < 0)
                return (false, "Học phí khóa học không được âm.");

            try
            {
                var existing = await _context.KhoaHocs.FindAsync(khoaHoc.MaKhoaHoc);
                if (existing == null)
                    return (false, "Không tìm thấy khóa học này trên hệ thống.");

                // Kiểm tra logic chuyển trạng thái và ràng buộc đăng ký
                if (existing.TrangThai != khoaHoc.TrangThai)
                {
                    var (valid, errorMsg) = await KiemTraChuyenTrangThaiAsync(existing, khoaHoc.TrangThai, khoaHoc.NgayBatDau, khoaHoc.NgayKetThuc);
                    if (!valid)
                        return (false, errorMsg);
                }
                else
                {
                    var today = DateTime.Today;
                    if (khoaHoc.TrangThai == "SapMo" && khoaHoc.NgayBatDau.Date <= today)
                        return (false, $"Không thể lưu khóa học ở trạng thái 'Sắp mở' vì ngày bắt đầu ({khoaHoc.NgayBatDau:dd/MM/yyyy}) đã đến hoặc đã qua.");

                    if (khoaHoc.TrangThai == "DangMo" && khoaHoc.NgayBatDau.Date <= today)
                        return (false, $"Không thể lưu khóa học ở trạng thái 'Đang mở đăng ký' vì lớp học đã đến hoặc qua ngày bắt đầu giảng dạy ({khoaHoc.NgayBatDau:dd/MM/yyyy}).");

                    if (khoaHoc.TrangThai == "DaKetThuc" && khoaHoc.NgayKetThuc.Date > today)
                        return (false, $"Không thể lưu khóa học ở trạng thái 'Đã kết thúc' vì chưa đến ngày kết thúc ({khoaHoc.NgayKetThuc:dd/MM/yyyy}).");
                }

                existing.TenKhoaHoc = khoaHoc.TenKhoaHoc.Trim();
                existing.MaMonHoc = khoaHoc.MaMonHoc;
                existing.MaGiangVien = khoaHoc.MaGiangVien;
                existing.NgayBatDau = khoaHoc.NgayBatDau;
                existing.NgayKetThuc = khoaHoc.NgayKetThuc;
                existing.SoLuongToiDa = khoaHoc.SoLuongToiDa;
                existing.HocPhi = khoaHoc.HocPhi;
                existing.HinhThuc = khoaHoc.HinhThuc;
                existing.TrangThai = khoaHoc.TrangThai;
                existing.MoTa = khoaHoc.MoTa?.Trim();

                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.KhoaHocs.AnyAsync(e => e.MaKhoaHoc == khoaHoc.MaKhoaHoc))
                    return (false, "Không tìm thấy khóa học này trên hệ thống.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật khóa học ID {Id}", khoaHoc.MaKhoaHoc);
                return (false, "Không thể cập nhật khóa học do sự cố CSDL.");
            }
        }

        // 10. Lấy thông tin khóa học để xác nhận xóa
        public async Task<KhoaHoc?> GetByIdForDeleteAsync(int id)
        {
            return await _context.KhoaHocs
                .Include(k => k.MonHoc).Include(k => k.GiangVien).Include(k => k.DangKyKhoaHocs)
                .FirstOrDefaultAsync(k => k.MaKhoaHoc == id);
        }

        // 11. Kiểm tra khóa học đã có đăng ký chưa
        public async Task<bool> HasDangKyAsync(int id)
        {
            return await _context.DangKyKhoaHocs.AnyAsync(dk => dk.MaKhoaHoc == id);
        }

        // 12. Xóa khóa học an toàn
        public async Task<(bool Success, string? ErrorMessage)> DeleteKhoaHocAsync(int id)
        {
            try
            {
                if (await HasDangKyAsync(id))
                    return (false, "Không thể xóa khóa học này vì đã có học viên đăng ký. Dữ liệu lịch sử cần được lưu trữ để bảo toàn tính toàn vẹn hệ thống!");

                var khoaHoc = await _context.KhoaHocs.FindAsync(id);
                if (khoaHoc == null)
                    return (false, "Không tìm thấy khóa học cần xóa.");

                _context.KhoaHocs.Remove(khoaHoc);
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException)
            {
                return (false, "Không thể xóa khóa học này do đang liên kết với dữ liệu khác trong hệ thống.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa khóa học ID {Id}", id);
                return (false, "Đã xảy ra sự cố khi xóa khóa học. Vui lòng thử lại sau.");
            }
        }

        // 13. Cập nhật nhanh trạng thái khóa học
        public async Task<(bool Success, string? ErrorMessage, string StatusLabel)> UpdateStatusAsync(int id, string newStatus)
        {
            try
            {
                var khoaHoc = await _context.KhoaHocs.FindAsync(id);
                if (khoaHoc == null)
                    return (false, "Không tìm thấy khóa học cần cập nhật.", string.Empty);

                // Kiểm tra nghiệp vụ chuyển trạng thái
                var (isValid, errorMessage) = await KiemTraChuyenTrangThaiAsync(khoaHoc, newStatus);
                if (!isValid)
                    return (false, errorMessage, string.Empty);

                khoaHoc.TrangThai = newStatus;
                await _context.SaveChangesAsync();
                return (true, null, FormatStatusLabel(newStatus));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái khóa học ID {Id}", id);
                return (false, "Không thể cập nhật trạng thái khóa học do sự cố CSDL.", string.Empty);
            }
        }

        // 14. Định dạng nhãn trạng thái tiếng Việt
        public string GetStatusLabel(string? trangThai) => FormatStatusLabel(trangThai);

        public static string FormatStatusLabel(string? trangThai) => trangThai switch
        {
            "SapMo"     => "Sắp mở",
            "DangMo"    => "Đang mở đăng ký",
            "DangHoc"   => "Đang giảng dạy",
            "DaKetThuc" => "Đã kết thúc",
            "BiHuy"     => "Đã hủy",
            _           => trangThai ?? "Chưa xác định"
        };

        // === Hàm bổ trợ ===

        // Kiểm tra tính hợp lệ của việc chuyển đổi trạng thái vòng đời và ràng buộc đăng ký
        private async Task<(bool Valid, string? ErrorMessage)> KiemTraChuyenTrangThaiAsync(
            KhoaHoc khoaHoc, string newStatus, DateTime? ngayBatDauMoi = null, DateTime? ngayKetThucMoi = null)
        {
            var validStatuses = new[] { "SapMo", "DangMo", "DangHoc", "DaKetThuc", "BiHuy" };
            if (!validStatuses.Contains(newStatus))
                return (false, "Trạng thái cập nhật không hợp lệ.");

            if (khoaHoc.TrangThai == newStatus && !ngayBatDauMoi.HasValue && !ngayKetThucMoi.HasValue)
                return (false, $"Khóa học hiện đã ở trạng thái '{FormatStatusLabel(newStatus)}'.");

            var today = DateTime.Today;
            var batDau = (ngayBatDauMoi ?? khoaHoc.NgayBatDau).Date;
            var ketThuc = (ngayKetThucMoi ?? khoaHoc.NgayKetThuc).Date;

            // 1. Chuyển sang "Sắp mở": chỉ hợp lệ khi chưa đến ngày bắt đầu
            if (newStatus == "SapMo")
            {
                if (batDau <= today)
                {
                    return (false, $"Không thể chuyển khóa học sang trạng thái 'Sắp mở' vì khóa học đã đến hoặc qua ngày bắt đầu ({batDau:dd/MM/yyyy}).");
                }
                if (khoaHoc.TrangThai == "DangHoc" || khoaHoc.TrangThai == "DaKetThuc")
                {
                    return (false, $"Không thể chuyển khóa học từ trạng thái '{FormatStatusLabel(khoaHoc.TrangThai)}' về 'Sắp mở'.");
                }
            }

            // 2. Chuyển sang "Đã kết thúc": chỉ hợp lệ khi đã đến hoặc qua ngày kết thúc
            if (newStatus == "DaKetThuc")
            {
                if (ketThuc > today)
                {
                    return (false, $"Không thể chuyển khóa học sang trạng thái 'Đã kết thúc' vì chưa đến ngày kết thúc ({ketThuc:dd/MM/yyyy}).");
                }
            }

            // 3. Khóa học đã quá ngày kết thúc: không được chuyển về SapMo, DangMo, DangHoc
            if (ketThuc < today && (newStatus == "SapMo" || newStatus == "DangMo" || newStatus == "DangHoc"))
            {
                return (false, $"Không thể chuyển sang trạng thái '{FormatStatusLabel(newStatus)}' vì khóa học đã quá ngày kết thúc ({ketThuc:dd/MM/yyyy}).");
            }

            // 4. Chuyển sang "Đang giảng dạy": chỉ hợp lệ khi đã đến ngày bắt đầu
            if (newStatus == "DangHoc")
            {
                if (batDau > today)
                {
                    return (false, $"Không thể chuyển khóa học sang trạng thái 'Đang giảng dạy' vì chưa đến ngày khai giảng ({batDau:dd/MM/yyyy}).");
                }
            }

            // 5. Chuyển sang "Đã hủy": chỉ được thực hiện khi khóa học không có học sinh nào đăng ký
            if (newStatus == "BiHuy")
            {
                var soLuongDangKy = await _context.DangKyKhoaHocs.CountAsync(dk => dk.MaKhoaHoc == khoaHoc.MaKhoaHoc);
                if (soLuongDangKy > 0)
                {
                    return (false, $"Không thể hủy khóa học vì đã có {soLuongDangKy} học viên đăng ký. Khóa học chỉ được phép hủy khi chưa có học viên nào đăng ký!");
                }
            }

            // 6. Chuyển sang "Đang mở đăng ký": chỉ cho phép khi lớp học chưa bắt đầu giảng dạy
            if (newStatus == "DangMo")
            {
                if (batDau <= today)
                {
                    return (false, $"Không thể chuyển khóa học sang trạng thái 'Đang mở đăng ký' vì lớp học đã đến hoặc qua ngày bắt đầu giảng dạy ({batDau:dd/MM/yyyy}).");
                }
                if (khoaHoc.TrangThai == "DangHoc")
                {
                    return (false, "Không thể chuyển khóa học sang trạng thái 'Đang mở đăng ký' vì lớp học đang trong quá trình giảng dạy.");
                }
                if (khoaHoc.TrangThai == "DaKetThuc")
                {
                    return (false, "Không thể chuyển khóa học sang trạng thái 'Đang mở đăng ký' vì khóa học đã kết thúc.");
                }
                if (ketThuc < today)
                {
                    return (false, $"Không thể chuyển khóa học sang trạng thái 'Đang mở đăng ký' vì khóa học đã quá ngày kết thúc ({ketThuc:dd/MM/yyyy}).");
                }
            }

            return (true, null);
        }

        // Lấy danh sách môn học cho dropdown bộ lọc
        private async Task<SelectList> GetMonHocSelectListAsync(int? selected = null)
        {
            var monHocs = await _context.MonHocs
                .Where(m => m.TrangThai).OrderBy(m => m.TenMonHoc)
                .Select(m => new { m.MaMonHoc, Display = $"{m.TenMonHoc} ({m.SoTinChi} TC)" })
                .ToListAsync();
            return new SelectList(monHocs, "MaMonHoc", "Display", selected);
        }

        // Áp dụng tìm kiếm, lọc kết hợp và sắp xếp
        private static IQueryable<KhoaHoc> ApplyFilterAndSort(
            IQueryable<KhoaHoc> query, string? searchString, int? maMonHoc,
            string? trangThai, string? hinhThuc, string? khoangHocPhi,
            string? tinhTrangCho, string? sortBy)
        {
            // 1. Tìm kiếm theo tên, môn học, giảng viên hoặc mã khóa học
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.Trim();
                var lowerTerm = term.ToLower();

                // Nhận diện tìm kiếm theo mã định danh (#1, KH01, hoặc số 1)
                bool isIdSearch = false;
                int searchId = 0;
                var rawIdStr = lowerTerm.TrimStart('#');
                if (rawIdStr.StartsWith("kh"))
                    rawIdStr = rawIdStr.Substring(2);
                rawIdStr = rawIdStr.TrimStart('0');
                if (int.TryParse(rawIdStr, out searchId) && searchId > 0 &&
                    (term.StartsWith("#") || lowerTerm.StartsWith("kh") || int.TryParse(term, out _)))
                {
                    isIdSearch = true;
                }

                query = query.Where(k =>
                    k.TenKhoaHoc.ToLower().Contains(lowerTerm) ||
                    (k.MonHoc != null && k.MonHoc.TenMonHoc.ToLower().Contains(lowerTerm)) ||
                    (k.GiangVien != null && k.GiangVien.HoTen.ToLower().Contains(lowerTerm)) ||
                    (isIdSearch && k.MaKhoaHoc == searchId));
            }

            // 2. Lọc theo môn học
            if (maMonHoc.HasValue && maMonHoc.Value > 0)
                query = query.Where(k => k.MaMonHoc == maMonHoc.Value);

            // 3. Lọc theo trạng thái
            if (!string.IsNullOrWhiteSpace(trangThai) &&
                !trangThai.Equals("all", StringComparison.OrdinalIgnoreCase) &&
                !trangThai.Equals("tatca", StringComparison.OrdinalIgnoreCase))
            {
                var cleanStatus = trangThai.Trim();
                query = query.Where(k => k.TrangThai == cleanStatus);
            }

            // 4. Lọc theo hình thức đào tạo
            if (!string.IsNullOrWhiteSpace(hinhThuc) &&
                !hinhThuc.Equals("all", StringComparison.OrdinalIgnoreCase) &&
                !hinhThuc.Equals("tatca", StringComparison.OrdinalIgnoreCase))
            {
                var cleanHinhThuc = hinhThuc.Trim();
                query = query.Where(k => k.HinhThuc == cleanHinhThuc);
            }

            // 5. Lọc theo khoảng học phí
            if (!string.IsNullOrWhiteSpace(khoangHocPhi) &&
                !khoangHocPhi.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                var cleanKhoangHocPhi = khoangHocPhi.Trim().ToLower();
                query = cleanKhoangHocPhi switch
                {
                    "duoi_2tr" => query.Where(k => k.HocPhi < 2000000m),
                    "2tr_4tr"  => query.Where(k => k.HocPhi >= 2000000m && k.HocPhi <= 4000000m),
                    "4tr_6tr"  => query.Where(k => k.HocPhi > 4000000m && k.HocPhi <= 6000000m),
                    "tren_6tr" => query.Where(k => k.HocPhi > 6000000m),
                    _ => query
                };
            }

            // 6. Lọc theo tình trạng chỗ
            if (!string.IsNullOrWhiteSpace(tinhTrangCho) &&
                !tinhTrangCho.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                var cleanTinhTrangCho = tinhTrangCho.Trim().ToLower();
                if (cleanTinhTrangCho == "con_cho")
                    query = query.Where(k => k.SoLuongToiDa > k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"));
                else if (cleanTinhTrangCho == "het_cho")
                    query = query.Where(k => k.SoLuongToiDa <= k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"));
                else if (cleanTinhTrangCho == "khadung")
                {
                    var today = DateTime.Today;
                    query = query.Where(k => k.TrangThai == "DangMo" && k.NgayBatDau > today && k.SoLuongToiDa > k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"));
                }
            }

            // 7. Sắp xếp danh sách
            return sortBy switch
            {
                "ten_asc"     => query.OrderBy(k => k.TenKhoaHoc),
                "ten_desc"    => query.OrderByDescending(k => k.TenKhoaHoc),
                "ngay_asc"    => query.OrderBy(k => k.NgayBatDau).ThenBy(k => k.TenKhoaHoc),
                "ngay_desc"   => query.OrderByDescending(k => k.NgayBatDau).ThenBy(k => k.TenKhoaHoc),
                "hocphi_asc"  => query.OrderBy(k => k.HocPhi).ThenBy(k => k.TenKhoaHoc),
                "hocphi_desc" => query.OrderByDescending(k => k.HocPhi).ThenBy(k => k.TenKhoaHoc),
                "cho_asc"     => query.OrderBy(k => k.SoLuongToiDa - k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy")).ThenBy(k => k.TenKhoaHoc),
                "cho_desc"    => query.OrderByDescending(k => k.SoLuongToiDa - k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy")).ThenBy(k => k.TenKhoaHoc),
                "id_asc"      => query.OrderBy(k => k.MaKhoaHoc),
                _             => query.OrderByDescending(k => k.MaKhoaHoc)
            };
        }
    }
}
