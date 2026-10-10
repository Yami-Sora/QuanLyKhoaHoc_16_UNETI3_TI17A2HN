// Họ và tên: Nguyễn Công Thành
// Mã sinh viên: 23103100102
// Nội dung thực hiện: Module 5 - ViewModel Thống kê tổng hợp bằng LINQ

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    /// <summary>
    /// ViewModel tổng hợp 8 loại thống kê LINQ theo yêu cầu đề bài (Mục 9.4).
    /// Tất cả gộp trong 1 trang, mỗi mục 1 section.
    /// LINQ: Count(), Sum(), Average(), GroupBy(), OrderBy(), OrderByDescending(), Any().
    /// </summary>
    public class ThongKeViewModel
    {
        // =====================================================================
        // 1. SỐ KHÓA HỌC THEO MÔN HỌC — GroupBy(), Count()
        // =====================================================================
        public List<ThongKeKhoaHocTheoMonViewModel> KhoaHocTheoMon { get; set; } = new();

        // =====================================================================
        // 2. SỐ HỌC VIÊN ĐĂNG KÝ THEO KHÓA — GroupBy(), Count()
        // =====================================================================
        public List<ThongKeHocVienTheoKhoaViewModel> HocVienTheoKhoa { get; set; } = new();

        // =====================================================================
        // 3. KHÓA HỌC CÓ NHIỀU HỌC VIÊN NHẤT — OrderByDescending(), Take()
        // =====================================================================
        public List<ThongKeTopKhoaHocViewModel> TopKhoaHocNhieuHocVien { get; set; } = new();

        // =====================================================================
        // 4. DOANH THU HỌC PHÍ — Sum(), GroupBy()
        // =====================================================================
        public decimal TongDoanhThu { get; set; }
        public List<ThongKeDoanhThuTheoKhoaViewModel> DoanhThuTheoKhoa { get; set; } = new();

        // =====================================================================
        // 5. TỔNG CÔNG NỢ HỌC PHÍ — Sum()
        // =====================================================================
        public decimal TongCongNo { get; set; }
        public List<ThongKeCongNoTheoKhoaViewModel> CongNoTheoKhoa { get; set; } = new();

        // =====================================================================
        // 6. ĐIỂM TRUNG BÌNH THEO KHÓA — Average(), GroupBy()
        // =====================================================================
        public List<ThongKeDiemTBTheoKhoaViewModel> DiemTBTheoKhoa { get; set; } = new();

        // =====================================================================
        // 7. TỶ LỆ ĐẠT / KHÔNG ĐẠT — Count(), tính %
        // =====================================================================
        public int TongSoKetQua { get; set; }
        public int SoDat { get; set; }
        public int SoKhongDat { get; set; }
        public double TyLeDat => TongSoKetQua > 0 ? Math.Round((double)SoDat / TongSoKetQua * 100, 1) : 0;
        public double TyLeKhongDat => TongSoKetQua > 0 ? Math.Round((double)SoKhongDat / TongSoKetQua * 100, 1) : 0;

        // =====================================================================
        // 8. SỐ ĐĂNG KÝ THEO THÁNG — GroupBy() theo tháng
        // =====================================================================
        public List<ThongKeDangKyTheoThangViewModel> DangKyTheoThang { get; set; } = new();
    }

    // =====================================================================
    // CÁC SUB-VIEWMODEL CHO TỪNG LOẠI THỐNG KÊ
    // =====================================================================

    /// <summary>1. Số khóa học theo môn</summary>
    public class ThongKeKhoaHocTheoMonViewModel
    {
        public int MaMonHoc { get; set; }
        public string TenMonHoc { get; set; } = string.Empty;
        public int SoKhoaHoc { get; set; }
        public int SoTinChi { get; set; }
    }

    /// <summary>2. Số học viên đăng ký theo khóa</summary>
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

    /// <summary>3. Top khóa học nhiều học viên nhất</summary>
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

    /// <summary>4. Doanh thu theo khóa</summary>
    public class ThongKeDoanhThuTheoKhoaViewModel
    {
        public int MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; } = string.Empty;
        public string? TenMonHoc { get; set; }
        public decimal DoanhThu { get; set; }
        public int SoHocVienDaDong { get; set; }
    }

    /// <summary>5. Công nợ theo khóa</summary>
    public class ThongKeCongNoTheoKhoaViewModel
    {
        public int MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; } = string.Empty;
        public decimal CongNo { get; set; }
        public int SoHocVienConNo { get; set; }
    }

    /// <summary>6. Điểm trung bình theo khóa</summary>
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

    /// <summary>8. Số đăng ký theo tháng</summary>
    public class ThongKeDangKyTheoThangViewModel
    {
        public int Nam { get; set; }
        public int Thang { get; set; }
        public string NhanThang => $"T{Thang}/{Nam}";
        public int SoDangKy { get; set; }
    }
}
