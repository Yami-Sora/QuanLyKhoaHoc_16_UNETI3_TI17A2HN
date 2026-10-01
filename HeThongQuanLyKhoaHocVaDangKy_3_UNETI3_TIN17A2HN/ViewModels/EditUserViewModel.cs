// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Module 1: ViewModel sửa tài khoản (Admin)

using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    public class EditUserViewModel
    {
        public int MaTaiKhoan { get; set; }

        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Họ và tên phải từ 2 đến 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng")]
        [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
        [Display(Name = "Email liên hệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn vai trò")]
        [Display(Name = "Vai trò hệ thống")]
        public string VaiTro { get; set; } = "HocVien";

        [Display(Name = "Số điện thoại")]
        [StringLength(15, ErrorMessage = "Số điện thoại không vượt quá 15 ký tự")]
        [RegularExpression(@"^$|^0[35789][0-9]{8}$", ErrorMessage = "Số điện thoại phải là số di động Việt Nam hợp lệ (10 số, bắt đầu bằng 03, 05, 07, 08, 09)")]
        public string? SoDienThoai { get; set; }

        [Display(Name = "Trạng thái hoạt động")]
        public bool TrangThai { get; set; } = true;
    }
}
