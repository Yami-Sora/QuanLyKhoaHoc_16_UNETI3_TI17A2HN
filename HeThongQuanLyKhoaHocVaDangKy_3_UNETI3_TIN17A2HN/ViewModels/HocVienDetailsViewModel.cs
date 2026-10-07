// Họ và tên: Ngọc Tùng Lâm
// Mã sinh viên: 23103100098
// Nội dung thực hiện: Module 3 - ViewModel Chi tiết Hồ sơ Học viên và Lịch sử Khóa học

using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    public class HocVienDangKyLichSuViewModel
    {
        public int MaDangKy { get; set; }
        public int MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; } = string.Empty;
        public string TenMonHoc { get; set; } = string.Empty;
        public int SoTinChi { get; set; }
        public string? TenGiangVien { get; set; }
        public DateTime NgayDangKy { get; set; }
        public string TrangThai { get; set; } = "ChoXuLy";
        public decimal HocPhi { get; set; }
        public decimal SoTienDaDong { get; set; }
        public decimal SoTienConLai { get; set; }
        public DateTime? NgayXacNhan { get; set; }
        public string? GhiChu { get; set; }
        
        // Kết quả học tập (nếu có)
        public decimal? DiemTongKet { get; set; }
        public string? XepLoai { get; set; }
        public string? KetQua { get; set; }
    }

    public class HocVienDetailsViewModel
    {
        public HocVien HocVien { get; set; } = new();
        public TaiKhoan? TaiKhoan { get; set; }
        public List<HocVienDangKyLichSuViewModel> LịchSuDangKy { get; set; } = new();

        public int TongKhoaDaDangKy => LịchSuDangKy.Count(d => d.TrangThai != "BiHuy");
        public int TongDonDangKy => LịchSuDangKy.Count;
        public int SoKhoaHoanThanh => LịchSuDangKy.Count(d => d.TrangThai == "HoanThanh");
        public int SoKhoaDangXuLy => LịchSuDangKy.Count(d => d.TrangThai == "DangXuLy");
        public int SoKhoaChoXuLy => LịchSuDangKy.Count(d => d.TrangThai == "ChoXuLy");
        public int SoKhoaBiHuy => LịchSuDangKy.Count(d => d.TrangThai == "BiHuy");
        public decimal TongHocPhiDaDong => LịchSuDangKy.Sum(d => d.SoTienDaDong);
        public decimal TongCongNoConLai => LịchSuDangKy.Where(d => d.TrangThai != "BiHuy").Sum(d => d.SoTienConLai);
    }
}
