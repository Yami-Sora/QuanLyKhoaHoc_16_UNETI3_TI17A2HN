// Họ và tên: Bạch Đức Sự
// Mã sinh viên: 23103100063
// Nội dung thực hiện: Tiếp nhận đăng ký, xử lý trạng thái bằng Repository Pattern, đồng bộ UI.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    [AuthorizeRole("Admin", "NhanVien")]
    public class TiepNhanDangKyController : Controller
    {
        private readonly ITiepNhanDangKyRepository _repo;
        private readonly ILogger<TiepNhanDangKyController> _logger;

        // Constructor Injection Repository Pattern - Loại bỏ hoàn toàn DbContext trực tiếp
        public TiepNhanDangKyController(ITiepNhanDangKyRepository repo, ILogger<TiepNhanDangKyController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        // GET: /TiepNhanDangKy (Danh sách, tìm kiếm, lọc đa tiêu chí, phân trang)
        [HttpGet]
        public async Task<IActionResult> Index(TiepNhanFilterViewModel filter)
        {
            var result = await _repo.GetDanhSachDangKyAsync(filter);
            return View(result);
        }

        // GET: /TiepNhanDangKy/Details/5 (Chi tiết hồ sơ, sĩ số, lịch sử)
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var model = await _repo.GetChiTietDangKyAsync(id.Value);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ đăng ký khóa học này.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: /TiepNhanDangKy/CapNhatTrangThai/5
        [HttpGet]
        public async Task<IActionResult> CapNhatTrangThai(int? id)
        {
            if (id == null) return NotFound();

            var viewModel = await _repo.GetCapNhatTrangThaiViewModelAsync(id.Value);
            if (viewModel == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin đơn đăng ký cần cập nhật.";
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // POST: /TiepNhanDangKy/CapNhatTrangThai
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(CapNhatTrangThaiViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var refreshed = await _repo.GetCapNhatTrangThaiViewModelAsync(model.MaDangKy);
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

            var result = await _repo.CapNhatTrangThaiAsync(model);
            if (!result.ThanhCong)
            {
                ModelState.AddModelError(string.Empty, result.ThongBao);
                var refreshed = await _repo.GetCapNhatTrangThaiViewModelAsync(model.MaDangKy);
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

        // POST: /TiepNhanDangKy/TiepNhanNhanh/5 (Chuyển nhanh từ Chờ xử lý -> Đang xử lý)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TiepNhanNhanh(int id)
        {
            var result = await _repo.TiepNhanNhanhAsync(id);
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

        // POST: /TiepNhanDangKy/XacNhanHoanThanh/5 (Chuyển sang Hoàn thành & Ghi nhận tiền)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanHoanThanh(int id, decimal? soTienDong, string? ghiChuXacNhan)
        {
            var result = await _repo.XacNhanHoanThanhAsync(id, soTienDong, ghiChuXacNhan);
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

        // POST: /TiepNhanDangKy/HuyDangKy/5 (Hủy/Từ chối kèm lý do)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDangKy(int id, string lyDoHuy)
        {
            var result = await _repo.HuyDangKyAsync(id, lyDoHuy);
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

        // GET: /TiepNhanDangKy/Create (Ghi danh tại quầy)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await NapDuLieuDropDownList();
            return View(new TaoDangKyMoiViewModel { TrangThai = "ChoXuLy", SoTienDaDong = 0 });
        }

        // POST: /TiepNhanDangKy/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaoDangKyMoiViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _repo.TaoDangKyMoiAsync(model);
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

        // GET: /TiepNhanDangKy/InPhieuDangKy/5 (Trang in biểu mẫu chính thức)
        [HttpGet]
        public async Task<IActionResult> InPhieuDangKy(int? id)
        {
            if (id == null) return NotFound();

            var dangKy = await _repo.GetDangKyForPrintAsync(id.Value);
            if (dangKy == null) return NotFound();

            return View(dangKy);
        }

        private async Task NapDuLieuDropDownList()
        {
            var (hocViens, khoaHocs) = await _repo.LayDuLieuDropDownListAsync();
            ViewBag.HocViens = new SelectList(hocViens, "MaHocVien", "ThongTin");
            ViewBag.KhoaHocs = new SelectList(khoaHocs, "MaKhoaHoc", "ThongTin");
        }
    }
}
