// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Nội dung thực hiện: Module 1 - Interface Repository cho Tài khoản, Đăng nhập & Phân quyền

using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories
{
    public interface ITaiKhoanRepository
    {
        // 1. Đăng nhập: tìm tài khoản theo tên đăng nhập (kèm HocVien)
        Task<TaiKhoan?> GetByTenDangNhapAsync(string tenDangNhap);

        // 2. Tìm tài khoản theo Email
        Task<TaiKhoan?> GetByEmailAsync(string email);

        // 3. Tìm tài khoản theo Id
        Task<TaiKhoan?> GetByIdAsync(int id);

        // 4. Tìm tài khoản theo Id kèm HocVien
        Task<TaiKhoan?> GetByIdWithHocVienAsync(int id);

        // 5. Kiểm tra trùng tên đăng nhập
        Task<bool> ExistsTenDangNhapAsync(string tenDangNhap, int? excludeId = null);

        // 6. Kiểm tra trùng Email
        Task<bool> ExistsEmailAsync(string email, int? excludeId = null);

        // 7. Đếm số Admin đang hoạt động
        Task<int> CountActiveAdminsAsync();

        // 8. Đăng ký tài khoản học viên mới (kèm tạo hồ sơ HocVien)
        Task<(bool Success, string? ErrorMessage)> RegisterAsync(RegisterViewModel model);

        // 9. Tạo tài khoản mới bởi Admin (kèm hồ sơ HocVien nếu vai trò là HocVien)
        Task<(bool Success, string? ErrorMessage)> CreateByAdminAsync(CreateUserViewModel model);

        // 10. Cập nhật thông tin tài khoản bởi Admin
        Task<(bool Success, string? ErrorMessage)> UpdateByAdminAsync(int id, EditUserViewModel model);

        // 11. Đổi mật khẩu
        Task<(bool Success, string? ErrorMessage)> ChangePasswordAsync(int maTaiKhoan, string matKhauHienTai, string matKhauMoi);

        // 12. Đặt lại mật khẩu (sau khi xác thực token)
        Task<(bool Success, string? ErrorMessage)> ResetPasswordAsync(string email, string matKhauMoi);

        // 13. Khóa / Mở khóa tài khoản
        Task<(bool Success, string? ErrorMessage, bool NewStatus)> ToggleStatusAsync(int id, int currentUserId);

        // 14. Admin đặt lại mật khẩu về mặc định
        Task<(bool Success, string? ErrorMessage)> ResetPasswordByAdminAsync(int id);

        // 15. Admin cập nhật vai trò tài khoản
        Task<(bool Success, string? ErrorMessage)> UpdateRoleAsync(int id, string vaiTro, int currentUserId);

        // 16. Danh sách tài khoản có tìm kiếm, lọc, phân trang
        Task<(List<TaiKhoan> Items, int TotalCount, int TotalAdmins, int TotalNhanViens, int TotalHocViens, int TotalLocked, int TotalFiltered, int TotalPages)> GetListAsync(string? searchString, string? role, bool? trangThai, int page, int pageSize);

        // 17. Tự tạo hồ sơ học viên nếu chưa có (khi đăng nhập)
        Task<HocVien?> EnsureHocVienProfileAsync(TaiKhoan user);

        // 18. Lưu thay đổi
        Task SaveChangesAsync();
    }
}
