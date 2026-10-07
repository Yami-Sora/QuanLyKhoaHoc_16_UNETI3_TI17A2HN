// Họ và tên: Ngọc Tùng Lâm
// Mã sinh viên: 23103100098
// Nội dung thực hiện: Module 3 - ViewModel Theo dõi Đăng ký và Quản lý Giao dịch của Học viên

using System.ComponentModel.DataAnnotations;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    public class TheoDoiDangKyItemViewModel
    {
        public int MaDangKy { get; set; }
        public int MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; } = string.Empty;
        public string TenMonHoc { get; set; } = string.Empty;
        public int SoTinChi { get; set; }
        public string? TenGiangVien { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public string HinhThuc { get; set; } = "Trực tiếp";
        public DateTime NgayDangKy { get; set; }
        public string TrangThai { get; set; } = "ChoXuLy";
        public decimal HocPhi { get; set; }
        public decimal SoTienDaDong { get; set; }
        public decimal SoTienConLai { get; set; }
        public DateTime? NgayXacNhan { get; set; }
        public string? GhiChu { get; set; }
        
        // Điều kiện hủy giao dịch (Mục 7.4)
        public bool DuocPhepTuhuy => TrangThai == "ChoXuLy";

        // Kết quả đào tạo (nếu có)
        public decimal? DiemTongKet { get; set; }
        public string? XepLoai { get; set; }
        public string? KetQua { get; set; }
    }

    public class TheoDoiDangKyFilterViewModel
    {
        public List<TheoDoiDangKyItemViewModel> DanhSachDangKy { get; set; } = new();

        // 1. Thống kê Stat Cards tổng quan
        public int TongSoDon { get; set; }
        public int SoDonChoXuLy { get; set; }
        public int SoDonDangXuLy { get; set; }
        public int SoDonHoanThanh { get; set; }
        public int SoDonBiHuy { get; set; }
        public decimal TongTienDaDong { get; set; }
        public decimal TongTienConLai { get; set; }

        // 2. Bộ lọc & Tìm kiếm
        [Display(Name = "Tìm kiếm khóa học")]
        public string? SearchKhoaHoc { get; set; }

        [Display(Name = "Trạng thái đơn")]
        public string? TrangThai { get; set; } = "TatCa";

        [Display(Name = "Sắp xếp theo")]
        public string? SortBy { get; set; } = "ngay_desc"; // ngay_desc, ngay_asc, hocphi_desc, hocphi_asc

        // 3. Phân trang LINQ
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 8;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / (PageSize > 0 ? PageSize : 8));
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
