// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Module 1: ViewModel quên mật khẩu

using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email đã đăng ký")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
        [Display(Name = "Địa chỉ email")]
        public string Email { get; set; } = string.Empty;
    }
}
