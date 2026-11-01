// Họ và tên: Nguyễn Công Thành
// Mã sinh viên: 23103100102
// Nội dung thực hiện: Module 5 - Interface Repository cho Quản lý kết quả học tập & bảng điểm

using Microsoft.AspNetCore.Mvc.Rendering;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories
{
    public interface IKetQuaHocTapRepository
    {
        // 1. Lấy danh sách khóa học kèm tiến độ nhập điểm và SelectList môn học
        Task<(KetQuaFilterViewModel Filter, SelectList MonHocList)> GetKetQuaFilterAsync(KetQuaFilterViewModel filter);

        // 2. Lấy thông tin bảng điểm chi tiết của một khóa học
        Task<BangDiemKhoaHocViewModel?> GetBangDiemAsync(int maKhoaHoc);

        // 3. Lưu điểm hàng loạt cho học viên trong khóa học
        Task<(bool Success, int CountSaved, string? ErrorMessage)> LuuDiemAsync(BangDiemKhoaHocViewModel model);

        // 4. Lấy kết quả học tập cá nhân của học viên đang đăng nhập
        Task<KetQuaCuaToiViewModel?> GetKetQuaCuaToiAsync(int maTaiKhoan, string? vaiTro);
    }
}
