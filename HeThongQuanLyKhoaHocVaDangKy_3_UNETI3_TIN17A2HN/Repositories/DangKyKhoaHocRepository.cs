// Họ và tên: Ngọc Tùng Lâm
// Mã sinh viên: 23103100098
// Nội dung thực hiện: Module 3 - Triển khai DangKyKhoaHocRepository (Repository Pattern)

using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories
{
    public class DangKyKhoaHocRepository : IDangKyKhoaHocRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DangKyKhoaHocRepository> _logger;

        public DangKyKhoaHocRepository(ApplicationDbContext context, ILogger<DangKyKhoaHocRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(DangKyKhoaHocViewModel? ViewModel, string? ErrorMessage)> GetDangKyFormAsync(int maKhoaHoc, int maTaiKhoan)
        {
            // 1. Tìm hoặc khởi tạo hồ sơ học viên liên kết
            var hocVien = await _context.HocViens
                .Include(h => h.TaiKhoan)
                .FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);

            if (hocVien == null)
            {
                var taiKhoan = await _context.TaiKhoans.FindAsync(maTaiKhoan);
                if (taiKhoan == null || !taiKhoan.TrangThai)
                {
                    return (null, "Tài khoản của bạn không hợp lệ hoặc đã bị khóa.");
                }

                hocVien = new HocVien
                {
                    MaTaiKhoan = taiKhoan.MaTaiKhoan,
                    HoTen = taiKhoan.HoTen,
                    Email = taiKhoan.Email,
                    SoDienThoai = string.Empty,
                    NgayDangKy = DateTime.Now,
                    TrangThai = true,
                    TrinhDo = "Đại học",
                    GioiTinh = "Nam"
                };
                _context.HocViens.Add(hocVien);
                await _context.SaveChangesAsync();
            }

            // 2. Kiểm tra tài khoản học viên có bị khóa không
            if (!hocVien.TrangThai)
            {
                return (null, "Hồ sơ học viên của bạn đang ở trạng thái tạm khóa. Vui lòng liên hệ Văn phòng đào tạo để được hỗ trợ.");
            }

            // 3. Lấy thông tin chi tiết Khóa học
            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.GiangVien)
                .Include(k => k.DangKyKhoaHocs)
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.MaKhoaHoc == maKhoaHoc);

            if (khoaHoc == null)
            {
                return (null, "Khóa học được yêu cầu không tồn tại trong hệ thống.");
            }

            // 4. Tính toán sĩ số hiện tại và số chỗ còn lại
            var siSoHienTai = khoaHoc.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy");
            var soChoConLai = Math.Max(0, khoaHoc.SoLuongToiDa - siSoHienTai);

            // 5. Kiểm tra giao dịch trùng lặp
            var donDangKyHienCo = await _context.DangKyKhoaHocs
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.MaHocVien == hocVien.MaHocVien && d.MaKhoaHoc == maKhoaHoc && d.TrangThai != "BiHuy");

            var model = new DangKyKhoaHocViewModel
            {
                MaKhoaHoc = khoaHoc.MaKhoaHoc,
                TenKhoaHoc = khoaHoc.TenKhoaHoc,
                TenMonHoc = khoaHoc.MonHoc?.TenMonHoc ?? "N/A",
                SoTinChi = khoaHoc.MonHoc?.SoTinChi ?? 0,
                HocPhi = khoaHoc.HocPhi,
                TenGiangVien = khoaHoc.GiangVien?.HoTen,
                HocVi = khoaHoc.GiangVien?.HocVi,
                NgayBatDau = khoaHoc.NgayBatDau,
                NgayKetThuc = khoaHoc.NgayKetThuc,
                HinhThuc = khoaHoc.HinhThuc,
                TrangThaiKhoaHoc = khoaHoc.TrangThai,
                SoLuongToiDa = khoaHoc.SoLuongToiDa,
                SiSoHienTai = siSoHienTai,
                MoTaKhoaHoc = khoaHoc.MoTa,

                MaHocVien = hocVien.MaHocVien,
                HoTenHocVien = hocVien.HoTen,
                EmailHocVien = hocVien.Email,
                SoDienThoaiHocVien = hocVien.SoDienThoai,

                DaDangKy = donDangKyHienCo != null,
                MaDangKyCu = donDangKyHienCo?.MaDangKy,
                TrangThaiDangKyCu = donDangKyHienCo?.TrangThai
            };

            // Xác định lý do không khả dụng
            if (khoaHoc.TrangThai != "DangMo")
            {
                model.LyDoKhongKhaDung = $"Khóa học hiện không ở trạng thái mở đăng ký (Trạng thái: {khoaHoc.TrangThai}).";
            }
            else if (soChoConLai <= 0)
            {
                model.LyDoKhongKhaDung = "Khóa học đã đủ sĩ số tối đa, không còn chỗ nhận thêm.";
            }
            else if (khoaHoc.NgayBatDau.Date < DateTime.Today)
            {
                model.LyDoKhongKhaDung = "Khóa học đã quá hạn ngày khai giảng bắt đầu.";
            }

            return (model, null);
        }

        public async Task<(bool Success, string? ErrorMessage, int MaDangKy)> XuLyDangKyAsync(DangKyKhoaHocViewModel model, int maTaiKhoan)
        {
            try
            {
                // 1. Xác định học viên thực tế từ MaTaiKhoan
                var hocVien = await _context.HocViens
                    .Include(h => h.TaiKhoan)
                    .FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);

                if (hocVien == null || !hocVien.TrangThai)
                {
                    return (false, "Hồ sơ học viên không hợp lệ hoặc đã bị khóa.", 0);
                }

                // 2. Tải khóa học và kiểm tra điều kiện
                var khoaHoc = await _context.KhoaHocs
                    .Include(k => k.MonHoc)
                    .Include(k => k.DangKyKhoaHocs)
                    .FirstOrDefaultAsync(k => k.MaKhoaHoc == model.MaKhoaHoc);

                if (khoaHoc == null)
                {
                    return (false, "Không tìm thấy khóa học cần đăng ký.", 0);
                }

                if (khoaHoc.TrangThai != "DangMo")
                {
                    return (false, "Khóa học hiện không trong thời gian mở tiếp nhận đăng ký.", 0);
                }

                var siSoHienTai = khoaHoc.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy");
                if (siSoHienTai >= khoaHoc.SoLuongToiDa)
                {
                    return (false, "Khóa học này đã đủ số lượng học viên tối đa.", 0);
                }

                if (khoaHoc.NgayBatDau.Date < DateTime.Today)
                {
                    return (false, "Khóa học đã quá ngày bắt đầu khai giảng, không thể tiếp nhận thêm học viên.", 0);
                }

                // 3. Kiểm tra chống đăng ký trùng
                var daDangKy = await _context.DangKyKhoaHocs.AnyAsync(d =>
                    d.MaHocVien == hocVien.MaHocVien &&
                    d.MaKhoaHoc == model.MaKhoaHoc &&
                    d.TrangThai != "BiHuy");

                if (daDangKy)
                {
                    return (false, "Bạn đã đăng ký khóa học này trước đó. Không được tạo giao dịch trùng lặp.", 0);
                }

                // 4. Tạo bản ghi đăng ký mới
                var dangKyMoi = new DangKyKhoaHoc
                {
                    MaHocVien = hocVien.MaHocVien,
                    MaKhoaHoc = khoaHoc.MaKhoaHoc,
                    NgayDangKy = DateTime.Now,
                    TrangThai = "ChoXuLy",
                    SoTienDaDong = 0,
                    SoTienConLai = khoaHoc.HocPhi,
                    GhiChu = string.IsNullOrWhiteSpace(model.GhiChu) ? null : model.GhiChu.Trim(),
                    NgayXacNhan = null
                };

                _context.DangKyKhoaHocs.Add(dangKyMoi);
                await _context.SaveChangesAsync();

                return (true, null, dangKyMoi.MaDangKy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý đăng ký khóa học");
                return (false, ex.Message, 0);
            }
        }

        public async Task<TheoDoiDangKyFilterViewModel> GetTheoDoiFilterAsync(int maTaiKhoan, TheoDoiDangKyFilterViewModel filter)
        {
            if (filter.PageIndex < 1) filter.PageIndex = 1;
            if (filter.PageSize < 1) filter.PageSize = 8;

            var hocVien = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);
            if (hocVien == null)
            {
                return filter;
            }

            // 1. Thống kê Stat Cards cá nhân bằng LINQ
            var myDonQuery = _context.DangKyKhoaHocs
                .Where(d => d.MaHocVien == hocVien.MaHocVien)
                .AsNoTracking();

            filter.TongSoDon = await myDonQuery.CountAsync();
            filter.SoDonChoXuLy = await myDonQuery.CountAsync(d => d.TrangThai == "ChoXuLy");
            filter.SoDonDangXuLy = await myDonQuery.CountAsync(d => d.TrangThai == "DangXuLy");
            filter.SoDonHoanThanh = await myDonQuery.CountAsync(d => d.TrangThai == "HoanThanh");
            filter.SoDonBiHuy = await myDonQuery.CountAsync(d => d.TrangThai == "BiHuy");
            filter.TongTienDaDong = await myDonQuery.SumAsync(d => (decimal?)d.SoTienDaDong) ?? 0;
            filter.TongTienConLai = await myDonQuery.Where(d => d.TrangThai != "BiHuy").SumAsync(d => (decimal?)d.SoTienConLai) ?? 0;

            // 2. Query danh sách giao dịch cá nhân
            var query = _context.DangKyKhoaHocs
                .Where(d => d.MaHocVien == hocVien.MaHocVien)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.MonHoc)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.GiangVien)
                .Include(d => d.KetQuaHocTap)
                .AsNoTracking()
                .AsQueryable();

            // 3. Lọc theo từ khóa
            if (!string.IsNullOrWhiteSpace(filter.SearchKhoaHoc))
            {
                var keyword = filter.SearchKhoaHoc.Trim();
                int? parsedId = null;
                var digits = System.Text.RegularExpressions.Regex.Replace(keyword, @"\D", "");
                if (int.TryParse(digits, out int numVal) && numVal > 0)
                {
                    parsedId = numVal;
                }

                if (parsedId.HasValue)
                {
                    int id = parsedId.Value;
                    query = query.Where(d =>
                        d.MaDangKy == id ||
                        (d.KhoaHoc != null && d.KhoaHoc.TenKhoaHoc.Contains(keyword)) ||
                        (d.KhoaHoc != null && d.KhoaHoc.MonHoc != null && d.KhoaHoc.MonHoc.TenMonHoc.Contains(keyword)));
                }
                else
                {
                    query = query.Where(d =>
                        (d.KhoaHoc != null && d.KhoaHoc.TenKhoaHoc.Contains(keyword)) ||
                        (d.KhoaHoc != null && d.KhoaHoc.MonHoc != null && d.KhoaHoc.MonHoc.TenMonHoc.Contains(keyword)));
                }
            }

            // 4. Lọc theo trạng thái
            if (!string.IsNullOrWhiteSpace(filter.TrangThai) && filter.TrangThai != "TatCa")
            {
                query = query.Where(d => d.TrangThai == filter.TrangThai);
            }

            // 5. Sắp xếp LINQ
            query = (filter.SortBy ?? "ngay_desc") switch
            {
                "ngay_asc" => query.OrderBy(d => d.NgayDangKy),
                "hocphi_desc" => query.OrderByDescending(d => d.KhoaHoc != null ? d.KhoaHoc.HocPhi : 0),
                "hocphi_asc" => query.OrderBy(d => d.KhoaHoc != null ? d.KhoaHoc.HocPhi : 0),
                _ => query.OrderByDescending(d => d.NgayDangKy)
            };

            // 6. Tổng số kết quả lọc
            filter.TotalItems = await query.CountAsync();

            // 7. Phân trang Skip / Take
            var listItems = await query
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(d => new TheoDoiDangKyItemViewModel
                {
                    MaDangKy = d.MaDangKy,
                    MaKhoaHoc = d.MaKhoaHoc,
                    TenKhoaHoc = d.KhoaHoc != null ? d.KhoaHoc.TenKhoaHoc : "N/A",
                    TenMonHoc = d.KhoaHoc != null && d.KhoaHoc.MonHoc != null ? d.KhoaHoc.MonHoc.TenMonHoc : "N/A",
                    SoTinChi = d.KhoaHoc != null && d.KhoaHoc.MonHoc != null ? d.KhoaHoc.MonHoc.SoTinChi : 0,
                    TenGiangVien = d.KhoaHoc != null && d.KhoaHoc.GiangVien != null ? d.KhoaHoc.GiangVien.HoTen : null,
                    NgayBatDau = d.KhoaHoc != null ? d.KhoaHoc.NgayBatDau : DateTime.MinValue,
                    NgayKetThuc = d.KhoaHoc != null ? d.KhoaHoc.NgayKetThuc : DateTime.MinValue,
                    HinhThuc = d.KhoaHoc != null ? d.KhoaHoc.HinhThuc : "Trực tiếp",
                    NgayDangKy = d.NgayDangKy,
                    TrangThai = d.TrangThai,
                    HocPhi = d.KhoaHoc != null ? d.KhoaHoc.HocPhi : 0,
                    SoTienDaDong = d.SoTienDaDong,
                    SoTienConLai = d.SoTienConLai,
                    NgayXacNhan = d.NgayXacNhan,
                    GhiChu = d.GhiChu,
                    DiemTongKet = d.KetQuaHocTap != null ? d.KetQuaHocTap.DiemTongKet : null,
                    XepLoai = d.KetQuaHocTap != null ? d.KetQuaHocTap.XepLoai : null,
                    KetQua = d.KetQuaHocTap != null ? d.KetQuaHocTap.KetQua : null
                })
                .ToListAsync();

            filter.DanhSachDangKy = listItems;
            return filter;
        }

        public async Task<(DangKyKhoaHoc? DangKy, string? ErrorMessage)> GetChiTietDangKyAsync(int id, int maTaiKhoan, bool isStaffOrAdmin)
        {
            var dangKy = await _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.MonHoc)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.GiangVien)
                .Include(d => d.KetQuaHocTap)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dangKy == null)
            {
                return (null, "Không tìm thấy hồ sơ đăng ký khóa học yêu cầu.");
            }

            // Kiểm tra quyền chính chủ
            var hocVien = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);
            if (!isStaffOrAdmin && (hocVien == null || dangKy.MaHocVien != hocVien.MaHocVien))
            {
                return (null, "Bạn không có quyền truy cập hồ sơ đăng ký của học viên khác!");
            }

            return (dangKy, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> HuyDangKyAsync(int id, string? lyDoHuy, int maTaiKhoan, bool isStaffOrAdmin)
        {
            try
            {
                var dangKy = await _context.DangKyKhoaHocs
                    .Include(d => d.KhoaHoc)
                    .FirstOrDefaultAsync(d => d.MaDangKy == id);

                if (dangKy == null)
                {
                    return (false, "Không tìm thấy đơn đăng ký cần hủy.");
                }

                // Kiểm tra quyền dữ liệu
                var hocVien = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);
                if (!isStaffOrAdmin && (hocVien == null || dangKy.MaHocVien != hocVien.MaHocVien))
                {
                    return (false, "Bạn không thể hủy đơn đăng ký của học viên khác!");
                }

                // Học viên chỉ được hủy khi ở trạng thái ChoXuLy
                if (!isStaffOrAdmin && dangKy.TrangThai != "ChoXuLy")
                {
                    return (false, $"Đơn đăng ký #{dangKy.MaDangKy} đang ở trạng thái '{dangKy.TrangThai}'. Theo quy chế, đơn đã tiếp nhận hoặc hoàn tất không được hủy tự do trực tuyến. Vui lòng liên hệ trực tiếp Bộ phận đào tạo.");
                }

                if (dangKy.TrangThai == "BiHuy")
                {
                    return (false, "Đơn đăng ký này đã được hủy trước đó.");
                }

                // Tiến hành hủy đơn
                dangKy.TrangThai = "BiHuy";
                dangKy.SoTienConLai = 0;
                var lyDo = string.IsNullOrWhiteSpace(lyDoHuy) ? "Học viên tự hủy qua cổng trực tuyến" : lyDoHuy.Trim();
                var thongTinHuy = $"[Học viên tự hủy: {lyDo} - Thời gian: {DateTime.Now:dd/MM/yyyy HH:mm}]";

                dangKy.GhiChu = string.IsNullOrEmpty(dangKy.GhiChu)
                    ? thongTinHuy
                    : $"{dangKy.GhiChu} | {thongTinHuy}";

                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hủy đơn đăng ký Id {Id}", id);
                return (false, ex.Message);
            }
        }

        public async Task<(DangKyKhoaHoc? DangKy, string? ErrorMessage)> GetPhieuDangKyAsync(int id, int maTaiKhoan, bool isStaffOrAdmin)
        {
            var dangKy = await _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.MonHoc)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.GiangVien)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dangKy == null)
            {
                return (null, "Không tìm thấy phiếu đăng ký.");
            }

            var hocVien = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);
            if (!isStaffOrAdmin && (hocVien == null || dangKy.MaHocVien != hocVien.MaHocVien))
            {
                return (null, "Bạn không có quyền in phiếu đăng ký của người khác!");
            }

            return (dangKy, null);
        }
    }
}
