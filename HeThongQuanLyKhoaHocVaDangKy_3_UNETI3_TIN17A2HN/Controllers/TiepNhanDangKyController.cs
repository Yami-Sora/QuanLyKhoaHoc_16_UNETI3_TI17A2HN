// Họ và tên: Bạch Đức Sự
// Mã sinh viên: 23103100063
// Nội dung thực hiện: Module 4 - Tiếp nhận đăng ký, Xác nhận/Hủy đăng ký và Quản lý trạng thái.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Helpers;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    [AuthorizeRole("Admin", "NhanVien")]
    public class TiepNhanDangKyController : Controller
    {
        private readonly TiepNhanDangKyService _service;

        public TiepNhanDangKyController(ApplicationDbContext context)
        {
            _service = new TiepNhanDangKyService(context);
        }

        [HttpGet]
        public async Task<IActionResult> Index(TiepNhanFilterViewModel filter)
        {
            var result = await _service.GetDanhSachDangKyAsync(filter);
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var model = await _service.GetChiTietDangKyAsync(id.Value);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ đăng ký khóa học này.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CapNhatTrangThai(int? id)
        {
            if (id == null) return NotFound();

            var viewModel = await _service.GetCapNhatTrangThaiViewModelAsync(id.Value);
            if (viewModel == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin đơn đăng ký cần cập nhật.";
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(CapNhatTrangThaiViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var refreshed = await _service.GetCapNhatTrangThaiViewModelAsync(model.MaDangKy);
                if (refreshed != null)
                {
                    model.HoTenHocVien = refreshed.HoTenHocVien;
                    model.SoDienThoai = refreshed.SoDienThoai;
                    model.Email = refreshed.Email;
                    model.TenKhoaHoc = refreshed.TenKhoaHoc;
                    model.TrangThaiKhoaHoc = refreshed.TrangThaiKhoaHoc;
                    model.HocPhi = refreshed.HocPhi;
                    model.SiSoHienTai = refreshed.SiSoHienTai;
                    model.SoLuongToiDa = refreshed.SoLuongToiDa;
                    model.NgayDangKy = refreshed.NgayDangKy;
                    model.TrangThaiHienTai = refreshed.TrangThaiHienTai;
                    model.TrangThaiHopLe = refreshed.TrangThaiHopLe;
                }
                return View(model);
            }

            var result = await _service.CapNhatTrangThaiAsync(model);
            if (!result.ThanhCong)
            {
                ModelState.AddModelError(string.Empty, result.ThongBao);
                var refreshed = await _service.GetCapNhatTrangThaiViewModelAsync(model.MaDangKy);
                if (refreshed != null)
                {
                    model.HoTenHocVien = refreshed.HoTenHocVien;
                    model.SoDienThoai = refreshed.SoDienThoai;
                    model.Email = refreshed.Email;
                    model.TenKhoaHoc = refreshed.TenKhoaHoc;
                    model.TrangThaiKhoaHoc = refreshed.TrangThaiKhoaHoc;
                    model.HocPhi = refreshed.HocPhi;
                    model.SiSoHienTai = refreshed.SiSoHienTai;
                    model.SoLuongToiDa = refreshed.SoLuongToiDa;
                    model.NgayDangKy = refreshed.NgayDangKy;
                    model.TrangThaiHienTai = refreshed.TrangThaiHienTai;
                    model.TrangThaiHopLe = refreshed.TrangThaiHopLe;
                }
                return View(model);
            }

            TempData["SuccessMessage"] = result.ThongBao;
            return RedirectToAction(nameof(Details), new { id = model.MaDangKy });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TiepNhanNhanh(int id)
        {
            var result = await _service.TiepNhanNhanhAsync(id);
            if (result.ThanhCong)
            {
                TempData["SuccessMessage"] = result.ThongBao;
            }
            else
            {
                TempData["ErrorMessage"] = result.ThongBao;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanHoanThanh(int id, decimal? soTienDong, string? ghiChuXacNhan)
        {
            var result = await _service.XacNhanHoanThanhAsync(id, soTienDong, ghiChuXacNhan);
            if (result.ThanhCong)
            {
                TempData["SuccessMessage"] = result.ThongBao;
            }
            else
            {
                TempData["ErrorMessage"] = result.ThongBao;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDangKy(int id, string lyDoHuy)
        {
            var result = await _service.HuyDangKyAsync(id, lyDoHuy);
            if (result.ThanhCong)
            {
                TempData["SuccessMessage"] = result.ThongBao;
            }
            else
            {
                TempData["ErrorMessage"] = result.ThongBao;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await NapDuLieuDropDownList();
            return View(new TaoDangKyMoiViewModel { TrangThai = "ChoXuLy", SoTienDaDong = 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaoDangKyMoiViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _service.TaoDangKyMoiAsync(model);
                if (result.ThanhCong)
                {
                    TempData["SuccessMessage"] = result.ThongBao;
                    return RedirectToAction(nameof(Details), new { id = result.MaDangKyMoi });
                }

                ModelState.AddModelError(string.Empty, result.ThongBao);
            }

            await NapDuLieuDropDownList();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> InPhieuDangKy(int? id)
        {
            if (id == null) return NotFound();

            var dangKy = await _service.GetDangKyForPrintAsync(id.Value);
            if (dangKy == null) return NotFound();

            return View(dangKy);
        }

        private async Task NapDuLieuDropDownList()
        {
            var (hocViens, khoaHocs) = await _service.LayDuLieuDropDownListAsync();
            ViewBag.HocViens = new SelectList(hocViens, "MaHocVien", "ThongTin");
            ViewBag.KhoaHocs = new SelectList(khoaHocs, "MaKhoaHoc", "ThongTin");
        }
    }
}
