// Họ và tên: Nguyễn Văn Mạnh
// Mã sinh viên: 23103100096
// Nội dung thực hiện: Module 2 - Interface Repository cho Quản lý Khóa học

using Microsoft.AspNetCore.Mvc.Rendering;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories
{
    public interface IKhoaHocRepository
    {
        // 1. Đồng bộ vòng đời khóa học theo ngày thực tế
        Task DongBoVongDoiKhoaHocAsync();

        // 2. Tra cứu, lọc, sắp xếp và phân trang khóa học (công khai)
        Task<(KhoaHocFilterViewModel Filter, SelectList MonHocList)> GetFilterViewModelAsync(KhoaHocFilterViewModel filter, int? page = null);

        // 3. Danh sách quản trị khóa học (tìm kiếm, lọc, phân trang) với stat cards
        Task<(List<KhoaHoc> Items, int TotalCount, int DangMoCount, int DangHocCount, int DaKetThucCount, long AvgHocPhi, int TotalFiltered, int TotalPages, SelectList MonHocList)> GetQuanLyDataAsync(
            string? searchString, int? maMonHoc, string? trangThai, string? hinhThuc,
            string? khoangHocPhi, string? tinhTrangCho, string? sortBy, int page, int pageSize);

        // 4. Chi tiết khóa học kèm môn học, giảng viên và danh sách đăng ký
        Task<KhoaHoc?> GetByIdWithDetailsAsync(int id);

        // 5. Kiểm tra học viên đã đăng ký khóa học chưa
        Task<bool> CheckHocVienDaDangKyAsync(int maKhoaHoc, int maTaiKhoan);

        // 6. Lấy khóa học theo Id
        Task<KhoaHoc?> GetByIdAsync(int id);

        // 7. Lấy dữ liệu dropdown môn học và giảng viên
        Task<(SelectList MonHocList, SelectList GiangVienList)> GetDropdownDataAsync(int? selectedMonHoc = null, int? selectedGiangVien = null);

        // 8. Thêm mới khóa học
        Task<(bool Success, string? ErrorMessage, int MaKhoaHoc)> CreateKhoaHocAsync(KhoaHoc khoaHoc);

        // 9. Cập nhật thông tin khóa học
        Task<(bool Success, string? ErrorMessage)> UpdateKhoaHocAsync(KhoaHoc khoaHoc);

        // 10. Lấy thông tin khóa học để xác nhận xóa
        Task<KhoaHoc?> GetByIdForDeleteAsync(int id);

        // 11. Kiểm tra khóa học đã có đăng ký chưa
        Task<bool> HasDangKyAsync(int id);

        // 12. Xóa khóa học an toàn
        Task<(bool Success, string? ErrorMessage)> DeleteKhoaHocAsync(int id);

        // 13. Cập nhật nhanh trạng thái khóa học
        Task<(bool Success, string? ErrorMessage, string StatusLabel)> UpdateStatusAsync(int id, string newStatus);

        // 14. Định dạng nhãn trạng thái tiếng Việt
        string GetStatusLabel(string? trangThai);
    }
}
