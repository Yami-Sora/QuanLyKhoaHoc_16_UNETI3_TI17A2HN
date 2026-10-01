// Họ và tên: Nguyễn Văn Mạnh
// Mã sinh viên: 23103100096
// Nội dung thực hiện: Module 2 - Entity Khóa học (Phục vụ liên kết Môn học và Đăng ký học)

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models
{
    [Table("KhoaHoc")]
    public class KhoaHoc
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã khóa học")]
        public int MaKhoaHoc { get; set; }

        [Required(ErrorMessage = "Tên khóa học không được để trống")]
        [StringLength(150, ErrorMessage = "Tên khóa học tối đa 150 ký tự")]
        [Display(Name = "Tên khóa học")]
        public string TenKhoaHoc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn môn học")]
        [Display(Name = "Môn học")]
        public int MaMonHoc { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giảng viên phụ trách")]
        [Display(Name = "Giảng viên")]
        public int MaGiangVien { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày bắt đầu")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime NgayBatDau { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày kết thúc")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime NgayKetThuc { get; set; }

        [Required(ErrorMessage = "Số lượng tối đa không được để trống")]
        [Range(1, 500, ErrorMessage = "Số lượng học viên tối đa phải từ 1 đến 500")]
        [Display(Name = "Sĩ số tối đa")]
        public int SoLuongToiDa { get; set; } = 30;

        [Required(ErrorMessage = "Học phí khóa học không được để trống")]
        [Range(0, 1000000000, ErrorMessage = "Học phí phải lớn hơn hoặc bằng 0 đ")]
        [Display(Name = "Học phí thực tế")]
        [DisplayFormat(DataFormatString = "{0:N0} đ", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal HocPhi { get; set; }

        [Required(ErrorMessage = "Hình thức đào tạo không được để trống")]
        [StringLength(50)]
        [Display(Name = "Hình thức")]
        public string HinhThuc { get; set; } = "Trực tiếp"; // Trực tiếp | Trực tuyến | Kết hợp

        [Required(ErrorMessage = "Trạng thái khóa học không được để trống")]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "DangMo"; // SapMo | DangMo | DangHoc | DaKetThuc | BiHuy

        [Display(Name = "Mô tả khóa học")]
        [DataType(DataType.MultilineText)]
        public string? MoTa { get; set; }

        // Navigation Properties (Module 2)
        [ForeignKey("MaMonHoc")]
        public virtual MonHoc? MonHoc { get; set; }

        [ForeignKey("MaGiangVien")]
        public virtual GiangVien? GiangVien { get; set; }

        public virtual ICollection<DangKyKhoaHoc> DangKyKhoaHocs { get; set; } = new List<DangKyKhoaHoc>();
    }
}
