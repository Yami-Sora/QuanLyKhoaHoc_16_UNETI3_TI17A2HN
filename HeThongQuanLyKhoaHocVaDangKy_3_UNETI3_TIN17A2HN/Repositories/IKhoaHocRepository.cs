// Họ và tên: Nguyễn Văn Mạnh
// Mã sinh viên: 23103100096
// Nội dung thực hiện: Module 2 - Interface repository cho quản lý khóa học

using Microsoft.AspNetCore.Mvc.Rendering;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories
{
    /// <summary>
    /// Kết quả truy vấn trang quản trị khóa học
    /// </summary>
    public class KhoaHocQuanLyResult
    {
        public List<KhoaHoc> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int DangMoCount { get; set; }
        public int DangHocCount { get; set; }
        public int DaKetThucCount { get; set; }
        public long AvgHocPhi { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public SelectList MonHocList { get; set; } = null!;
    }

    public interface IKhoaHocRepository
    {
        /// <summary>
        /// Tự động đồng bộ vòng đời khóa học theo thời gian thực
        /// </summary>
        Task DongBoVongDoiKhoaHocAsync();

        /// <summary>
        /// Cổng tra cứu khóa học công khai
        /// </summary>
        Task<(KhoaHocFilterViewModel Filter, SelectList MonHocList)> GetFilterViewModelAsync(KhoaHocFilterViewModel filter, int? page = null);

        /// <summary>
        /// Quản trị khóa học
        /// </summary>
        Task<KhoaHocQuanLyResult> GetQuanLyDataAsync(
            string? searchString,
            int? maMonHoc,
            string? trangThai,
            string? hinhThuc,
            string? khoangHocPhi,
            string? tinhTrangCho,
            string? sortBy,
            int page,
            int pageSize);

        /// <summary>
        /// Lấy chi tiết thông tin khóa học
        /// </summary>
        Task<KhoaHoc?> GetByIdWithDetailsAsync(int id);

        /// <summary>
        /// Kiểm tra học viên đã đăng ký khóa học hay chưa
        /// </summary>
        Task<bool> CheckHocVienDaDangKyAsync(int maKhoaHoc, int maTaiKhoan);

        /// <summary>
        /// Lấy thông tin khóa học theo mã
        /// </summary>
        Task<KhoaHoc?> GetByIdAsync(int id);

        /// <summary>
        /// Lấy dữ liệu dropdown cho môn học và giảng viên
        /// </summary>
        Task<(SelectList MonHocList, SelectList GiangVienList)> GetDropdownDataAsync(int? selectedMonHoc = null, int? selectedGiangVien = null);

        /// <summary>
        /// Thêm mới khóa học
        /// </summary>
        Task<(bool Success, string? ErrorMessage, int MaKhoaHoc)> CreateKhoaHocAsync(KhoaHoc khoaHoc);

        /// <summary>
        /// Cập nhật thông tin khóa học
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> UpdateKhoaHocAsync(KhoaHoc khoaHoc);

        /// <summary>
        /// Lấy thông tin khóa học trước khi xóa
        /// </summary>
        Task<KhoaHoc?> GetByIdForDeleteAsync(int id);

        /// <summary>
        /// Kiểm tra khóa học đã phát sinh lịch sử đăng ký chưa
        /// </summary>
        Task<bool> HasDangKyAsync(int id);

        /// <summary>
        /// Xóa khóa học an toàn
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> DeleteKhoaHocAsync(int id);

        /// <summary>
        /// Cập nhật trạng thái khóa học
        /// </summary>
        Task<(bool Success, string? ErrorMessage, string StatusLabel)> UpdateStatusAsync(int id, string newStatus);

        /// <summary>
        /// Lấy nhãn trạng thái tiếng Việt
        /// </summary>
        string GetStatusLabel(string? trangThai);
    }
}
