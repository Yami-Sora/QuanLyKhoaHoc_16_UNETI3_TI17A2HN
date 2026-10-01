// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Module 1: Entity Môn học

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models
{
    [Table("MonHoc")]
    public class MonHoc
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã môn học")]
        public int MaMonHoc { get; set; }

        [Required(ErrorMessage = "Tên môn học bắt buộc phải nhập")]
        [StringLength(150, ErrorMessage = "Tên môn học tối đa 150 ký tự")]
        [Display(Name = "Tên môn học")]
        public string TenMonHoc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số tín chỉ bắt buộc phải nhập")]
        [Range(1, 10, ErrorMessage = "Số tín chỉ phải từ 1 đến 10")]
        [Display(Name = "Số tín chỉ")]
        public int SoTinChi { get; set; } = 3;

        [Display(Name = "Mô tả chi tiết")]
        [DataType(DataType.MultilineText)]
        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Học phí chuẩn không được để trống")]
        [Range(0, 1000000000, ErrorMessage = "Học phí phải lớn hơn hoặc bằng 0 đ")]
        [Display(Name = "Học phí định mức")]
        [DisplayFormat(DataFormatString = "{0:N0} đ", ApplyFormatInEditMode = false)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal HocPhi { get; set; }

        [Display(Name = "Trạng thái giảng dạy")]
        public bool TrangThai { get; set; } = true; // true: Đang mở, false: Tạm dừng

        // Danh sách khóa học (Module 2)
        public virtual ICollection<KhoaHoc> KhoaHocs { get; set; } = new List<KhoaHoc>();
    }
}
