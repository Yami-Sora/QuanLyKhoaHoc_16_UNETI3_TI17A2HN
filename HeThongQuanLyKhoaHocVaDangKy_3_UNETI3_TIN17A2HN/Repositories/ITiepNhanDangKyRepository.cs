// Họ và tên: Bạch Đức Sự
// Mã sinh viên: 23103100063
// Nội dung thực hiện: Tiếp nhận đăng ký, xử lý trạng thái bằng Repository Pattern, đồng bộ UI.

using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories
{
    public interface ITiepNhanDangKyRepository
    {
        // 1. Lấy danh sách đăng ký có tìm kiếm, lọc đa tiêu chí, sắp xếp và phân trang LINQ
        Task<TiepNhanFilterViewModel> GetDanhSachDangKyAsync(TiepNhanFilterViewModel filter);

        // 2. Lấy thông tin chi tiết hồ sơ đăng ký kèm theo sĩ số và lịch sử đăng ký khác
        Task<ChiTietDangKyViewModel?> GetChiTietDangKyAsync(int id);

        // 3. Lấy dữ liệu chuẩn bị cho form cập nhật trạng thái
        Task<CapNhatTrangThaiViewModel?> GetCapNhatTrangThaiViewModelAsync(int id);

        // 4. Kiểm tra điều kiện nghiệp vụ trước khi duyệt (sĩ số, trạng thái khóa học, chống trùng)
        Task<(bool HopLe, string ThongBaoLoi)> KiemTraDieuKienDuyetDangKyAsync(int maDangKy, string trangThaiMoi);

        // 5. Cập nhật trạng thái và tài chính của hồ sơ đăng ký
        Task<(bool ThanhCong, string ThongBao)> CapNhatTrangThaiAsync(CapNhatTrangThaiViewModel model);

        // 6. Tiếp nhận nhanh đơn đăng ký (Chờ xử lý -> Đang xử lý)
        Task<(bool ThanhCong, string ThongBao)> TiepNhanNhanhAsync(int id);

        // 7. Xác nhận hoàn thành đơn đăng ký và ghi nhận học phí (Đang xử lý -> Hoàn thành)
        Task<(bool ThanhCong, string ThongBao)> XacNhanHoanThanhAsync(int id, decimal? soTienDong, string? ghiChuXacNhan);

        // 8. Hủy / Từ chối đơn đăng ký có kèm lý do
        Task<(bool ThanhCong, string ThongBao)> HuyDangKyAsync(int id, string lyDoHuy);

        // 9. Tiếp nhận ghi danh trực tiếp tại quầy
        Task<(bool ThanhCong, string ThongBao, int MaDangKyMoi)> TaoDangKyMoiAsync(TaoDangKyMoiViewModel model);

        // 10. Lấy dữ liệu in phiếu đăng ký / biên nhận
        Task<DangKyKhoaHoc?> GetDangKyForPrintAsync(int id);

        // 11. Nạp danh sách dữ liệu cho DropDownList (Học viên, Khóa học đang mở)
        Task<(IEnumerable<object> HocViens, IEnumerable<object> KhoaHocs)> LayDuLieuDropDownListAsync();
    }
}

