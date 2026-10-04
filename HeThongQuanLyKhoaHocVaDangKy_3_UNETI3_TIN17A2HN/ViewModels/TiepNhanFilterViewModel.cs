// Họ và tên: Bạch Đức Sự
// Mã sinh viên: 23103100063
// Nội dung thực hiện: Module 4 - Tiếp nhận đăng ký, Xác nhận/Hủy đăng ký và Quản lý trạng thái.

using System.ComponentModel.DataAnnotations;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    public class TiepNhanFilterViewModel
    {
        // 1. Dữ liệu danh sách hiển thị
        public List<DangKyKhoaHoc> DanhSachDangKy { get; set; } = new();

        // 2. Thống kê Stat Cards tổng quan
        public int TongSoDon { get; set; }
        public int SoDonChoXuLy { get; set; }
        public int SoDonDangXuLy { get; set; }
        public int SoDonHoanThanh { get; set; }
        public int SoDonBiHuy { get; set; }
        public decimal TongDoanhThuDaThu { get; set; }
        public decimal TongCongNoConLai { get; set; }

        // 3. Tiêu chí Tìm kiếm & Lọc
        [Display(Name = "Từ khóa tìm kiếm")]
        public string? SearchString { get; set; }

        [Display(Name = "Tên học viên")]
        public string? SearchHocVien { get; set; }

        [Display(Name = "Tên khóa học")]
        public string? SearchKhoaHoc { get; set; }

        [Display(Name = "Trạng thái đơn")]
        public string? TrangThai { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Từ ngày đăng ký")]
        public DateTime? TuNgay { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Đến ngày đăng ký")]
        public DateTime? DenNgay { get; set; }

        [Display(Name = "Sắp xếp theo")]
        public string? SortBy { get; set; } = "ngay_desc";

        // 4. Phân trang chuẩn LINQ
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / (PageSize > 0 ? PageSize : 10));
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }

    public class TiepNhanDangKyListViewModel : TiepNhanFilterViewModel
    {
    }
}
