// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// ViewModel dữ liệu thống kê và môn học tiêu biểu cho Trang chủ

using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    public class HomeStatsViewModel
    {
        public int TongSoMon { get; set; }
        public int SoMonDangMo { get; set; }
        public int TongTinChi { get; set; }
        public decimal HocPhiTb { get; set; }
        public decimal HocPhiThapNhat { get; set; }
        public int TongHocVien { get; set; }
        public List<MonHoc> MonHocTieuBieu { get; set; } = new List<MonHoc>();
    }
}
