// Họ và tên: Ngọc Tùng Lâm
// Mã sinh viên: 23103100098
// Nội dung thực hiện: Module 3 - Interface Repository cho Đăng ký Khóa học & Theo dõi Đăng ký

using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories
{
    public interface IDangKyKhoaHocRepository
    {
        // 1. Lấy thông tin khởi tạo form đăng ký khóa học
        Task<(DangKyKhoaHocViewModel? ViewModel, string? ErrorMessage)> GetDangKyFormAsync(int maKhoaHoc, int maTaiKhoan);

        // 2. Xử lý lưu đăng ký khóa học
        Task<(bool Success, string? ErrorMessage, int MaDangKy)> XuLyDangKyAsync(DangKyKhoaHocViewModel model, int maTaiKhoan);

        // 3. Lọc & phân trang lịch sử đăng ký của học viên
        Task<TheoDoiDangKyFilterViewModel> GetTheoDoiFilterAsync(int maTaiKhoan, TheoDoiDangKyFilterViewModel filter);

        // 4. Lấy chi tiết đơn đăng ký (có kiểm tra quyền chính chủ)
        Task<(DangKyKhoaHoc? DangKy, string? ErrorMessage)> GetChiTietDangKyAsync(int id, int maTaiKhoan, bool isStaffOrAdmin);

        // 5. Hủy đơn đăng ký
        Task<(bool Success, string? ErrorMessage)> HuyDangKyAsync(int id, string? lyDoHuy, int maTaiKhoan, bool isStaffOrAdmin);

        // 6. Lấy thông tin in phiếu đăng ký
        Task<(DangKyKhoaHoc? DangKy, string? ErrorMessage)> GetPhieuDangKyAsync(int id, int maTaiKhoan, bool isStaffOrAdmin);
    }
}
