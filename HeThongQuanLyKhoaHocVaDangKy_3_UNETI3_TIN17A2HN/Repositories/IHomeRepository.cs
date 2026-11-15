// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Nội dung thực hiện: Module 1 - Interface Repository cho Trang chủ (Home)

using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories
{
    public interface IHomeRepository
    {
        // Lấy toàn bộ dữ liệu thống kê và danh sách môn học tiêu biểu cho trang chủ
        Task<HomeStatsViewModel> GetHomeDataAsync();
    }
}
