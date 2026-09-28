// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Nội dung thực hiện: Module 1 - Entity Giảng viên (Phục vụ liên kết Khóa học)

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models
{
    [Table("GiangVien")]
    public class GiangVien
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã giảng viên")]
        public int MaGiangVien { get; set; }

        [Required(ErrorMessage = "Họ và tên giảng viên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ và tên tối đa 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Chuyên môn")]
        public string? ChuyenMon { get; set; }

        [StringLength(50)]
        [Display(Name = "Học vị")]
        public string? HocVi { get; set; } // Thạc sĩ, Tiến sĩ, Giáo sư...

        [Display(Name = "Trạng thái công tác")]
        public bool TrangThai { get; set; } = true; // true: Đang giảng dạy, false: Tạm nghỉ

        // Navigation Property: 1 Giảng viên phụ trách N Khóa học (Module 2)
        public virtual ICollection<KhoaHoc> KhoaHocs { get; set; } = new List<KhoaHoc>();
    }
}
