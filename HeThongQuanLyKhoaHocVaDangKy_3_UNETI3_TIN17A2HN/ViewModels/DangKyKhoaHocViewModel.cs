// Họ và tên: Ngọc Tùng Lâm
// Mã sinh viên: 23103100098
// Nội dung thực hiện: Module 3 - ViewModel Đăng ký Khóa học phía Học viên

using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    public class DangKyKhoaHocViewModel
    {
        // Thông tin khóa học
        public int MaKhoaHoc { get; set; }
        public string? TenKhoaHoc { get; set; }
        public string? TenMonHoc { get; set; }
        public int SoTinChi { get; set; }
        public decimal HocPhi { get; set; }
        public string? TenGiangVien { get; set; }
        public string? HocVi { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public string? HinhThuc { get; set; } = "Trực tiếp";
        public string? TrangThaiKhoaHoc { get; set; } = "DangMo";
        public int SoLuongToiDa { get; set; }
        public int SiSoHienTai { get; set; }
        public int SoChoConLai => Math.Max(0, SoLuongToiDa - SiSoHienTai);
        public string? MoTaKhoaHoc { get; set; }

        // Thông tin học viên đăng ký
        public int MaHocVien { get; set; }
        public string? HoTenHocVien { get; set; }
        public string? EmailHocVien { get; set; }
        public string? SoDienThoaiHocVien { get; set; }

        // Trạng thái khả dụng giao dịch
        public bool KhaDungGiaoDich => (TrangThaiKhoaHoc ?? "DangMo") == "DangMo" && SoChoConLai > 0 && NgayBatDau.Date >= DateTime.Today;
        public string? LyDoKhongKhaDung { get; set; }

        // Trạng thái đã đăng ký trước đó
        public bool DaDangKy { get; set; }
        public int? MaDangKyCu { get; set; }
        public string? TrangThaiDangKyCu { get; set; }

        // Ghi chú của học viên khi đăng ký
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        [Display(Name = "Ghi chú / Nguyện vọng đặc biệt")]
        public string? GhiChu { get; set; }

        // Cam kết quy chế
        [Display(Name = "Tôi xác nhận thông tin và đồng ý với quy chế đào tạo")]
        public bool DongYQuyChe { get; set; }
    }
}
