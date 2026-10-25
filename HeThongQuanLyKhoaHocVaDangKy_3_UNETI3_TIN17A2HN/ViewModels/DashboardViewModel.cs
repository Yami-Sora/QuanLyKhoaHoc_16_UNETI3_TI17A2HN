// Họ và tên: Nguyễn Công Thành
// Mã sinh viên: 23103100102
// Nội dung thực hiện: Module 5 - ViewModel Dashboard tổng quan hệ thống

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    public class DashboardViewModel
    {
        // 9 chỉ số thống kê chính
        public int TongSoMonHoc { get; set; }
        public int TongSoKhoaHoc { get; set; }
        public int SoKhoaHocKhaDung { get; set; }
        public int TongSoHocVien { get; set; }
        public int TongSoGiaoDich { get; set; }
        public int SoGiaoDichChoXuLy { get; set; }
        public int SoGiaoDichDangXuLy { get; set; }
        public int SoGiaoDichHoanThanh { get; set; }
        public decimal TongDoanhThu { get; set; }

        // Chỉ số mở rộng
        public decimal TongCongNo { get; set; }
        public int SoGiaoDichBiHuy { get; set; }
        public int SoKhoaHocDangHoc { get; set; }
        public int SoKhoaHocDaKetThuc { get; set; }
        public int TongSoGiangVien { get; set; }

        // Dữ liệu biểu đồ
        public List<ChartDataItem> BieuDoDangKyTheoTrangThai { get; set; } = new();
        public List<ChartDataItem> BieuDoHocVienTheoKhoa { get; set; } = new();
        public List<ChartDataItem> BieuDoDangKyTheoThang { get; set; } = new();
        public List<ChartDataItem> BieuDoTyLeDatKhongDat { get; set; } = new();
    }

    // Dữ liệu biểu đồ Chart.js
    public class ChartDataItem
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string? Color { get; set; }
    }
}
