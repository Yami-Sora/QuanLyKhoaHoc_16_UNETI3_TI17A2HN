// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Nội dung thực hiện: Module 1 - Entity Học viên (Phục vụ liên kết Tài khoản và Đăng ký học)

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models
{
    [Table("HocVien")]
    public class HocVien
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã học viên")]
        public int MaHocVien { get; set; }

        [Display(Name = "Mã tài khoản")]
        public int? MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Họ và tên học viên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ và tên tối đa 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; } = "Nam"; // Nam | Nữ | Khác

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "Địa chỉ liên lạc")]
        public string? DiaChi { get; set; }

        [StringLength(100)]
        [Display(Name = "Trình độ học vấn")]
        public string? TrinhDo { get; set; }

        [Display(Name = "Ngày tham gia")]
        public DateTime NgayDangKy { get; set; } = DateTime.Now;

        [Display(Name = "Trạng thái hồ sơ")]
        public bool TrangThai { get; set; } = true; // true: Hoạt động, false: Khóa/Nghỉ

        [Display(Name = "Ghi chú thêm")]
        public string? GhiChu { get; set; }

        // Navigation Properties (Module 3)
        [ForeignKey("MaTaiKhoan")]
        public virtual TaiKhoan? TaiKhoan { get; set; }

        public virtual ICollection<DangKyKhoaHoc> DangKyKhoaHocs { get; set; } = new List<DangKyKhoaHoc>();
    }
}
