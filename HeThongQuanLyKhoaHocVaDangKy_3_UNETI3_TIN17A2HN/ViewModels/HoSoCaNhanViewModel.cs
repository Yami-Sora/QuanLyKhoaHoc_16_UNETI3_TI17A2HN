// Họ và tên: Ngọc Tùng Lâm
// Mã sinh viên: 23103100098
// Nội dung thực hiện: Module 3 - ViewModel Hồ sơ cá nhân của Học viên

using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    public class HoSoCaNhanViewModel
    {
        public int MaHocVien { get; set; }
        public int? MaTaiKhoan { get; set; }

        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ và tên tối đa 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [Display(Name = "Giới tính")]
        [StringLength(10)]
        public string? GioiTinh { get; set; } = "Nam";

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải gồm 10 chữ số và bắt đầu bằng số 0")]
        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
        [Display(Name = "Địa chỉ Email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Địa chỉ tối đa 200 ký tự")]
        [Display(Name = "Địa chỉ liên lạc")]
        public string? DiaChi { get; set; }

        [StringLength(100, ErrorMessage = "Trình độ tối đa 100 ký tự")]
        [Display(Name = "Trình độ học vấn")]
        public string? TrinhDo { get; set; } = "Đại học";

        [Display(Name = "Ngày tham gia hệ thống")]
        public DateTime NgayDangKy { get; set; }

        [Display(Name = "Trạng thái hồ sơ")]
        public bool TrangThai { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
        [Display(Name = "Giới thiệu bản thân / Ghi chú")]
        public string? GhiChu { get; set; }

        [StringLength(255)]
        [Display(Name = "Đường dẫn ảnh đại diện")]
        public string? HinhAnh { get; set; }

        [Display(Name = "Tải lên ảnh đại diện")]
        public IFormFile? FileAnh { get; set; }

        // Thống kê cá nhân
        public int TongKhoaHocDaDangKy { get; set; }
        public int SoKhoaHocHoanThanh { get; set; }
        public int SoKhoaHocChoXuLy { get; set; }
        public decimal TongHocPhiDaDong { get; set; }
        public decimal TongHocPhiConLai { get; set; }
    }
}
