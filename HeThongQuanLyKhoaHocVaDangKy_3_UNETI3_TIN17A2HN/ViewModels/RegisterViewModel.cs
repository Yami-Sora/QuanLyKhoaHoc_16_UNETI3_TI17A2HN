// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Nội dung thực hiện: Module 1 - ViewModel phục vụ Form Đăng ký tài khoản học viên

using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Họ và tên phải từ 2 đến 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
        [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập từ 3 đến 50 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9_.]+$", ErrorMessage = "Tên đăng nhập chỉ chứa chữ cái không dấu, số, dấu gạch dưới (_) và dấu chấm (.)")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("MatKhau", ErrorMessage = "Mật khẩu xác nhận không trùng khớp")]
        [Display(Name = "Nhập lại mật khẩu")]
        public string XacNhanMatKhau { get; set; } = string.Empty;

        [Display(Name = "Số điện thoại")]
        [StringLength(15, ErrorMessage = "Số điện thoại không vượt quá 15 ký tự")]
        [RegularExpression(@"^$|^(0[3|5|7|8|9])[0-9]{8}$", ErrorMessage = "Số điện thoại phải là số di động Việt Nam hợp lệ (10 số, bắt đầu bằng 03, 05, 07, 08, 09)")]
        public string? SoDienThoai { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Vui lòng tích chọn đồng ý với quy chế đào tạo và điều khoản")]
        [Display(Name = "Đồng ý điều khoản")]
        public bool DongYDieuKhoan { get; set; } = true;
    }
}
