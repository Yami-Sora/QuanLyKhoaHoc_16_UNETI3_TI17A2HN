// Họ và tên: Bạch Đức Sự
// Mã sinh viên: 23103100063
// Nội dung thực hiện: Module 4 - Tiếp nhận đăng ký, Xác nhận/Hủy đăng ký và Quản lý trạng thái.

using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    public class TaoDangKyMoiViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn học viên")]
        [Display(Name = "Học viên")]
        public int MaHocVien { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn khóa học")]
        [Display(Name = "Khóa học đăng ký")]
        public int MaKhoaHoc { get; set; }

        [Display(Name = "Trạng thái ban đầu")]
        public string TrangThai { get; set; } = "ChoXuLy";

        [Range(0, 100000000, ErrorMessage = "Số tiền đóng phải >= 0")]
        [Display(Name = "Số tiền nộp đợt đầu (VNĐ)")]
        public decimal SoTienDaDong { get; set; } = 0;

        [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
        [Display(Name = "Ghi chú tiếp nhận đơn")]
        public string? GhiChu { get; set; }
    }
}
