// Họ và tên: Nguyễn Công Thành
// Mã sinh viên: 23103100102
// Nội dung thực hiện: Module 5 - ViewModel Kết quả học tập cá nhân của Học viên

using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    public class KetQuaCuaToiViewModel
    {
        // Thông tin học viên
        public int MaHocVien { get; set; }
        public string HoTenHocVien { get; set; } = string.Empty;
        public string? EmailHocVien { get; set; }

        // Danh sách kết quả từng khóa
        public List<KetQuaCuaToiItem> DanhSachKetQua { get; set; } = new();

        // Thống kê cá nhân
        public decimal? DiemTBToanBo { get; set; }
        public int TongSoKhoaDaHoc { get; set; }
        public int SoKhoaDat { get; set; }
        public int SoKhoaKhongDat { get; set; }
        public int SoKhoaChuaCoKetQua { get; set; }
        public int TongTinChi { get; set; }
        public int TinChiDat { get; set; }

        public double TyLeDat => TongSoKhoaDaHoc > 0
            ? Math.Round((double)SoKhoaDat / TongSoKhoaDaHoc * 100, 1)
            : 0;
    }

    // Kết quả từng khóa học của học viên
    public class KetQuaCuaToiItem
    {
        // Thông tin khóa học
        public int MaDangKy { get; set; }
        public int MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; } = string.Empty;
        public string? TenMonHoc { get; set; }
        public int SoTinChi { get; set; }
        public string? TenGiangVien { get; set; }
        public string? HinhThucHoc { get; set; }

        // Điểm và kết quả
        public bool DaCoKetQua { get; set; }
        public decimal DiemChuyenCan { get; set; }
        public decimal DiemGiuaKy { get; set; }
        public decimal DiemCuoiKy { get; set; }
        public decimal DiemTongKet { get; set; }
        public string? XepLoai { get; set; }
        public string? KetQua { get; set; }
        public DateTime? NgayCapNhat { get; set; }
    }
}
