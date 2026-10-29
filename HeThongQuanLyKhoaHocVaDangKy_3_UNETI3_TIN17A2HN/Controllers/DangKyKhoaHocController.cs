// Họ và tên: Ngọc Tùng Lâm
// Mã sinh viên: 23103100098
// Nội dung thực hiện: Module 3 - Đăng ký Khóa học & Theo dõi Đăng ký (Sử dụng Repository Pattern)

using Microsoft.AspNetCore.Mvc;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    public class DangKyKhoaHocController : Controller
    {
        private readonly IDangKyKhoaHocRepository _dangKyRepo;
        private readonly ILogger<DangKyKhoaHocController> _logger;

        public DangKyKhoaHocController(IDangKyKhoaHocRepository dangKyRepo, ILogger<DangKyKhoaHocController> logger)
        {
            _dangKyRepo = dangKyRepo;
            _logger = logger;
        }

        // GET: /DangKyKhoaHoc/DangKy?maKhoaHoc=5
        [HttpGet]
        public async Task<IActionResult> DangKy(int? maKhoaHoc)
        {
            if (maKhoaHoc == null)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn một khóa học cụ thể để tiến hành đăng ký.";
                return RedirectToAction("Index", "KhoaHoc");
            }

            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!maTaiKhoan.HasValue)
            {
                return RedirectToAction("Login", "TaiKhoan", new { returnUrl = Url.Action(nameof(DangKy), new { maKhoaHoc }) });
            }

            var (model, error) = await _dangKyRepo.GetDangKyFormAsync(maKhoaHoc.Value, maTaiKhoan.Value);
            if (model == null)
            {
                TempData["ErrorMessage"] = error;
                return RedirectToAction("Index", "KhoaHoc");
            }

            return View(model);
        }

        // POST: /DangKyKhoaHoc/DangKy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(DangKyKhoaHocViewModel model)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!maTaiKhoan.HasValue)
            {
                return RedirectToAction("Login", "TaiKhoan", new { returnUrl = Url.Action(nameof(DangKy), new { maKhoaHoc = model.MaKhoaHoc }) });
            }

            if (!model.DongYQuyChe)
            {
                ModelState.AddModelError(nameof(model.DongYQuyChe), "Bạn phải đồng ý với quy chế đào tạo để tiếp tục đăng ký.");
            }

            if (ModelState.IsValid)
            {
                var (success, error, maDangKy) = await _dangKyRepo.XuLyDangKyAsync(model, maTaiKhoan.Value);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Đăng ký khóa học '{model.TenKhoaHoc}' thành công! Mã giao dịch của bạn là #{maDangKy}. Đơn của bạn đang ở trạng thái 'Chờ xử lý'.";
                    return RedirectToAction(nameof(ChiTiet), new { id = maDangKy });
                }

                ModelState.AddModelError(string.Empty, error ?? "Có lỗi xảy ra khi xử lý đăng ký.");
            }

            return View(model);
        }

        // GET: /DangKyKhoaHoc/Index (Theo dõi đăng ký cá nhân)
        [HttpGet]
        [AuthorizeRole("HocVien", "Admin", "NhanVien")]
        public async Task<IActionResult> Index(TheoDoiDangKyFilterViewModel filter)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!maTaiKhoan.HasValue)
            {
                return RedirectToAction("Login", "TaiKhoan", new { returnUrl = Url.Action(nameof(Index)) });
            }

            var result = await _dangKyRepo.GetTheoDoiFilterAsync(maTaiKhoan.Value, filter);
            return View(result);
        }

        // GET: /DangKyKhoaHoc/ChiTiet/5
        [HttpGet]
        [AuthorizeRole("HocVien", "Admin", "NhanVien")]
        public async Task<IActionResult> ChiTiet(int? id)
        {
            if (id == null) return NotFound();

            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!maTaiKhoan.HasValue) return RedirectToAction("Login", "TaiKhoan");

            bool isStaffOrAdmin = vaiTro == VaiTro.Admin || vaiTro == VaiTro.NhanVien;
            var (dangKy, error) = await _dangKyRepo.GetChiTietDangKyAsync(id.Value, maTaiKhoan.Value, isStaffOrAdmin);

            if (dangKy == null)
            {
                TempData["ErrorMessage"] = error;
                return RedirectToAction(nameof(Index));
            }

            return View(dangKy);
        }

        // POST: /DangKyKhoaHoc/HuyDangKy/5
        [HttpPost]
        [AuthorizeRole("HocVien", "Admin", "NhanVien")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDangKy(int id, string? lyDoHuy)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!maTaiKhoan.HasValue) return RedirectToAction("Login", "TaiKhoan");

            bool isStaffOrAdmin = vaiTro == VaiTro.Admin || vaiTro == VaiTro.NhanVien;
            var (success, error) = await _dangKyRepo.HuyDangKyAsync(id, lyDoHuy, maTaiKhoan.Value, isStaffOrAdmin);

            if (!success)
            {
                TempData["ErrorMessage"] = error;
                return RedirectToAction(nameof(ChiTiet), new { id });
            }

            TempData["SuccessMessage"] = $"Đã hủy đơn đăng ký #{id} thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /DangKyKhoaHoc/InPhieu/5
        [HttpGet]
        [AuthorizeRole("HocVien", "Admin", "NhanVien")]
        public async Task<IActionResult> InPhieu(int? id)
        {
            if (id == null) return NotFound();

            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!maTaiKhoan.HasValue) return RedirectToAction("Login", "TaiKhoan");

            bool isStaffOrAdmin = vaiTro == VaiTro.Admin || vaiTro == VaiTro.NhanVien;
            var (dangKy, error) = await _dangKyRepo.GetPhieuDangKyAsync(id.Value, maTaiKhoan.Value, isStaffOrAdmin);

            if (dangKy == null)
            {
                TempData["ErrorMessage"] = error;
                return RedirectToAction(nameof(Index));
            }

            return View(dangKy);
        }
    }
}
