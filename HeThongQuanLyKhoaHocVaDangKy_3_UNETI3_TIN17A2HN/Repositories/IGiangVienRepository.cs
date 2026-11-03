// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Nội dung thực hiện: Module 1 - Interface Repository cho Quản lý Giảng viên (dữ liệu nền)

using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories
{
    public interface IGiangVienRepository
    {
        // 1. Danh sách giảng viên (tìm kiếm, lọc, phân trang) với stat cards
        Task<(List<GiangVien> Items, int TotalCount, int ActiveCount, int InactiveCount, int TotalKhoaHoc, int TotalFiltered, int TotalPages)> GetListAsync(string? searchString, bool? trangThai, bool? coKhoaHoc, int page, int pageSize);

        // 2. Xuất danh sách giảng viên (theo điều kiện lọc)
        Task<List<GiangVien>> GetExportListAsync(string? searchString, bool? trangThai, bool? coKhoaHoc);

        // 3. Lấy giảng viên theo Id
        Task<GiangVien?> GetByIdAsync(int id);

        // 4. Lấy giảng viên theo Id kèm danh sách khóa học
        Task<GiangVien?> GetByIdWithKhoaHocsAsync(int id);

        // 5. Kiểm tra trùng email giảng viên
        Task<bool> ExistsEmailAsync(string email, int? excludeId = null);

        // 6. Kiểm tra giảng viên có đang phụ trách khóa học
        Task<bool> HasAnyKhoaHocAsync(int maGiangVien);

        // 7. Thêm mới giảng viên
        Task<(bool Success, string? ErrorMessage)> CreateAsync(GiangVien giangVien);

        // 8. Cập nhật giảng viên
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, GiangVien giangVien);

        // 9. Xóa giảng viên
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);

        // 10. Đổi trạng thái nhanh Đang giảng dạy / Tạm nghỉ
        Task<(bool Success, string? ErrorMessage, bool NewStatus)> ToggleStatusAsync(int id);
    }
}
