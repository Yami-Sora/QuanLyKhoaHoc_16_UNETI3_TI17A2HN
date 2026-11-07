// Họ và tên: Nguyễn Văn Mạnh
// Mã sinh viên: 23103100096
// Nội dung thực hiện: Module 2 - ViewModel tra cứu, lọc, sắp xếp và phân trang khóa học

using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    public class KhoaHocItemViewModel
    {
        public int MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; } = string.Empty;
        public int MaMonHoc { get; set; }
        public string TenMonHoc { get; set; } = string.Empty;
        public int SoTinChi { get; set; }
        public int MaGiangVien { get; set; }
        public string TenGiangVien { get; set; } = string.Empty;
        public string? HocVi { get; set; }
        public string? ChuyenMon { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public int SoLuongToiDa { get; set; }
        public int SoLuongDaDangKy { get; set; }
        public int SoLuongConLai => Math.Max(0, SoLuongToiDa - SoLuongDaDangKy);
        public bool ConCho => SoLuongConLai > 0;
        public decimal HocPhi { get; set; }
        public string HinhThuc { get; set; } = "Trực tiếp";
        public string TrangThai { get; set; } = "DangMo";
        public string? MoTa { get; set; }

        // Kiểm tra tính khả dụng giao dịch cho học viên
        public bool KhaDungGiaoDich => TrangThai == "DangMo" && ConCho && NgayBatDau.Date > DateTime.Today;

        public string LyDoKhongKhaDung => TrangThai switch
        {
            "BiHuy" => "Khóa học đã bị hủy",
            "DaKetThuc" => "Khóa học đã kết thúc",
            "DangHoc" => "Khóa học đang diễn ra, đã đóng đăng ký",
            "SapMo" => "Khóa học sắp mở, chưa tới đợt đăng ký",
            _ when NgayKetThuc.Date < DateTime.Today => "Khóa học đã kết thúc",
            _ when NgayBatDau.Date <= DateTime.Today => "Khóa học đã đến ngày khai giảng hoặc đang diễn ra, đã đóng đăng ký",
            _ when !ConCho => "Khóa học đã hết chỗ (đã đủ sĩ số)",
            _ => string.Empty
        };
    }

    public class KhoaHocFilterViewModel
    {
        // 1. Danh sách khóa học hiển thị
        public List<KhoaHocItemViewModel> DanhSachKhoaHoc { get; set; } = new();

        // 2. Thống kê Stat Cards
        public int TongSoKhoaHoc { get; set; }
        public int SoKhoaDangMo { get; set; }
        public int SoKhoaConCho { get; set; }
        public int SoKhoaKhaDung { get; set; }
        public decimal HocPhiThapNhat { get; set; }
        public decimal HocPhiCaoNhat { get; set; }

        // 3. Tiêu chí Tìm kiếm & Lọc
        [Display(Name = "Từ khóa tìm kiếm")]
        public string? SearchString { get; set; }

        [Display(Name = "Môn học")]
        public int? MaMonHoc { get; set; }

        [Display(Name = "Hình thức")]
        public string? HinhThuc { get; set; }

        [Display(Name = "Trạng thái")]
        public string? TrangThai { get; set; }

        [Display(Name = "Khoảng học phí")]
        public string? KhoangHocPhi { get; set; }

        [Display(Name = "Tình trạng chỗ")]
        public string? TinhTrangCho { get; set; }

        // 4. Sắp xếp
        [Display(Name = "Sắp xếp theo")]
        public string? SortBy { get; set; } = "ngay_desc";

        // 5. Phân trang
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 6;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / (PageSize > 0 ? PageSize : 6));
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        // Trạng thái bộ lọc đang kích hoạt
        public bool HasActiveFilter =>
            !string.IsNullOrWhiteSpace(SearchString) ||
            (MaMonHoc.HasValue && MaMonHoc.Value > 0) ||
            (!string.IsNullOrWhiteSpace(HinhThuc) && !HinhThuc.Equals("all", StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrWhiteSpace(TrangThai) && !TrangThai.Equals("all", StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrWhiteSpace(KhoangHocPhi) && !KhoangHocPhi.Equals("all", StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrWhiteSpace(TinhTrangCho) && !TinhTrangCho.Equals("all", StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrWhiteSpace(SortBy) && SortBy != "ngay_desc");
    }
}
