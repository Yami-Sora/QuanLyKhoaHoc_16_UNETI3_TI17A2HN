// Họ và tên: Ngọc Tùng Lâm
// Mã sinh viên: 23103100098
// Nội dung thực hiện: Module 3 - Triển khai HocVienRepository (Repository Pattern)

using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Helpers;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories
{
    public class HocVienRepository : IHocVienRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HocVienRepository> _logger;

        public HocVienRepository(ApplicationDbContext context, ILogger<HocVienRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<HocVienFilterViewModel> GetFilterViewModelAsync(HocVienFilterViewModel filter)
        {
            if (filter.PageIndex < 1) filter.PageIndex = 1;
            if (filter.PageSize < 1) filter.PageSize = 10;

            // 1. Thống kê Stat Cards tổng quan bằng LINQ
            var allQuery = _context.HocViens.AsNoTracking();
            filter.TongSoHocVien = await allQuery.CountAsync();
            filter.SoHocVienHoatDong = await allQuery.CountAsync(h => h.TrangThai);
            filter.SoHocVienBiKhoa = await allQuery.CountAsync(h => !h.TrangThai);
            filter.SoHocVienCoDangKy = await allQuery.CountAsync(h => h.DangKyKhoaHocs.Any(d => d.TrangThai != "BiHuy"));

            // 2. Query cơ sở kèm nạp Eager Loading
            var query = _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyKhoaHocs)
                .AsNoTracking()
                .AsQueryable();

            // 3. Tìm kiếm từ khóa tổng hợp
            if (!string.IsNullOrWhiteSpace(filter.SearchString))
            {
                var keyword = filter.SearchString.Trim();
                int? parsedId = null;
                var digits = System.Text.RegularExpressions.Regex.Replace(keyword, @"\D", "");
                if (int.TryParse(digits, out int numVal) && numVal > 0)
                {
                    parsedId = numVal;
                }

                if (parsedId.HasValue)
                {
                    int id = parsedId.Value;
                    query = query.Where(h =>
                        h.MaHocVien == id ||
                        h.HoTen.Contains(keyword) ||
                        h.SoDienThoai.Contains(keyword) ||
                        h.Email.Contains(keyword) ||
                        (h.TaiKhoan != null && h.TaiKhoan.TenDangNhap.Contains(keyword)));
                }
                else
                {
                    query = query.Where(h =>
                        h.HoTen.Contains(keyword) ||
                        h.SoDienThoai.Contains(keyword) ||
                        h.Email.Contains(keyword) ||
                        (h.TaiKhoan != null && h.TaiKhoan.TenDangNhap.Contains(keyword)));
                }
            }

            // 4. Lọc theo Trạng thái hồ sơ
            if (!string.IsNullOrWhiteSpace(filter.TrangThai) && filter.TrangThai != "TatCa")
            {
                if (filter.TrangThai == "HoatDong")
                {
                    query = query.Where(h => h.TrangThai);
                }
                else if (filter.TrangThai == "BiKhoa")
                {
                    query = query.Where(h => !h.TrangThai);
                }
            }

            // 5. Lọc theo Trình độ học vấn
            if (!string.IsNullOrWhiteSpace(filter.TrinhDo) && filter.TrinhDo != "TatCa")
            {
                query = query.Where(h => h.TrinhDo == filter.TrinhDo);
            }

            // 6. Sắp xếp bằng LINQ
            query = (filter.SortBy ?? "ngay_desc") switch
            {
                "ten_asc" => query.OrderBy(h => h.HoTen),
                "ten_desc" => query.OrderByDescending(h => h.HoTen),
                "ngay_asc" => query.OrderBy(h => h.NgayDangKy),
                _ => query.OrderByDescending(h => h.NgayDangKy)
            };

            // 7. Tổng số kết quả lọc
            filter.TotalItems = await query.CountAsync();

            // 8. Phân trang bằng Skip() và Take()
            var pagedData = await query
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(h => new HocVienItemViewModel
                {
                    MaHocVien = h.MaHocVien,
                    MaTaiKhoan = h.MaTaiKhoan,
                    TenDangNhap = h.TaiKhoan != null ? h.TaiKhoan.TenDangNhap : null,
                    HoTen = h.HoTen,
                    NgaySinh = h.NgaySinh,
                    GioiTinh = h.GioiTinh,
                    SoDienThoai = h.SoDienThoai,
                    Email = h.Email,
                    DiaChi = h.DiaChi,
                    TrinhDo = h.TrinhDo,
                    NgayDangKy = h.NgayDangKy,
                    TrangThai = h.TrangThai,
                    HinhAnh = h.HinhAnh,
                    GhiChu = h.GhiChu,
                    SoKhoaHocDaDangKy = h.DangKyKhoaHocs.Count(d => d.TrangThai != "BiHuy"),
                    SoKhoaHocHoanThanh = h.DangKyKhoaHocs.Count(d => d.TrangThai == "HoanThanh"),
                    TongHocPhiDaDong = h.DangKyKhoaHocs.Sum(d => d.SoTienDaDong),
                    TongHocPhiConLai = h.DangKyKhoaHocs.Where(d => d.TrangThai != "BiHuy").Sum(d => d.SoTienConLai)
                })
                .ToListAsync();

            filter.DanhSachHocVien = pagedData;
            return filter;
        }

        public async Task<HocVienDetailsViewModel?> GetDetailsViewModelAsync(int id)
        {
            var hocVien = await _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyKhoaHocs)
                    .ThenInclude(d => d.KhoaHoc)
                        .ThenInclude(k => k!.MonHoc)
                .Include(h => h.DangKyKhoaHocs)
                    .ThenInclude(d => d.KhoaHoc)
                        .ThenInclude(k => k!.GiangVien)
                .Include(h => h.DangKyKhoaHocs)
                    .ThenInclude(d => d.KetQuaHocTap)
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.MaHocVien == id);

            if (hocVien == null) return null;

            var viewModel = new HocVienDetailsViewModel
            {
                HocVien = hocVien,
                TaiKhoan = hocVien.TaiKhoan,
                LịchSuDangKy = hocVien.DangKyKhoaHocs
                    .OrderByDescending(d => d.NgayDangKy)
                    .Select(d => new HocVienDangKyLichSuViewModel
                    {
                        MaDangKy = d.MaDangKy,
                        MaKhoaHoc = d.MaKhoaHoc,
                        TenKhoaHoc = d.KhoaHoc != null ? d.KhoaHoc.TenKhoaHoc : "N/A",
                        TenMonHoc = d.KhoaHoc != null && d.KhoaHoc.MonHoc != null ? d.KhoaHoc.MonHoc.TenMonHoc : "N/A",
                        SoTinChi = d.KhoaHoc != null && d.KhoaHoc.MonHoc != null ? d.KhoaHoc.MonHoc.SoTinChi : 0,
                        TenGiangVien = d.KhoaHoc != null && d.KhoaHoc.GiangVien != null ? d.KhoaHoc.GiangVien.HoTen : null,
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
                    .ToList()
            };

            return viewModel;
        }

        public async Task<HocVien?> GetByIdAsync(int id)
        {
            return await _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyKhoaHocs)
                .FirstOrDefaultAsync(h => h.MaHocVien == id);
        }

        public async Task<HocVien?> GetByIdWithTaiKhoanAsync(int id)
        {
            return await _context.HocViens
                .Include(h => h.TaiKhoan)
                .FirstOrDefaultAsync(h => h.MaHocVien == id);
        }

        public async Task<bool> ExistsEmailAsync(string email, int? excludeId = null)
        {
            var clean = email.Trim().ToLower();
            if (excludeId.HasValue)
            {
                return await _context.HocViens.AnyAsync(h => h.MaHocVien != excludeId.Value && h.Email.ToLower() == clean);
            }
            return await _context.HocViens.AnyAsync(h => h.Email.ToLower() == clean);
        }

        public async Task<bool> ExistsSoDienThoaiAsync(string soDienThoai, int? excludeId = null)
        {
            var clean = soDienThoai.Trim();
            if (excludeId.HasValue)
            {
                return await _context.HocViens.AnyAsync(h => h.MaHocVien != excludeId.Value && h.SoDienThoai == clean);
            }
            return await _context.HocViens.AnyAsync(h => h.SoDienThoai == clean);
        }

        public async Task<bool> ExistsTenDangNhapAsync(string tenDangNhap, int? excludeTaiKhoanId = null)
        {
            var clean = tenDangNhap.Trim().ToLower();
            if (excludeTaiKhoanId.HasValue)
            {
                return await _context.TaiKhoans.AnyAsync(t => t.MaTaiKhoan != excludeTaiKhoanId.Value && t.TenDangNhap.ToLower() == clean);
            }
            return await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap.ToLower() == clean);
        }

        public async Task<(bool Success, string? ErrorMessage, int MaHocVien)> CreateHocVienAsync(HocVienFormViewModel model, string? hinhAnhPath)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                TaiKhoan? newTaiKhoan = null;
                var cleanEmail = model.Email.Trim().ToLower();
                var cleanPhone = model.SoDienThoai.Trim();

                // 1. Tạo tài khoản nếu người dùng chọn
                if (model.TaoTaiKhoan && !string.IsNullOrWhiteSpace(model.TenDangNhap))
                {
                    newTaiKhoan = new TaiKhoan
                    {
                        TenDangNhap = model.TenDangNhap.Trim(),
                        MatKhau = PasswordHelper.HashPassword(model.MatKhau ?? "User@123"),
                        HoTen = model.HoTen.Trim(),
                        Email = cleanEmail,
                        VaiTro = VaiTro.HocVien,
                        TrangThai = model.TrangThai,
                        NgayTao = DateTime.Now
                    };
                    _context.TaiKhoans.Add(newTaiKhoan);
                    await _context.SaveChangesAsync();
                }

                // 2. Tạo bản ghi học viên
                var newHocVien = new HocVien
                {
                    MaTaiKhoan = newTaiKhoan?.MaTaiKhoan,
                    HoTen = model.HoTen.Trim(),
                    NgaySinh = model.NgaySinh,
                    GioiTinh = model.GioiTinh,
                    SoDienThoai = cleanPhone,
                    Email = cleanEmail,
                    DiaChi = model.DiaChi?.Trim(),
                    TrinhDo = model.TrinhDo?.Trim(),
                    TrangThai = model.TrangThai,
                    HinhAnh = hinhAnhPath ?? model.HinhAnh,
                    GhiChu = model.GhiChu?.Trim(),
                    NgayDangKy = DateTime.Now
                };

                _context.HocViens.Add(newHocVien);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, null, newHocVien.MaHocVien);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi thêm mới học viên trong HocVienRepository");
                return (false, ex.Message, 0);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateHocVienAsync(int id, HocVienFormViewModel model, string? hinhAnhPath)
        {
            try
            {
                var hocVien = await _context.HocViens
                    .Include(h => h.TaiKhoan)
                    .FirstOrDefaultAsync(h => h.MaHocVien == id);

                if (hocVien == null)
                {
                    return (false, "Không tìm thấy hồ sơ học viên.");
                }

                var cleanEmail = model.Email.Trim().ToLower();
                var cleanPhone = model.SoDienThoai.Trim();

                hocVien.HoTen = model.HoTen.Trim();
                hocVien.NgaySinh = model.NgaySinh;
                hocVien.GioiTinh = model.GioiTinh;
                hocVien.SoDienThoai = cleanPhone;
                hocVien.Email = cleanEmail;
                hocVien.DiaChi = model.DiaChi?.Trim();
                hocVien.TrinhDo = model.TrinhDo?.Trim();
                hocVien.TrangThai = model.TrangThai;
                if (!string.IsNullOrEmpty(hinhAnhPath))
                {
                    hocVien.HinhAnh = hinhAnhPath;
                }
                hocVien.GhiChu = model.GhiChu?.Trim();

                // Đồng bộ sang tài khoản liên kết nếu có
                if (hocVien.TaiKhoan != null)
                {
                    hocVien.TaiKhoan.HoTen = hocVien.HoTen;
                    hocVien.TaiKhoan.Email = hocVien.Email;
                    hocVien.TaiKhoan.TrangThai = hocVien.TrangThai;

                    // Nếu có nhập mật khẩu mới
                    if (!string.IsNullOrWhiteSpace(model.MatKhau))
                    {
                        hocVien.TaiKhoan.MatKhau = PasswordHelper.HashPassword(model.MatKhau.Trim());
                    }
                }

                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật học viên Id {Id}", id);
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteHocVienAsync(int id)
        {
            try
            {
                var hocVien = await _context.HocViens
                    .Include(h => h.DangKyKhoaHocs)
                    .Include(h => h.TaiKhoan)
                    .FirstOrDefaultAsync(h => h.MaHocVien == id);

                if (hocVien == null)
                {
                    return (false, "Không tìm thấy hồ sơ học viên cần xóa.");
                }

                // Kiểm tra ràng buộc toàn vẹn dữ liệu
                if (hocVien.DangKyKhoaHocs.Any())
                {
                    return (false, $"Không thể xóa học viên '{hocVien.HoTen}' vì đã có {hocVien.DangKyKhoaHocs.Count} lịch sử đăng ký khóa học. Để đảm bảo tính toàn vẹn dữ liệu, vui lòng chuyển trạng thái học viên sang 'Bị khóa'.");
                }

                _context.HocViens.Remove(hocVien);
                if (hocVien.TaiKhoan != null)
                {
                    _context.TaiKhoans.Remove(hocVien.TaiKhoan);
                }

                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xóa học viên Id {Id}", id);
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? ErrorMessage, bool NewStatus)> ToggleTrangThaiAsync(int id)
        {
            try
            {
                var hocVien = await _context.HocViens
                    .Include(h => h.TaiKhoan)
                    .FirstOrDefaultAsync(h => h.MaHocVien == id);

                if (hocVien == null)
                {
                    return (false, "Không tìm thấy hồ sơ học viên.", false);
                }

                hocVien.TrangThai = !hocVien.TrangThai;
                if (hocVien.TaiKhoan != null)
                {
                    hocVien.TaiKhoan.TrangThai = hocVien.TrangThai;
                }

                await _context.SaveChangesAsync();
                return (true, null, hocVien.TrangThai);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi đổi trạng thái học viên Id {Id}", id);
                return (false, ex.Message, false);
            }
        }

        public async Task<HoSoCaNhanViewModel?> GetHoSoCaNhanAsync(int maTaiKhoan)
        {
            var hocVien = await _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyKhoaHocs)
                .FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);

            if (hocVien == null)
            {
                var taiKhoan = await _context.TaiKhoans.FindAsync(maTaiKhoan);
                if (taiKhoan == null) return null;

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

            return new HoSoCaNhanViewModel
            {
                MaHocVien = hocVien.MaHocVien,
                MaTaiKhoan = hocVien.MaTaiKhoan,
                TenDangNhap = hocVien.TaiKhoan?.TenDangNhap ?? "N/A",
                HoTen = hocVien.HoTen,
                NgaySinh = hocVien.NgaySinh,
                GioiTinh = hocVien.GioiTinh ?? "Nam",
                SoDienThoai = hocVien.SoDienThoai,
                Email = hocVien.Email,
                DiaChi = hocVien.DiaChi,
                TrinhDo = hocVien.TrinhDo ?? "Đại học",
                HinhAnh = hocVien.HinhAnh,
                GhiChu = hocVien.GhiChu,
                NgayDangKy = hocVien.NgayDangKy,
                TrangThai = hocVien.TrangThai,
                TongKhoaHocDaDangKy = hocVien.DangKyKhoaHocs?.Count ?? 0,
                SoKhoaHocHoanThanh = hocVien.DangKyKhoaHocs?.Count(d => d.TrangThai == "HoanThanh") ?? 0,
                SoKhoaHocChoXuLy = hocVien.DangKyKhoaHocs?.Count(d => d.TrangThai == "ChoXuLy") ?? 0,
                TongHocPhiDaDong = hocVien.DangKyKhoaHocs?.Sum(d => d.SoTienDaDong) ?? 0,
                TongHocPhiConLai = hocVien.DangKyKhoaHocs?.Where(d => d.TrangThai != "BiHuy").Sum(d => d.SoTienConLai) ?? 0
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CapNhatHoSoAsync(int maTaiKhoan, HoSoCaNhanViewModel model, string? hinhAnhPath)
        {
            try
            {
                var hocVien = await _context.HocViens
                    .Include(h => h.TaiKhoan)
                    .FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);

                if (hocVien == null) return (false, "Không tìm thấy hồ sơ cá nhân.");

                var cleanEmail = model.Email.Trim().ToLower();
                var cleanPhone = model.SoDienThoai.Trim();

                // Kiểm tra trùng Email
                if (await _context.HocViens.AnyAsync(h => h.MaHocVien != hocVien.MaHocVien && h.Email.ToLower() == cleanEmail))
                {
                    return (false, "Email này đã được sử dụng bởi một tài khoản khác.");
                }

                // Kiểm tra trùng SĐT
                if (await _context.HocViens.AnyAsync(h => h.MaHocVien != hocVien.MaHocVien && h.SoDienThoai == cleanPhone))
                {
                    return (false, "Số điện thoại này đã được sử dụng bởi một tài khoản khác.");
                }

                hocVien.HoTen = model.HoTen.Trim();
                hocVien.NgaySinh = model.NgaySinh;
                hocVien.GioiTinh = model.GioiTinh;
                hocVien.SoDienThoai = cleanPhone;
                hocVien.Email = cleanEmail;
                hocVien.DiaChi = model.DiaChi?.Trim();
                hocVien.TrinhDo = model.TrinhDo?.Trim();
                if (!string.IsNullOrEmpty(hinhAnhPath))
                {
                    hocVien.HinhAnh = hinhAnhPath;
                }
                hocVien.GhiChu = model.GhiChu?.Trim();

                if (hocVien.TaiKhoan != null)
                {
                    hocVien.TaiKhoan.HoTen = hocVien.HoTen;
                    hocVien.TaiKhoan.Email = hocVien.Email;
                }

                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật hồ sơ cá nhân MaTaiKhoan {MaTaiKhoan}", maTaiKhoan);
                return (false, ex.Message);
            }
        }
    }
}
