// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Module 1: Entity Đăng ký khóa học

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models
{
    [Table("DangKyKhoaHoc")]
    public class DangKyKhoaHoc
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã đăng ký")]
        public int MaDangKy { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn học viên")]
        [Display(Name = "Học viên")]
        public int MaHocVien { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn khóa học")]
        [Display(Name = "Khóa học")]
        public int MaKhoaHoc { get; set; }

        [Display(Name = "Ngày đăng ký")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime NgayDangKy { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái giao dịch")]
        public string TrangThai { get; set; } = "ChoXuLy"; // ChoXuLy | DangXuLy | HoanThanh | BiHuy

        [Range(0, 100000000, ErrorMessage = "Số tiền phải >= 0")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Số tiền đã nộp")]
        [DisplayFormat(DataFormatString = "{0:N0} đ", ApplyFormatInEditMode = false)]
        public decimal SoTienDaDong { get; set; } = 0;

        [Range(0, 100000000, ErrorMessage = "Số tiền phải >= 0")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Học phí còn lại")]
        [DisplayFormat(DataFormatString = "{0:N0} đ", ApplyFormatInEditMode = false)]
        public decimal SoTienConLai { get; set; } = 0;

        [Display(Name = "Ghi chú giao dịch")]
        public string? GhiChu { get; set; }

        [Display(Name = "Ngày xác nhận")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? NgayXacNhan { get; set; }

        // Navigation Properties (Module 4 & 5)
        [ForeignKey("MaHocVien")]
        public virtual HocVien? HocVien { get; set; }

        [ForeignKey("MaKhoaHoc")]
        public virtual KhoaHoc? KhoaHoc { get; set; }

        public virtual KetQuaHocTap? KetQuaHocTap { get; set; }
    }
}
