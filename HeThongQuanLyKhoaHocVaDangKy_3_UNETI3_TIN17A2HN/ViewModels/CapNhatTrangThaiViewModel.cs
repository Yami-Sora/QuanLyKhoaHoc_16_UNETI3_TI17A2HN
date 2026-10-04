// Họ và tên: Bạch Đức Sự
// Mã sinh viên: 23103100063
// Nội dung thực hiện: Module 4 - Tiếp nhận đăng ký, Xác nhận/Hủy đăng ký và Quản lý trạng thái.

using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    public class CapNhatTrangThaiViewModel
    {
        public int MaDangKy { get; set; }

        [Display(Name = "Học viên")]
        public string HoTenHocVien { get; set; } = string.Empty;

        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Khóa học")]
        public string TenKhoaHoc { get; set; } = string.Empty;

        [Display(Name = "Trạng thái khóa học")]
        public string TrangThaiKhoaHoc { get; set; } = string.Empty;

        [Display(Name = "Học phí khóa học")]
        public decimal HocPhi { get; set; }

        [Display(Name = "Sĩ số hiện tại")]
        public int SiSoHienTai { get; set; }

        [Display(Name = "Sĩ số tối đa")]
        public int SoLuongToiDa { get; set; }

        [Display(Name = "Ngày đăng ký")]
        public DateTime NgayDangKy { get; set; }

        [Display(Name = "Trạng thái hiện tại")]
        public string TrangThaiHienTai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn trạng thái mới cần chuyển")]
        [Display(Name = "Trạng thái mới")]
        public string TrangThaiMoi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số tiền đã đóng")]
        [Range(0, 100000000, ErrorMessage = "Số tiền đã đóng phải >= 0 và <= 100.000.000 đ")]
        [Display(Name = "Số tiền đã nộp (VNĐ)")]
        public decimal SoTienDaDong { get; set; }

        [Display(Name = "Số tiền còn lại (VNĐ)")]
        public decimal SoTienConLai { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
        [Display(Name = "Ghi chú giao dịch / Lý do chuyển trạng thái")]
        public string? GhiChu { get; set; }

        public List<string> TrangThaiHopLe { get; set; } = new();
    }
}
