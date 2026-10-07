// Họ và tên: Ngọc Tùng Lâm
// Mã sinh viên: 23103100098
// Nội dung thực hiện: Module 3 - ViewModel Thêm mới và Chỉnh sửa Hồ sơ Học viên

using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    public class HocVienFormViewModel
    {
        public int? MaHocVien { get; set; }

        public int? MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Họ và tên học viên không được để trống")]
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
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Địa chỉ tối đa 200 ký tự")]
        [Display(Name = "Địa chỉ liên lạc")]
        public string? DiaChi { get; set; }

        [StringLength(100, ErrorMessage = "Trình độ học vấn tối đa 100 ký tự")]
        [Display(Name = "Trình độ học vấn")]
        public string? TrinhDo { get; set; } = "Đại học";

        [Display(Name = "Trạng thái hồ sơ")]
        public bool TrangThai { get; set; } = true;

        [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
        [Display(Name = "Ghi chú thêm")]
        public string? GhiChu { get; set; }

        [StringLength(255)]
        [Display(Name = "Đường dẫn ảnh đại diện")]
        public string? HinhAnh { get; set; }

        [Display(Name = "Tải lên ảnh đại diện")]
        public IFormFile? FileAnh { get; set; }

        // Tùy chọn khi Thêm mới học viên
        [Display(Name = "Tự động tạo tài khoản đăng nhập")]
        public bool TaoTaiKhoan { get; set; } = true;

        [Display(Name = "Tên đăng nhập (nếu tạo)")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3 đến 50 ký tự")]
        public string? TenDangNhap { get; set; }

        [Display(Name = "Mật khẩu ban đầu")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên")]
        [DataType(DataType.Password)]
        public string? MatKhau { get; set; } = "User@123";
    }
}
