// Họ và tên: Bạch Đức Sự
// Mã sinh viên: 23103100063
// Nội dung thực hiện: Tiếp nhận đăng ký, xử lý trạng thái bằng Repository Pattern, đồng bộ UI.

using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    public class ChiTietDangKyViewModel
    {
        public DangKyKhoaHoc DangKy { get; set; } = new();

        public int SiSoHienTai { get; set; }

        public int SiSoToiDa { get; set; }

        public List<DangKyKhoaHoc> CacDangKyKhac { get; set; } = new();

        public decimal HocPhi => DangKy?.KhoaHoc?.HocPhi ?? 0;

        public decimal SoTienDaDong => DangKy?.SoTienDaDong ?? 0;

        public decimal SoTienConLai => DangKy?.SoTienConLai ?? 0;

        public decimal TyLeDongHocPhi => HocPhi > 0 ? Math.Min(100, Math.Round((SoTienDaDong / HocPhi) * 100, 1)) : 0;

        public string TrangThaiThanhToan => SoTienConLai <= 0 && SoTienDaDong > 0
            ? "Đã nộp đủ 100%"
            : (SoTienDaDong > 0 ? "Đã nộp một phần" : "Chưa nộp tiền");
    }
}
