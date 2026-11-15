// Họ và tên: Nguyễn Công Thành
// Mã sinh viên: 23103100102
// Nội dung thực hiện: Module 5 - Quản lý kết quả học tập, bảng điểm và nhập điểm hàng loạt

using Microsoft.AspNetCore.Mvc;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    public class KetQuaHocTapController : Controller
    {
        private readonly IKetQuaHocTapRepository _ketQuaRepo;

        public KetQuaHocTapController(IKetQuaHocTapRepository ketQuaRepo)
        {
            _ketQuaRepo = ketQuaRepo;
        }

        // 1. Danh sách khóa học để nhập điểm (Dành cho Admin & Nhân viên)
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        [HttpGet]
        public async Task<IActionResult> Index(KetQuaFilterViewModel filter)
        {
            var (filterResult, monHocList) = await _ketQuaRepo.GetKetQuaFilterAsync(filter);
            ViewBag.MonHocList = monHocList;
            return View(filterResult);
        }

        // 2. Bảng điểm chi tiết & Form nhập điểm hàng loạt (Admin & Nhân viên)
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        [HttpGet]
        public async Task<IActionResult> BangDiem(int maKhoaHoc)
        {
            var viewModel = await _ketQuaRepo.GetBangDiemAsync(maKhoaHoc);
            if (viewModel == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin khóa học yêu cầu.";
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // 3. Lưu bảng điểm hàng loạt (POST)
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LuuDiem(BangDiemKhoaHocViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu điểm không hợp lệ. Vui lòng kiểm tra lại các ô điểm (0-10).";
                return RedirectToAction(nameof(BangDiem), new { maKhoaHoc = model.MaKhoaHoc });
            }

            var (success, countSaved, errorMessage) = await _ketQuaRepo.LuuDiemAsync(model);
            if (success)
            {
                TempData["SuccessMessage"] = $"Lưu bảng điểm thành công ({countSaved} học viên có đủ 3 đầu điểm đã được ghi nhận)!";
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage ?? "Lỗi khi lưu bảng điểm vào cơ sở dữ liệu.";
            }

            return RedirectToAction(nameof(BangDiem), new { maKhoaHoc = model.MaKhoaHoc });
        }

        // 4. Kết quả học tập của tôi (Dành riêng cho Học viên xem điểm cá nhân)
        [AuthorizeRole(VaiTro.HocVien, VaiTro.Admin, VaiTro.NhanVien)]
        [HttpGet]
        public async Task<IActionResult> KetQuaCuaToi()
        {
            var (isAuthenticated, viewModel) = await LoadKetQuaCuaToiAsync();
            if (!isAuthenticated)
                return RedirectToAction("Login", "TaiKhoan");

            if (viewModel == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ học viên tương ứng với tài khoản này.";
                return RedirectToAction("Index", "Home");
            }

            return View(viewModel);
        }

        // 5. Trang in bảng điểm cá nhân (standalone, Layout = null)
        [AuthorizeRole(VaiTro.HocVien, VaiTro.Admin, VaiTro.NhanVien)]
        [HttpGet]
        public async Task<IActionResult> InBangDiem()
        {
            var (isAuthenticated, viewModel) = await LoadKetQuaCuaToiAsync();
            if (!isAuthenticated)
                return RedirectToAction("Login", "TaiKhoan");

            if (viewModel == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ học viên tương ứng với tài khoản này.";
                return RedirectToAction("Index", "Home");
            }

            return View(viewModel);
        }

        private async Task<(bool isAuthenticated, KetQuaCuaToiViewModel? viewModel)> LoadKetQuaCuaToiAsync()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!maTaiKhoan.HasValue)
                return (false, null);

            var vaiTro = HttpContext.Session.GetString("VaiTro");
            var viewModel = await _ketQuaRepo.GetKetQuaCuaToiAsync(maTaiKhoan.Value, vaiTro);
            return (true, viewModel);
        }
    }
}
