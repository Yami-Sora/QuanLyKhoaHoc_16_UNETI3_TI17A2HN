// Họ và tên: Nguyễn Văn Mạnh
// Mã sinh viên: 23103100096
// Nội dung thực hiện: Module 2 - ViewModel Tra cứu, Lọc, Sắp xếp và Phân trang Khóa học

using System.ComponentModel.DataAnnotations;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;

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

        /// <summary>
        /// Mục 6.7: Kiểm tra tính khả dụng giao dịch cho người dùng
        /// Điều kiện khả dụng:
        /// 1. Trạng thái khóa học phải là DangMo (Đang mở đăng ký)
        /// 2. Khóa học còn chỗ (Sĩ số đã đăng ký < Sĩ số tối đa)
        /// 3. Ngày bắt đầu khóa học phải lớn hơn hoặc bằng ngày hiện tại (chưa quá hạn khai giảng)
        /// </summary>
        public bool KhaDungGiaoDich => TrangThai == "DangMo" && ConCho && NgayBatDau.Date >= DateTime.Today;

        public string LyDoKhongKhaDung
        {
            get
            {
                if (TrangThai == "BiHuy") return "Khóa học đã bị hủy";
                if (TrangThai == "DaKetThuc") return "Khóa học đã kết thúc";
                if (TrangThai == "DangHoc") return "Khóa học đang diễn ra, đã đóng đăng ký";
                if (TrangThai == "SapMo") return "Khóa học sắp mở, chưa tới đợt đăng ký";
                if (!ConCho) return "Khóa học đã hết chỗ (đã đủ sĩ số)";
                if (NgayBatDau.Date < DateTime.Today) return "Khóa học đã quá hạn ngày khai giảng";
                return string.Empty;
            }
        }
    }

    public class KhoaHocFilterViewModel
    {
        // 1. Dữ liệu danh sách khóa học hiển thị trên trang hiện tại
        public List<KhoaHocItemViewModel> DanhSachKhoaHoc { get; set; } = new();

        // 2. Thống kê Stat Cards tổng quan
        public int TongSoKhoaHoc { get; set; }
        public int SoKhoaDangMo { get; set; }
        public int SoKhoaConCho { get; set; }
        public int SoKhoaKhaDung { get; set; }
        public decimal HocPhiThapNhat { get; set; }
        public decimal HocPhiCaoNhat { get; set; }

        // 3. Tiêu chí Tìm kiếm (Mục 6.3)
        [Display(Name = "Từ khóa tìm kiếm")]
        public string? SearchString { get; set; }

        // 4. Tiêu chí Lọc kết hợp (Mục 6.4)
        [Display(Name = "Môn học")]
        public int? MaMonHoc { get; set; }

        [Display(Name = "Hình thức")]
        public string? HinhThuc { get; set; } // Trực tiếp | Trực tuyến | Kết hợp

        [Display(Name = "Trạng thái")]
        public string? TrangThai { get; set; } // SapMo | DangMo | DangHoc | DaKetThuc | BiHuy

        [Display(Name = "Khoảng học phí")]
        public string? KhoangHocPhi { get; set; } // duoi_2tr | 2tr_4tr | 4tr_6tr | tren_6tr

        [Display(Name = "Tình trạng chỗ")]
        public string? TinhTrangCho { get; set; } // con_cho | het_cho

        // 5. Tiêu chí Sắp xếp (Mục 6.5)
        [Display(Name = "Sắp xếp theo")]
        public string? SortBy { get; set; } = "ngay_desc";
        // ten_asc | ten_desc | ngay_asc | ngay_desc | hocphi_asc | hocphi_desc | cho_asc | cho_desc

        // 6. Phân trang chuẩn LINQ Skip/Take (Mục 6.6)
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / (PageSize > 0 ? PageSize : 5));
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        // Trợ giúp kiểm tra có bộ lọc đang kích hoạt hay không
        public bool HasActiveFilter =>
            !string.IsNullOrWhiteSpace(SearchString) ||
            MaMonHoc.HasValue ||
            !string.IsNullOrWhiteSpace(HinhThuc) ||
            !string.IsNullOrWhiteSpace(TrangThai) ||
            !string.IsNullOrWhiteSpace(KhoangHocPhi) ||
            !string.IsNullOrWhiteSpace(TinhTrangCho);
    }
}
