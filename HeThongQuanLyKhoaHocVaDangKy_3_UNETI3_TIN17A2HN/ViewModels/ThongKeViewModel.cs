// Họ và tên: Nguyễn Công Thành
// Mã sinh viên: 23103100102
// Nội dung thực hiện: Module 5 - ViewModel Thống kê tổng hợp bằng LINQ

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    public class ThongKeViewModel
    {
        public List<ThongKeKhoaHocTheoMonViewModel> KhoaHocTheoMon { get; set; } = new();

        public List<ThongKeHocVienTheoKhoaViewModel> HocVienTheoKhoa { get; set; } = new();

        public List<ThongKeTopKhoaHocViewModel> TopKhoaHocNhieuHocVien { get; set; } = new();

        public decimal TongDoanhThu { get; set; }

        public List<ThongKeDoanhThuTheoKhoaViewModel> DoanhThuTheoKhoa { get; set; } = new();

        public decimal TongCongNo { get; set; }

        public List<ThongKeCongNoTheoKhoaViewModel> CongNoTheoKhoa { get; set; } = new();

        public List<ThongKeDiemTBTheoKhoaViewModel> DiemTBTheoKhoa { get; set; } = new();

        public int TongSoKetQua { get; set; }
        public int SoDat { get; set; }
        public int SoKhongDat { get; set; }
        public double TyLeDat => TongSoKetQua > 0 ? Math.Round((double)SoDat / TongSoKetQua * 100, 1) : 0;
        public double TyLeKhongDat => TongSoKetQua > 0 ? Math.Round((double)SoKhongDat / TongSoKetQua * 100, 1) : 0;

        // Danh sách các năm có dữ liệu đăng ký
        public List<int> DanhSachNamCoDuLieu { get; set; } = new();

        public List<ThongKeDangKyTheoThangViewModel> DangKyTheoThang { get; set; } = new();
    }

    //1. Số khóa học theo môn
    public class ThongKeKhoaHocTheoMonViewModel
    {
        public int MaMonHoc { get; set; }
        public string TenMonHoc { get; set; } = string.Empty;
        public int SoKhoaHoc { get; set; }
        public int SoTinChi { get; set; }
    }

    //2. Số học viên đăng ký theo khóa
    public class ThongKeHocVienTheoKhoaViewModel
    {
        public int MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; } = string.Empty;
        public string? TenMonHoc { get; set; }
        public int SoHocVienDangKy { get; set; }
        public int SoLuongToiDa { get; set; }
        public double TyLeLapDay => SoLuongToiDa > 0
            ? Math.Round((double)SoHocVienDangKy / SoLuongToiDa * 100, 1)
            : 0;
    }

    //3. Top khóa học nhiều học viên nhất
    public class ThongKeTopKhoaHocViewModel
    {
        public int MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; } = string.Empty;
        public string? TenMonHoc { get; set; }
        public string? TenGiangVien { get; set; }
        public int SoHocVien { get; set; }
        public int SoLuongToiDa { get; set; }
        public decimal HocPhi { get; set; }
    }

    //4. Doanh thu theo khóa
    public class ThongKeDoanhThuTheoKhoaViewModel
    {
        public int MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; } = string.Empty;
        public string? TenMonHoc { get; set; }
        public decimal DoanhThu { get; set; }
        public int SoHocVienDaDong { get; set; }
    }

    //5. Công nợ theo khóa
    public class ThongKeCongNoTheoKhoaViewModel
    {
        public int MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; } = string.Empty;
        public decimal CongNo { get; set; }
        public int SoHocVienConNo { get; set; }
    }

    //Điểm trung bình theo khóa
    public class ThongKeDiemTBTheoKhoaViewModel
    {
        public int MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; } = string.Empty;
        public string? TenMonHoc { get; set; }
        public decimal DiemTB { get; set; }
        public int SoHocVienCoKetQua { get; set; }
        public int SoDat { get; set; }
        public int SoKhongDat { get; set; }
    }

    //Số đăng ký theo tháng
    public class ThongKeDangKyTheoThangViewModel
    {
        public int Nam { get; set; }
        public int Thang { get; set; }
        public string NhanThang => $"T{Thang}/{Nam}";
        public int SoDangKy { get; set; }
    }
}
