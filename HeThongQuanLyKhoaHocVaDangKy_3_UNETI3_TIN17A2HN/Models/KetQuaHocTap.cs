// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Module 1: Entity Kết quả học tập

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models
{
    [Table("KetQuaHocTap")]
    public class KetQuaHocTap
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã kết quả")]
        public int MaKetQua { get; set; }

        [Required]
        [Display(Name = "Mã đăng ký")]
        public int MaDangKy { get; set; }

        [Range(0, 10, ErrorMessage = "Điểm chuyên cần từ 0 đến 10")]
        [Column(TypeName = "decimal(4,2)")]
        [Display(Name = "Điểm chuyên cần (10%)")]
        [DisplayFormat(DataFormatString = "{0:N1}")]
        public decimal DiemChuyenCan { get; set; } = 0;

        [Range(0, 10, ErrorMessage = "Điểm giữa kỳ từ 0 đến 10")]
        [Column(TypeName = "decimal(4,2)")]
        [Display(Name = "Điểm giữa kỳ (40%)")]
        [DisplayFormat(DataFormatString = "{0:N1}")]
        public decimal DiemGiuaKy { get; set; } = 0;

        [Range(0, 10, ErrorMessage = "Điểm cuối kỳ từ 0 đến 10")]
        [Column(TypeName = "decimal(4,2)")]
        [Display(Name = "Điểm cuối kỳ (50%)")]
        [DisplayFormat(DataFormatString = "{0:N1}")]
        public decimal DiemCuoiKy { get; set; } = 0;

        [Range(0, 10, ErrorMessage = "Điểm tổng kết từ 0 đến 10")]
        [Column(TypeName = "decimal(4,2)")]
        [Display(Name = "Điểm tổng kết")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal DiemTongKet { get; set; } = 0;

        [StringLength(50)]
        [Display(Name = "Xếp loại học lực")]
        public string? XepLoai { get; set; } = "Chưa xếp loại"; // XuatSac | Gioi | Kha | TrungBinh | Yeu

        [StringLength(50)]
        [Display(Name = "Kết quả")]
        public string? KetQua { get; set; } = "Chưa đánh giá"; // Dat | KhongDat

        [Display(Name = "Ngày cập nhật điểm")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        // Navigation Property (Module 5)
        [ForeignKey("MaDangKy")]
        public virtual DangKyKhoaHoc? DangKyKhoaHoc { get; set; }
    }
}
