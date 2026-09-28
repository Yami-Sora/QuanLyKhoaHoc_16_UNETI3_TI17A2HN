// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Nội dung thực hiện: Module 1 - ViewModel phục vụ Form Đăng nhập

using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = string.Empty;

        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool GhiNho { get; set; } = false;

        public string? ReturnUrl { get; set; }
    }
}
