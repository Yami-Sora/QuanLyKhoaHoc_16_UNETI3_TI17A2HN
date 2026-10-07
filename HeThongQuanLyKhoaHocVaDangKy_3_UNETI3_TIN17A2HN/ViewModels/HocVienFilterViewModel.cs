// Họ và tên: Ngọc Tùng Lâm
// Mã sinh viên: 23103100098
// Nội dung thực hiện: Module 3 - ViewModel Bộ lọc, Tìm kiếm, Sắp xếp và Phân trang Quản lý Học viên

using System.ComponentModel.DataAnnotations;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    public class HocVienItemViewModel
    {
        public int MaHocVien { get; set; }
        public int? MaTaiKhoan { get; set; }
        public string? TenDangNhap { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public DateTime? NgaySinh { get; set; }
        public string? GioiTinh { get; set; }
        public string SoDienThoai { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? DiaChi { get; set; }
        public string? TrinhDo { get; set; }
        public DateTime NgayDangKy { get; set; }
        public bool TrangThai { get; set; }
        public string? GhiChu { get; set; }
        public int SoKhoaHocDaDangKy { get; set; }
        public int SoKhoaHocHoanThanh { get; set; }
        public decimal TongHocPhiDaDong { get; set; }
        public decimal TongHocPhiConLai { get; set; }
    }

    public class HocVienFilterViewModel
    {
        // 1. Danh sách học viên hiển thị trên trang hiện tại
        public List<HocVienItemViewModel> DanhSachHocVien { get; set; } = new();

        // 2. Thống kê Stat Cards
        public int TongSoHocVien { get; set; }
        public int SoHocVienHoatDong { get; set; }
        public int SoHocVienBiKhoa { get; set; }
        public int SoHocVienCoDangKy { get; set; }

        // 3. Tiêu chí Tìm kiếm & Lọc
        [Display(Name = "Từ khóa tìm kiếm")]
        public string? SearchString { get; set; }

        [Display(Name = "Trạng thái hồ sơ")]
        public string? TrangThai { get; set; } // "TatCa", "HoatDong", "BiKhoa"

        [Display(Name = "Trình độ")]
        public string? TrinhDo { get; set; }

        [Display(Name = "Sắp xếp theo")]
        public string? SortBy { get; set; } = "ngay_desc"; // ten_asc, ten_desc, ngay_asc, ngay_desc

        // 4. Phân trang theo chuẩn LINQ
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / (PageSize > 0 ? PageSize : 10));
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
