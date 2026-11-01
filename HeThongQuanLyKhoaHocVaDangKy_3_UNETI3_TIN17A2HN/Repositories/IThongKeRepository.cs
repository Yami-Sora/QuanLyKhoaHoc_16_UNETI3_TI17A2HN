// Họ và tên: Nguyễn Công Thành
// Mã sinh viên: 23103100102
// Nội dung thực hiện: Module 5 - Interface Repository cho Thống kê Báo cáo LINQ tổng hợp

using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories
{
    public interface IThongKeRepository
    {
        // 1. Thống kê tổng hợp toàn diện (8 sections báo cáo LINQ)
        Task<ThongKeViewModel> GetThongKeAsync();

        // 2. Lấy dữ liệu đăng ký theo tháng phục vụ biểu đồ (3 chế độ: 12months, year, all)
        Task<List<ThongKeDangKyTheoThangViewModel>> GetDangKyTheoThangAsync(string filterType, int? year);
    }
}
