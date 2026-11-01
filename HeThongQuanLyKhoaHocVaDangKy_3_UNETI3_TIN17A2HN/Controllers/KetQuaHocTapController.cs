// Họ và tên: Nguyễn Công Thành
// Mã sinh viên: 23103100102
// Nội dung thực hiện: Module 5 - Quản lý kết quả học tập, bảng điểm và nhập điểm hàng loạt

using Microsoft.AspNetCore.Mvc;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    public class KetQuaHocTapController : Controller
    {
        private readonly IKetQuaHocTapRepository _ketQuaRepo;
        private readonly ILogger<KetQuaHocTapController> _logger;

        public KetQuaHocTapController(IKetQuaHocTapRepository ketQuaRepo, ILogger<KetQuaHocTapController> logger)
        {
            _ketQuaRepo = ketQuaRepo;
            _logger = logger;
        }

        // 1. Danh sách khóa học để nhập điểm (Dành cho Admin & Nhân viên)
        [AuthorizeRole("Admin", "NhanVien")]
        [HttpGet]
        public async Task<IActionResult> Index(KetQuaFilterViewModel filter)
        {
            var (filterResult, monHocList) = await _ketQuaRepo.GetKetQuaFilterAsync(filter);
            ViewBag.MonHocList = monHocList;
            return View(filterResult);
        }

        // 2. Bảng điểm chi tiết & Form nhập điểm hàng loạt (Admin & Nhân viên)
        [AuthorizeRole("Admin", "NhanVien")]
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
        [AuthorizeRole("Admin", "NhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LuuDiem(BangDiemKhoaHocViewModel model)
        {
            var (success, countSaved, errorMessage) = await _ketQuaRepo.LuuDiemAsync(model);
            if (success)
            {
                TempData["SuccessMessage"] = $"Lưu bảng điểm thành công cho toàn bộ {countSaved} học viên!";
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage ?? "Lỗi khi lưu bảng điểm vào cơ sở dữ liệu.";
            }

            return RedirectToAction(nameof(BangDiem), new { maKhoaHoc = model.MaKhoaHoc });
        }

        // 4. Kết quả học tập của tôi (Dành riêng cho Học viên xem điểm cá nhân)
        [AuthorizeRole("HocVien", "Admin", "NhanVien")]
        [HttpGet]
        public async Task<IActionResult> KetQuaCuaToi()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!maTaiKhoan.HasValue)
            {
                return RedirectToAction("Login", "TaiKhoan");
            }

            var vaiTro = HttpContext.Session.GetString("VaiTro");
            var viewModel = await _ketQuaRepo.GetKetQuaCuaToiAsync(maTaiKhoan.Value, vaiTro);

            if (viewModel == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ học viên tương ứng với tài khoản này.";
                return RedirectToAction("Index", "Home");
            }

            return View(viewModel);
        }
    }
}
