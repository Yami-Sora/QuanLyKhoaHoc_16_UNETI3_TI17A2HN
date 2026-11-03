// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Nội dung thực hiện: Module 1 - Interface Repository cho Quản lý Môn học

using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories
{
    public interface IMonHocRepository
    {
        // 1. Danh sách môn học công khai (tìm kiếm, lọc, sắp xếp, phân trang)
        Task<(List<MonHoc> Items, int TotalCount, int TotalFiltered, int TotalPages)> GetPublicListAsync(string? searchString, int? soTinChi, bool? trangThai, string? sortBy, int page, int pageSize);

        // 2. Chi tiết môn học kèm các khóa học liên quan
        Task<MonHoc?> GetDetailsAsync(int id);

        // 3. Danh sách khóa học mà học viên đã đăng ký (theo mã tài khoản)
        Task<List<int>> GetRegisteredKhoaHocIdsAsync(int maTaiKhoan);

        // 4. Danh sách quản trị (tìm kiếm, lọc, phân trang) với stat cards
        Task<(List<MonHoc> Items, int TotalCount, int ActiveCount, int InactiveCount, int TotalTinChi, int AvgHocPhi, int TotalFiltered, int TotalPages)> GetAdminListAsync(string? searchString, bool? trangThai, int page, int pageSize);

        // 5. Lấy môn học theo Id
        Task<MonHoc?> GetByIdAsync(int id);

        // 6. Lấy môn học theo Id kèm danh sách khóa học
        Task<MonHoc?> GetByIdWithKhoaHocsAsync(int id);

        // 7. Kiểm tra trùng tên môn học
        Task<bool> ExistsTenMonHocAsync(string tenMonHoc, int? excludeId = null);

        // 8. Kiểm tra môn học có khóa học đang hoạt động
        Task<bool> HasActiveKhoaHocAsync(int maMonHoc);

        // 9. Kiểm tra môn học có khóa học tham chiếu
        Task<bool> HasAnyKhoaHocAsync(int maMonHoc);

        // 10. Thêm mới môn học
        Task<(bool Success, string? ErrorMessage)> CreateAsync(MonHoc monHoc);

        // 11. Cập nhật môn học
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, MonHoc monHoc);

        // 12. Xóa môn học
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);

        // 13. Đổi trạng thái nhanh Đang mở / Tạm dừng
        Task<(bool Success, string? ErrorMessage, bool NewStatus)> ToggleStatusAsync(int id);
    }
}
