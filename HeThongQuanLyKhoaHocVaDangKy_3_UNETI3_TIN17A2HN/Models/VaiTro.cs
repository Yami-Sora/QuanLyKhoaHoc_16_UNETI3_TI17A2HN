// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Module 1: Hằng số vai trò

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models
{
    public static class VaiTro
    {
        public const string Admin = "Admin";
        public const string NhanVien = "NhanVien";
        public const string HocVien = "HocVien";

        public static readonly string[] DanhSachVaiTro = { Admin, NhanVien, HocVien };

        public static string LayTenHienThi(string? vaiTro)
        {
            return vaiTro switch
            {
                Admin => "Quản trị viên",
                NhanVien => "Nhân viên đào tạo",
                HocVien => "Học viên",
                _ => "Không xác định"
            };
        }
    }
}
