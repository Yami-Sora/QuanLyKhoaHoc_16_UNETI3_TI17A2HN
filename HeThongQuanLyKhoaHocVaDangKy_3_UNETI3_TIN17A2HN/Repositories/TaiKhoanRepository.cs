// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Nội dung thực hiện: Module 1 - Triển khai TaiKhoanRepository (Repository Pattern)

using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Helpers;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories
{
    public class TaiKhoanRepository : ITaiKhoanRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TaiKhoanRepository> _logger;

        public TaiKhoanRepository(ApplicationDbContext context, ILogger<TaiKhoanRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 1. Đăng nhập: tìm tài khoản theo tên đăng nhập (kèm HocVien)
        public async Task<TaiKhoan?> GetByTenDangNhapAsync(string tenDangNhap)
        {
            var cleanUsername = (tenDangNhap ?? "").Trim().ToLower();
            return await _context.TaiKhoans
                .Include(t => t.HocVien)
                .FirstOrDefaultAsync(t => t.TenDangNhap.ToLower() == cleanUsername);
        }

        // 2. Tìm tài khoản theo Email
        public async Task<TaiKhoan?> GetByEmailAsync(string email)
        {
            var normalizedEmail = (email ?? "").Trim().ToLower();
            return await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.Email.ToLower() == normalizedEmail);
        }

        // 3. Tìm tài khoản theo Id
        public async Task<TaiKhoan?> GetByIdAsync(int id)
        {
            return await _context.TaiKhoans.FindAsync(id);
        }

        // 4. Tìm tài khoản theo Id kèm HocVien
        public async Task<TaiKhoan?> GetByIdWithHocVienAsync(int id)
        {
            return await _context.TaiKhoans
                .Include(t => t.HocVien)
                .FirstOrDefaultAsync(t => t.MaTaiKhoan == id);
        }

        // 5. Kiểm tra trùng tên đăng nhập
        public async Task<bool> ExistsTenDangNhapAsync(string tenDangNhap, int? excludeId = null)
        {
            var clean = (tenDangNhap ?? "").Trim().ToLower();
            if (excludeId.HasValue)
            {
                return await _context.TaiKhoans.AnyAsync(t => t.MaTaiKhoan != excludeId.Value && t.TenDangNhap.ToLower() == clean);
            }
            return await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap.ToLower() == clean);
        }

        // 6. Kiểm tra trùng Email
        public async Task<bool> ExistsEmailAsync(string email, int? excludeId = null)
        {
            var clean = (email ?? "").Trim().ToLower();
            if (excludeId.HasValue)
            {
                return await _context.TaiKhoans.AnyAsync(t => t.MaTaiKhoan != excludeId.Value && t.Email.ToLower() == clean);
            }
            return await _context.TaiKhoans.AnyAsync(t => t.Email.ToLower() == clean);
        }

        // 7. Đếm số Admin đang hoạt động
        public async Task<int> CountActiveAdminsAsync()
        {
            return await _context.TaiKhoans.CountAsync(t => t.VaiTro == VaiTro.Admin && t.TrangThai);
        }

        // 8. Đăng ký tài khoản học viên mới (kèm tạo hồ sơ HocVien)
        public async Task<(bool Success, string? ErrorMessage)> RegisterAsync(RegisterViewModel model)
        {
            string cleanUsername = (model.TenDangNhap ?? "").Trim();
            string cleanEmail = (model.Email ?? "").Trim();
            string cleanHoTen = (model.HoTen ?? "").Trim();
            string cleanPhone = string.IsNullOrWhiteSpace(model.SoDienThoai) ? string.Empty : model.SoDienThoai.Trim();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Tạo tài khoản mới với vai trò Học viên
                var taiKhoan = new TaiKhoan
                {
                    TenDangNhap = cleanUsername,
                    MatKhau = PasswordHelper.HashPassword(model.MatKhau),
                    HoTen = cleanHoTen,
                    Email = cleanEmail,
                    VaiTro = VaiTro.HocVien,
                    TrangThai = true,
                    NgayTao = DateTime.Now
                };

                _context.TaiKhoans.Add(taiKhoan);
                await _context.SaveChangesAsync();

                // Tạo hồ sơ học viên tương ứng
                var hocVien = new HocVien
                {
                    MaTaiKhoan = taiKhoan.MaTaiKhoan,
                    HoTen = taiKhoan.HoTen,
                    Email = taiKhoan.Email,
                    SoDienThoai = cleanPhone,
                    NgayDangKy = DateTime.Now,
                    TrangThai = true
                };
                _context.HocViens.Add(hocVien);
                await _context.SaveChangesAsync();

                // Cam kết Transaction thành công trọn vẹn
                await transaction.CommitAsync();
                return (true, null);
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                return (false, "Tên đăng nhập hoặc Email có thể đã được người khác sử dụng trong quá trình bạn gửi yêu cầu. Vui lòng thử lại.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi đăng ký tài khoản mới");
                return (false, "Không thể kết nối đến máy chủ CSDL hoặc xảy ra sự cố trong quá trình khởi tạo tài khoản. Vui lòng kiểm tra lại kết nối SQL Server và thử lại.");
            }
        }

        // 9. Tạo tài khoản mới bởi Admin (kèm hồ sơ HocVien nếu vai trò là HocVien)
        public async Task<(bool Success, string? ErrorMessage)> CreateByAdminAsync(CreateUserViewModel model)
        {
            string cleanUsername = (model.TenDangNhap ?? "").Trim();
            string cleanEmail = (model.Email ?? "").Trim();
            string cleanHoTen = (model.HoTen ?? "").Trim();
            string cleanPhone = (model.SoDienThoai ?? "").Trim();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var taiKhoan = new TaiKhoan
                {
                    TenDangNhap = cleanUsername,
                    MatKhau = PasswordHelper.HashPassword(model.MatKhau),
                    HoTen = cleanHoTen,
                    Email = cleanEmail,
                    VaiTro = model.VaiTro,
                    TrangThai = model.TrangThai,
                    NgayTao = DateTime.Now
                };

                _context.TaiKhoans.Add(taiKhoan);
                await _context.SaveChangesAsync();

                // Nếu vai trò là Học viên, tạo hồ sơ HocVien đồng bộ
                if (model.VaiTro == VaiTro.HocVien)
                {
                    var hocVien = new HocVien
                    {
                        MaTaiKhoan = taiKhoan.MaTaiKhoan,
                        HoTen = cleanHoTen,
                        Email = cleanEmail,
                        SoDienThoai = cleanPhone,
                        NgayDangKy = DateTime.Now,
                        TrangThai = model.TrangThai
                    };
                    _context.HocViens.Add(hocVien);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi tạo mới tài khoản bởi Admin");
                return (false, "Không thể tạo tài khoản do lỗi kết nối CSDL. Vui lòng thử lại sau.");
            }
        }

        // 10. Cập nhật thông tin tài khoản bởi Admin
        public async Task<(bool Success, string? ErrorMessage)> UpdateByAdminAsync(int id, EditUserViewModel model)
        {
            var user = await _context.TaiKhoans
                .Include(t => t.HocVien)
                .FirstOrDefaultAsync(t => t.MaTaiKhoan == id);

            if (user == null)
            {
                return (false, "Không tìm thấy tài khoản.");
            }

            string cleanEmail = (model.Email ?? "").Trim();
            string cleanHoTen = (model.HoTen ?? "").Trim();
            string cleanPhone = (model.SoDienThoai ?? "").Trim();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                user.HoTen = cleanHoTen;
                user.Email = cleanEmail;
                user.VaiTro = model.VaiTro;
                user.TrangThai = model.TrangThai;

                // Đồng bộ hồ sơ học viên
                if (user.HocVien != null)
                {
                    user.HocVien.HoTen = cleanHoTen;
                    user.HocVien.Email = cleanEmail;
                    user.HocVien.SoDienThoai = cleanPhone;
                    user.HocVien.TrangThai = model.TrangThai;
                }
                else if (model.VaiTro == VaiTro.HocVien)
                {
                    // Nếu đổi sang vai trò Học viên mà chưa có hồ sơ
                    _context.HocViens.Add(new HocVien
                    {
                        MaTaiKhoan = user.MaTaiKhoan,
                        HoTen = cleanHoTen,
                        Email = cleanEmail,
                        SoDienThoai = cleanPhone,
                        NgayDangKy = DateTime.Now,
                        TrangThai = model.TrangThai
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi cập nhật thông tin tài khoản ID {Id}", id);
                return (false, "Không thể lưu thông tin do lỗi kết nối CSDL.");
            }
        }

        // 11. Đổi mật khẩu
        public async Task<(bool Success, string? ErrorMessage)> ChangePasswordAsync(int maTaiKhoan, string matKhauHienTai, string matKhauMoi)
        {
            try
            {
                var user = await _context.TaiKhoans.FindAsync(maTaiKhoan);
                if (user == null)
                {
                    return (false, "Không tìm thấy tài khoản.");
                }

                // Kiểm tra mật khẩu hiện tại
                if (!PasswordHelper.VerifyPassword(matKhauHienTai, user.MatKhau))
                {
                    return (false, "Mật khẩu hiện tại không chính xác.");
                }

                // Cập nhật mật khẩu mới đã băm
                user.MatKhau = PasswordHelper.HashPassword(matKhauMoi);
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đổi mật khẩu cho tài khoản ID {Id}", maTaiKhoan);
                return (false, "Không thể cập nhật mật khẩu do lỗi kết nối CSDL. Vui lòng thử lại sau.");
            }
        }

        // 12. Đặt lại mật khẩu (sau khi xác thực token)
        public async Task<(bool Success, string? ErrorMessage)> ResetPasswordAsync(string email, string matKhauMoi)
        {
            try
            {
                var normalizedEmail = (email ?? "").Trim().ToLower();
                var user = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.Email.ToLower() == normalizedEmail);
                if (user == null)
                {
                    return (false, "Không tìm thấy thông tin tài khoản tương ứng.");
                }

                if (!user.TrangThai)
                {
                    return (false, "Tài khoản của bạn hiện đang bị khóa.");
                }

                // Băm và lưu mật khẩu mới
                user.MatKhau = PasswordHelper.HashPassword(matKhauMoi);
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đặt lại mật khẩu cho email {Email}", email);
                return (false, "Không thể kết nối đến máy chủ CSDL hoặc xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }

        // 13. Khóa / Mở khóa tài khoản
        public async Task<(bool Success, string? ErrorMessage, bool NewStatus)> ToggleStatusAsync(int id, int currentUserId)
        {
            if (currentUserId == id)
            {
                return (false, "Bạn không thể tự khóa tài khoản Admin đang đăng nhập của chính mình!", false);
            }

            try
            {
                var user = await _context.TaiKhoans.FindAsync(id);
                if (user == null)
                {
                    return (false, "Không tìm thấy tài khoản.", false);
                }

                // Chặn khóa tài khoản Admin duy nhất còn lại đang hoạt động
                if (user.VaiTro == VaiTro.Admin && user.TrangThai)
                {
                    var activeAdminCount = await CountActiveAdminsAsync();
                    if (activeAdminCount <= 1)
                    {
                        return (false, "Không thể khóa tài khoản Quản trị viên (Admin) duy nhất còn lại đang hoạt động trong hệ thống!", false);
                    }
                }

                user.TrangThai = !user.TrangThai;

                // Đồng bộ trạng thái hồ sơ học viên tương ứng (nếu có)
                var hocVien = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == id);
                if (hocVien != null)
                {
                    hocVien.TrangThai = user.TrangThai;
                }

                await _context.SaveChangesAsync();
                return (true, null, user.TrangThai);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái tài khoản ID {Id}", id);
                return (false, "Không thể cập nhật trạng thái tài khoản do sự cố CSDL.", false);
            }
        }

        // 14. Admin đặt lại mật khẩu về mặc định
        public async Task<(bool Success, string? ErrorMessage)> ResetPasswordByAdminAsync(int id)
        {
            try
            {
                var user = await _context.TaiKhoans.FindAsync(id);
                if (user == null)
                {
                    return (false, "Không tìm thấy tài khoản.");
                }

                // Đặt lại về mật khẩu mặc định 123456
                const string defaultPassword = "123456";
                user.MatKhau = PasswordHelper.HashPassword(defaultPassword);
                await _context.SaveChangesAsync();

                return (true, $"Đã đặt lại mật khẩu cho '{user.TenDangNhap}' về mặc định: [{defaultPassword}].");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đặt lại mật khẩu cho tài khoản ID {Id}", id);
                return (false, "Không thể đặt lại mật khẩu do sự cố CSDL.");
            }
        }

        // 15. Admin cập nhật vai trò tài khoản
        public async Task<(bool Success, string? ErrorMessage)> UpdateRoleAsync(int id, string vaiTro, int currentUserId)
        {
            if (!VaiTro.DanhSachVaiTro.Contains(vaiTro))
            {
                return (false, "Vai trò được chọn không hợp lệ trong hệ thống.");
            }

            if (currentUserId == id && vaiTro != VaiTro.Admin)
            {
                return (false, "Bạn không thể tự hạ quyền Admin của tài khoản đang đăng nhập!");
            }

            try
            {
                var user = await _context.TaiKhoans.Include(t => t.HocVien).FirstOrDefaultAsync(t => t.MaTaiKhoan == id);
                if (user == null)
                {
                    return (false, "Không tìm thấy tài khoản.");
                }

                // Chặn hạ quyền Admin duy nhất còn lại trong hệ thống
                if (user.VaiTro == VaiTro.Admin && vaiTro != VaiTro.Admin)
                {
                    var activeAdminCount = await CountActiveAdminsAsync();
                    if (activeAdminCount <= 1)
                    {
                        return (false, "Không thể hạ quyền tài khoản Quản trị viên (Admin) duy nhất còn lại trong hệ thống!");
                    }
                }

                var oldRole = user.VaiTro;
                user.VaiTro = vaiTro;

                // Tự tạo hồ sơ học viên khi chuyển vai trò
                if (vaiTro == VaiTro.HocVien && user.HocVien == null)
                {
                    _context.HocViens.Add(new HocVien
                    {
                        MaTaiKhoan = user.MaTaiKhoan,
                        HoTen = user.HoTen,
                        Email = user.Email,
                        SoDienThoai = string.Empty,
                        NgayDangKy = DateTime.Now,
                        TrangThai = true
                    });
                }

                await _context.SaveChangesAsync();
                return (true, $"Đã cập nhật vai trò của tài khoản '{user.TenDangNhap}' từ [{oldRole}] sang [{vaiTro}] thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật vai trò cho tài khoản ID {Id}", id);
                return (false, "Không thể cập nhật vai trò tài khoản do sự cố CSDL.");
            }
        }

        // 16. Danh sách tài khoản có tìm kiếm, lọc, phân trang
        public async Task<(List<TaiKhoan> Items, int TotalCount, int TotalAdmins, int TotalNhanViens, int TotalHocViens, int TotalLocked, int TotalFiltered, int TotalPages)> GetListAsync(string? searchString, string? role, bool? trangThai, int page, int pageSize)
        {
            var query = _context.TaiKhoans.Include(t => t.HocVien).AsQueryable();

            // LINQ Tìm kiếm đa trường (Tên đăng nhập, Họ tên, Email)
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.Trim().ToLower();
                query = query.Where(t => t.TenDangNhap.ToLower().Contains(term)
                                      || t.HoTen.ToLower().Contains(term)
                                      || t.Email.ToLower().Contains(term));
            }

            // LINQ Lọc theo vai trò (Admin, NhanVien, HocVien)
            if (!string.IsNullOrWhiteSpace(role))
            {
                query = query.Where(t => t.VaiTro == role);
            }

            // LINQ Lọc theo trạng thái hoạt động / bị khóa
            if (trangThai.HasValue)
            {
                query = query.Where(t => t.TrangThai == trangThai.Value);
            }

            var totalCount = await _context.TaiKhoans.CountAsync();
            var totalAdmins = await _context.TaiKhoans.CountAsync(t => t.VaiTro == "Admin");
            var totalNhanViens = await _context.TaiKhoans.CountAsync(t => t.VaiTro == "NhanVien");
            var totalHocViens = await _context.TaiKhoans.CountAsync(t => t.VaiTro == "HocVien");
            var totalLocked = await _context.TaiKhoans.CountAsync(t => !t.TrangThai);

            var totalFiltered = await query.CountAsync();

            if (page < 1) page = 1;
            var totalPages = (int)Math.Ceiling(totalFiltered / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages) page = totalPages;

            var items = await query.OrderByDescending(t => t.MaTaiKhoan)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount, totalAdmins, totalNhanViens, totalHocViens, totalLocked, totalFiltered, totalPages);
        }

        // 17. Tự tạo hồ sơ học viên nếu chưa có (khi đăng nhập)
        public async Task<HocVien?> EnsureHocVienProfileAsync(TaiKhoan user)
        {
            if (user.VaiTro == VaiTro.HocVien && user.HocVien == null)
            {
                var newHocVien = new HocVien
                {
                    MaTaiKhoan = user.MaTaiKhoan,
                    HoTen = user.HoTen,
                    Email = user.Email,
                    SoDienThoai = string.Empty,
                    NgayDangKy = DateTime.Now,
                    TrangThai = true
                };
                _context.HocViens.Add(newHocVien);
                await _context.SaveChangesAsync();
                user.HocVien = newHocVien;
                return newHocVien;
            }
            return user.HocVien;
        }

        // 18. Lưu thay đổi
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // 19. Xuất danh sách tài khoản (theo điều kiện lọc)
        public async Task<List<TaiKhoan>> GetExportListAsync(string? searchString, string? role, bool? trangThai)
        {
            var query = _context.TaiKhoans.Include(t => t.HocVien).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.Trim().ToLower();
                query = query.Where(t => t.TenDangNhap.ToLower().Contains(term)
                                      || t.HoTen.ToLower().Contains(term)
                                      || t.Email.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                query = query.Where(t => t.VaiTro == role);
            }

            if (trangThai.HasValue)
            {
                query = query.Where(t => t.TrangThai == trangThai.Value);
            }

            return await query.OrderBy(t => t.MaTaiKhoan).ToListAsync();
        }
    }
}
