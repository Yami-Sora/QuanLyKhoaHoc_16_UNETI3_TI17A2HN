// Họ và tên: Ngọc Tùng Lâm
// Mã sinh viên: 23103100098
// Nội dung thực hiện: Module 3 - Interface Repository cho Quản lý Học viên & Hồ sơ cá nhân

using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories
{
    public interface IHocVienRepository
    {
        // 1. Lọc, tìm kiếm và phân trang danh sách học viên
        Task<HocVienFilterViewModel> GetFilterViewModelAsync(HocVienFilterViewModel filter);

        // 2. Lấy chi tiết hồ sơ học viên kèm lịch sử học tập
        Task<HocVienDetailsViewModel?> GetDetailsViewModelAsync(int id);

        // 3. Lấy thực thể học viên theo Id
        Task<HocVien?> GetByIdAsync(int id);

        // 4. Lấy thực thể kèm thông tin tài khoản để chỉnh sửa
        Task<HocVien?> GetByIdWithTaiKhoanAsync(int id);

        // 5. Kiểm tra trùng lặp thông tin
        Task<bool> ExistsEmailAsync(string email, int? excludeId = null);
        Task<bool> ExistsSoDienThoaiAsync(string soDienThoai, int? excludeId = null);
        Task<bool> ExistsTenDangNhapAsync(string tenDangNhap, int? excludeTaiKhoanId = null);

        // 6. Thêm mới học viên (kèm tài khoản nếu chọn)
        Task<(bool Success, string? ErrorMessage, int MaHocVien)> CreateHocVienAsync(HocVienFormViewModel model, string? hinhAnhPath);

        // 7. Cập nhật thông tin học viên & mật khẩu
        Task<(bool Success, string? ErrorMessage)> UpdateHocVienAsync(int id, HocVienFormViewModel model, string? hinhAnhPath);

        // 8. Xóa học viên (có kiểm tra ràng buộc khóa học)
        Task<(bool Success, string? ErrorMessage)> DeleteHocVienAsync(int id);

        // 9. Đổi trạng thái hoạt động / khóa hồ sơ
        Task<(bool Success, string? ErrorMessage, bool NewStatus)> ToggleTrangThaiAsync(int id);

        // 10. Hồ sơ cá nhân của học viên đang đăng nhập
        Task<HoSoCaNhanViewModel?> GetHoSoCaNhanAsync(int maTaiKhoan);

        // 11. Cập nhật hồ sơ cá nhân
        Task<(bool Success, string? ErrorMessage)> CapNhatHoSoAsync(int maTaiKhoan, HoSoCaNhanViewModel model, string? hinhAnhPath);
    }
}
