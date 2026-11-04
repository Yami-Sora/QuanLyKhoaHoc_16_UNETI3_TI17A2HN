// Họ và tên: Nguyễn Văn Mạnh
// Mã sinh viên: 23103100096
// Nội dung thực hiện: Module 2 - ViewModel tra cứu, lọc, sắp xếp và phân trang khóa học

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
        /// 6.7. Kiểm tra tính khả dụng giao dịch cho người dùng
        /// </summary>
        public bool KhaDungGiaoDich => TrangThai == "DangMo" && ConCho && NgayBatDau.Date >= DateTime.Today;

        public string LyDoKhongKhaDung
        {
            get
            {
                if (TrangThai == "BiHuy") return "Khóa học đã bị hủy";
                if (TrangThai == "DaKetThuc" || NgayKetThuc.Date < DateTime.Today) return "Khóa học đã kết thúc";
                if (TrangThai == "DangHoc" || NgayBatDau.Date <= DateTime.Today) return "Khóa học đang diễn ra, đã đóng đăng ký";
                if (TrangThai == "SapMo") return "Khóa học sắp mở, chưa tới đợt đăng ký";
                if (!ConCho) return "Khóa học đã hết chỗ (đã đủ sĩ số)";
                return string.Empty;
            }
        }
    }

    public class KhoaHocFilterViewModel
    {
        // Danh sách khóa học hiển thị
        public List<KhoaHocItemViewModel> DanhSachKhoaHoc { get; set; } = new();

        // Thống kê tổng quan cho các thẻ thống kê
        public int TongSoKhoaHoc { get; set; }
        public int SoKhoaDangMo { get; set; }
        public int SoKhoaConCho { get; set; }
        public int SoKhoaKhaDung { get; set; }
        public decimal HocPhiThapNhat { get; set; }
        public decimal HocPhiCaoNhat { get; set; }

        // 6.3. Tiêu chí tìm kiếm
        [Display(Name = "Từ khóa tìm kiếm")]
        public string? SearchString { get; set; }

        // 6.4. Tiêu chí lọc kết hợp
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

        // 6.5. Tiêu chí sắp xếp
        [Display(Name = "Sắp xếp theo")]
        public string? SortBy { get; set; } = "ngay_desc";

        // 6.6. Phân trang dữ liệu
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / (PageSize > 0 ? PageSize : 5));
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        // Kiểm tra trạng thái bộ lọc đang kích hoạt
        public bool HasActiveFilter =>
            !string.IsNullOrWhiteSpace(SearchString) ||
            (MaMonHoc.HasValue && MaMonHoc.Value > 0) ||
            !string.IsNullOrWhiteSpace(HinhThuc) ||
            !string.IsNullOrWhiteSpace(TrangThai) ||
            !string.IsNullOrWhiteSpace(KhoangHocPhi) ||
            !string.IsNullOrWhiteSpace(TinhTrangCho) ||
            (!string.IsNullOrWhiteSpace(SortBy) && SortBy != "ngay_desc");
    }
}
