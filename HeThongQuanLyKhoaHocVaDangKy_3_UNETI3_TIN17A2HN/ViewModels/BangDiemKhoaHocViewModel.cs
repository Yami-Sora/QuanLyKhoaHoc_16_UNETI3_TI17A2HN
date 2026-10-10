// Họ và tên: Nguyễn Công Thành
// Mã sinh viên: 23103100102
// Nội dung thực hiện: Module 5 - ViewModel Bảng điểm khóa học (Nhập điểm hàng loạt)

using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels
{
    /// <summary>
    /// ViewModel cho từng dòng điểm học viên trong bảng nhập hàng loạt.
    /// Mỗi dòng tương ứng 1 đăng ký có TrangThai == "HoanThanh".
    /// </summary>
    public class NhapDiemHocVienItem
    {
        public int MaDangKy { get; set; }
        public int MaHocVien { get; set; }
        public string HoTenHocVien { get; set; } = string.Empty;
        public string? EmailHocVien { get; set; }

        /// <summary>Nếu null → chưa có kết quả, cần tạo mới. Nếu có → cập nhật.</summary>
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

        // Các trường tự tính — hiển thị trên View, server tính lại khi lưu
        public decimal DiemTongKet { get; set; }
        public string? XepLoai { get; set; }
        public string? KetQua { get; set; }
    }

    /// <summary>
    /// ViewModel cho toàn bộ bảng điểm 1 khóa học.
    /// Hiển thị thông tin khóa + danh sách điểm tất cả học viên đã hoàn thành đăng ký.
    /// </summary>
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

        // Thống kê điểm nhanh
        public decimal? DiemTBLop { get; set; }
        public int SoDat { get; set; }
        public int SoKhongDat { get; set; }
        public double TyLeDat => TongSoDangKyHoanThanh > 0
            ? Math.Round((double)SoDat / TongSoDangKyHoanThanh * 100, 1)
            : 0;

        // Danh sách điểm từng học viên
        public List<NhapDiemHocVienItem> DanhSachDiem { get; set; } = new();
    }

    /// <summary>
    /// ViewModel tóm tắt 1 khóa học trong danh sách chọn nhập điểm (trang Index).
    /// </summary>
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

        // Thống kê điểm
        public int SoDangKyHoanThanh { get; set; }
        public int SoDaCoKetQua { get; set; }
        public int SoChuaCoKetQua => SoDangKyHoanThanh - SoDaCoKetQua;
        public decimal? DiemTBLop { get; set; }

        // Hiển thị tiến độ nhập điểm
        public double TienDoNhapDiem => SoDangKyHoanThanh > 0
            ? Math.Round((double)SoDaCoKetQua / SoDangKyHoanThanh * 100, 0)
            : 0;
    }

    /// <summary>
    /// ViewModel bộ lọc + phân trang cho danh sách khóa học (trang Index nhập điểm).
    /// </summary>
    public class KetQuaFilterViewModel
    {
        // Bộ lọc
        public string? SearchString { get; set; }
        public int? MaMonHoc { get; set; }
        public string? TrangThai { get; set; }
        public string? TinhTrangDiem { get; set; } // "TatCa" | "DaNhapDiem" | "ChuaNhapDiem" | "NhapMotPhan"
        public string? SortBy { get; set; } = "ten_asc";

        // Dữ liệu danh sách
        public List<KhoaHocKetQuaViewModel> DanhSachKhoaHoc { get; set; } = new();

        // Thống kê tổng quan (stat cards)
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
