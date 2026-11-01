// Họ và tên: Nguyễn Công Thành
// Mã sinh viên: 23103100102
// Nội dung thực hiện: Module 5 - Interface Repository cho Dashboard quản trị LINQ

using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories
{
    public interface IDashboardRepository
    {
        // Lấy toàn bộ dữ liệu thống kê Dashboard
        Task<(DashboardViewModel Model, int MonHocDangMo, int TongTaiKhoan, int TaiKhoanHoatDong)> GetDashboardAsync();
    }
}
