// Họ và tên: Nguyễn Công Thành
// Mã sinh viên: 23103100102
// Nội dung thực hiện: Module 5 - ViewModel Bảng điểm khóa học (Nhập điểm hàng loạt)

using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    // Dòng nhập điểm từng học viên
    public class NhapDiemHocVienItem
    {
        public int MaDangKy { get; set; }
        public int MaHocVien { get; set; }
        public string HoTenHocVien { get; set; } = string.Empty;
        public string? EmailHocVien { get; set; }

        public int? MaKetQua { get; set; }
        public bool DaCoKetQua => MaKetQua.HasValue;

        [Range(0, 10, ErrorMessage = "Điểm chuyên cần phải từ 0 đến 10")]
        [Display(Name = "Điểm CC (10%)")]
        public decimal DiemChuyenCan { get; set; }

        [Range(0, 10, ErrorMessage = "Điểm giữa kỳ phải từ 0 đến 10")]
        [Display(Name = "Điểm GK (40%)")]
        public decimal DiemGiuaKy { get; set; }

        [Range(0, 10, ErrorMessage = "Điểm cuối kỳ phải từ 0 đến 10")]
        [Display(Name = "Điểm CK (50%)")]
        public decimal DiemCuoiKy { get; set; }

        // Điểm và xếp loại tự tính
        public decimal DiemTongKet { get; set; }
        public string? XepLoai { get; set; }
        public string? KetQua { get; set; }
    }

    // Bảng điểm chi tiết của khóa học
    public class BangDiemKhoaHocViewModel
    {
        // Thông tin khóa học
        public int MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; } = string.Empty;
        public string? TenMonHoc { get; set; }
        public int SoTinChi { get; set; }
        public string? TenGiangVien { get; set; }
        public string? TrangThaiKhoaHoc { get; set; }

        // Thống kê sĩ số
        public int TongSoDangKyHoanThanh { get; set; }
        public int SoDaCoKetQua { get; set; }
        public int SoChuaCoKetQua { get; set; }

        // Thống kê điểm
        public decimal? DiemTBLop { get; set; }
        public int SoDat { get; set; }
        public int SoKhongDat { get; set; }
        public double TyLeDat => TongSoDangKyHoanThanh > 0
            ? Math.Round((double)SoDat / TongSoDangKyHoanThanh * 100, 1)
            : 0;

        // Danh sách điểm từng học viên
        public List<NhapDiemHocVienItem> DanhSachDiem { get; set; } = new();
    }

    // Khóa học trong danh sách nhập điểm
    public class KhoaHocKetQuaViewModel
    {
        public int MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; } = string.Empty;
        public string? TenMonHoc { get; set; }
        public string? TenGiangVien { get; set; }
        public string? TrangThai { get; set; }
        public string? HinhThuc { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }

        // Thống kê điểm và tiến độ
        public int SoDangKyHoanThanh { get; set; }
        public int SoDaCoKetQua { get; set; }
        public int SoChuaCoKetQua => SoDangKyHoanThanh - SoDaCoKetQua;
        public decimal? DiemTBLop { get; set; }
        public double TienDoNhapDiem => SoDangKyHoanThanh > 0
            ? Math.Round((double)SoDaCoKetQua / SoDangKyHoanThanh * 100, 0)
            : 0;
    }

    // Bộ lọc và phân trang khóa học
    public class KetQuaFilterViewModel
    {
        // Bộ lọc
        public string? SearchString { get; set; }
        public int? MaMonHoc { get; set; }
        public string? TrangThai { get; set; }
        public string? TinhTrangDiem { get; set; }
        public string? SortBy { get; set; } = "ten_asc";

        // Danh sách khóa học
        public List<KhoaHocKetQuaViewModel> DanhSachKhoaHoc { get; set; } = new();

        // Thống kê tổng quan
        public int TongSoKhoaHoc { get; set; }
        public int SoKhoaHocDaNhapDiem { get; set; }
        public int SoKhoaHocChuaNhapDiem { get; set; }
        public int TongSoHocVienCanNhapDiem { get; set; }

        // Phân trang
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
