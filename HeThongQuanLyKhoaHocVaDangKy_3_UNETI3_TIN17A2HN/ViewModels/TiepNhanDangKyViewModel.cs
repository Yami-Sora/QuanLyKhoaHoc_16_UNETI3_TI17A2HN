// Họ và tên: Bạch Đức Sự
// Mã sinh viên: 23103100063
// Nội dung thực hiện: Module 4 - Tiếp nhận đăng ký, Xác nhận/Hủy đăng ký và Quản lý trạng thái.

using System.ComponentModel.DataAnnotations;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    // Họ và tên: Bạch Đức Sự
    // Mã sinh viên: 23103100063
    // Nội dung thực hiện: Module 4 - ViewModel quản lý Danh sách Tiếp nhận Đăng ký, Bộ lọc và Phân trang
    public class TiepNhanDangKyListViewModel
    {
        // 1. Dữ liệu danh sách hiển thị
        public List<DangKyKhoaHoc> DanhSachDangKy { get; set; } = new();

        // 2. Thống kê Stat Cards
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
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }

    // Họ và tên: Bạch Đức Sự
    // Mã sinh viên: 23103100063
    // Nội dung thực hiện: Module 4 - ViewModel Cập nhật Trạng thái & Đối soát Học phí
    public class CapNhatTrangThaiViewModel
    {
        public int MaDangKy { get; set; }

        [Display(Name = "Học viên")]
        public string HoTenHocVien { get; set; } = string.Empty;

        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Khóa học")]
        public string TenKhoaHoc { get; set; } = string.Empty;

        [Display(Name = "Trạng thái khóa học")]
        public string TrangThaiKhoaHoc { get; set; } = string.Empty;

        [Display(Name = "Học phí khóa học")]
        public decimal HocPhi { get; set; }

        [Display(Name = "Sĩ số hiện tại")]
        public int SiSoHienTai { get; set; }

        [Display(Name = "Sĩ số tối đa")]
        public int SoLuongToiDa { get; set; }

        [Display(Name = "Ngày đăng ký")]
        public DateTime NgayDangKy { get; set; }

        [Display(Name = "Trạng thái hiện tại")]
        public string TrangThaiHienTai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn trạng thái mới cần chuyển")]
        [Display(Name = "Trạng thái mới")]
        public string TrangThaiMoi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số tiền đã đóng")]
        [Range(0, 100000000, ErrorMessage = "Số tiền đã đóng phải >= 0 và <= 100.000.000 đ")]
        [Display(Name = "Số tiền đã nộp (VNĐ)")]
        public decimal SoTienDaDong { get; set; }

        [Display(Name = "Số tiền còn lại (VNĐ)")]
        public decimal SoTienConLai { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
        [Display(Name = "Ghi chú giao dịch / Lý do chuyển trạng thái")]
        public string? GhiChu { get; set; }

        // Danh sách trạng thái hợp lệ có thể chuyển đến từ trạng thái hiện tại
        public List<string> TrangThaiHopLe { get; set; } = new();
    }

    // Họ và tên: Bạch Đức Sự
    // Mã sinh viên: 23103100063
    // Nội dung thực hiện: Module 4 - ViewModel Đăng ký Khóa học Mới trực tiếp tại quầy
    public class TaoDangKyMoiViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn học viên")]
        [Display(Name = "Học viên")]
        public int MaHocVien { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn khóa học")]
        [Display(Name = "Khóa học đăng ký")]
        public int MaKhoaHoc { get; set; }

        [Display(Name = "Trạng thái ban đầu")]
        public string TrangThai { get; set; } = "ChoXuLy"; // ChoXuLy | DangXuLy

        [Range(0, 100000000, ErrorMessage = "Số tiền đóng phải >= 0")]
        [Display(Name = "Số tiền nộp đợt đầu (VNĐ)")]
        public decimal SoTienDaDong { get; set; } = 0;

        [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
        [Display(Name = "Ghi chú tiếp nhận đơn")]
        public string? GhiChu { get; set; }
    }
}
