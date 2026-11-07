// Họ và tên: Nguyễn Văn Mạnh
// Mã sinh viên: 23103100096
// Nội dung thực hiện: Module 2 - Quản lý khóa học (Sử dụng Repository Pattern)

using Microsoft.AspNetCore.Mvc;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    public class KhoaHocController : Controller
    {
        private readonly IKhoaHocRepository _khoaHocRepo;
        private readonly ILogger<KhoaHocController> _logger;

        public KhoaHocController(IKhoaHocRepository khoaHocRepo, ILogger<KhoaHocController> logger)
        {
            _khoaHocRepo = khoaHocRepo;
            _logger = logger;
        }

        // GET: /KhoaHoc or /KhoaHoc/Index
        // Cổng tra cứu khóa học (tìm kiếm + lọc + sắp xếp + phân trang)
        [HttpGet]
        public async Task<IActionResult> Index(KhoaHocFilterViewModel filter, int? page = null)
        {
            var (model, monHocList) = await _khoaHocRepo.GetFilterViewModelAsync(filter, page);
            ViewBag.MonHocList = monHocList;

            // Phản hồi AJAX khi tìm kiếm và lọc tức thì
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_KhoaHocGrid", model);

            return View(model);
        }

        // GET: /KhoaHoc/QuanLy
        // Trang quản trị khóa học (tìm kiếm + lọc đa tiêu chí + sắp xếp + phân trang)
        [HttpGet]
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> QuanLy(
            string? searchString, int? maMonHoc, string? trangThai, string? hinhThuc,
            string? khoangHocPhi, string? tinhTrangCho,
            string? sortBy = "id_desc", int page = 1, int pageSize = 10)
        {
            var result = await _khoaHocRepo.GetQuanLyDataAsync(
                searchString, maMonHoc, trangThai, hinhThuc, khoangHocPhi, tinhTrangCho, sortBy, page, pageSize);

            // Thống kê Stat Cards
            ViewBag.TotalCount = result.TotalCount;
            ViewBag.DangMoCount = result.DangMoCount;
            ViewBag.DangHocCount = result.DangHocCount;
            ViewBag.DaKetThucCount = result.DaKetThucCount;
            ViewBag.AvgHocPhi = result.AvgHocPhi;
            ViewBag.MonHocList = result.MonHocList;

            // Lưu trạng thái tìm kiếm và bộ lọc
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentMaMonHoc = maMonHoc;
            ViewBag.CurrentTrangThai = trangThai;
            ViewBag.CurrentHinhThuc = hinhThuc;
            ViewBag.CurrentKhoangHocPhi = khoangHocPhi;
            ViewBag.CurrentTinhTrangCho = tinhTrangCho;
            ViewBag.CurrentSortBy = sortBy;

            // Thông tin phân trang: Đảm bảo PageIndex không vượt quá TotalPages khi kết hợp bộ lọc
            int clampedPage = page;
            if (result.TotalPages > 0 && clampedPage > result.TotalPages)
                clampedPage = result.TotalPages;
            if (clampedPage < 1)
                clampedPage = 1;

            ViewBag.PageIndex = clampedPage;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = result.TotalFiltered;
            ViewBag.TotalPages = result.TotalPages;

            return View(result.Items);
        }

        // GET: /KhoaHoc/Details/5
        // Chi tiết khóa học kèm thông tin môn học, giảng viên và danh sách đăng ký
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var khoaHoc = await _khoaHocRepo.GetByIdWithDetailsAsync(id.Value);
            if (khoaHoc == null) return NotFound();

            // Kiểm tra học viên đang đăng nhập đã đăng ký khóa học này chưa
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            ViewBag.DaDangKy = maTaiKhoan.HasValue
                && await _khoaHocRepo.CheckHocVienDaDangKyAsync(id.Value, maTaiKhoan.Value);

            return View(khoaHoc);
        }

        // GET: /KhoaHoc/Create
        [HttpGet]
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> Create()
        {
            var (monHocList, giangVienList) = await _khoaHocRepo.GetDropdownDataAsync();
            ViewBag.MaMonHoc = monHocList;
            ViewBag.MaGiangVien = giangVienList;

            return View(new KhoaHoc
            {
                NgayBatDau = DateTime.Today,
                NgayKetThuc = DateTime.Today.AddMonths(3),
                SoLuongToiDa = 30,
                HocPhi = 3000000,
                HinhThuc = "Trực tiếp",
                TrangThai = "DangMo"
            });
        }

        // POST: /KhoaHoc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> Create(KhoaHoc khoaHoc)
        {
            if (khoaHoc.NgayKetThuc <= khoaHoc.NgayBatDau)
                ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải sau ngày bắt đầu khóa học.");

            if (ModelState.IsValid)
            {
                var (success, errorMessage, _) = await _khoaHocRepo.CreateKhoaHocAsync(khoaHoc);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Đã thêm mới khóa học '{khoaHoc.TenKhoaHoc}' thành công!";
                    return RedirectToAction(nameof(QuanLy));
                }
                ModelState.AddModelError(string.Empty, errorMessage ?? "Không thể thêm mới khóa học.");
            }

            // Tải lại dropdown khi thêm mới không thành công
            var (monHocList, giangVienList) = await _khoaHocRepo.GetDropdownDataAsync(khoaHoc.MaMonHoc, khoaHoc.MaGiangVien);
            ViewBag.MaMonHoc = monHocList;
            ViewBag.MaGiangVien = giangVienList;
            return View(khoaHoc);
        }

        // GET: /KhoaHoc/Edit/5
        [HttpGet]
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var khoaHoc = await _khoaHocRepo.GetByIdAsync(id.Value);
            if (khoaHoc == null) return NotFound();

            var (monHocList, giangVienList) = await _khoaHocRepo.GetDropdownDataAsync(khoaHoc.MaMonHoc, khoaHoc.MaGiangVien);
            ViewBag.MaMonHoc = monHocList;
            ViewBag.MaGiangVien = giangVienList;

            return View(khoaHoc);
        }

        // POST: /KhoaHoc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> Edit(int id, KhoaHoc khoaHoc)
        {
            if (id != khoaHoc.MaKhoaHoc) return NotFound();

            if (khoaHoc.NgayKetThuc <= khoaHoc.NgayBatDau)
                ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải sau ngày bắt đầu khóa học.");

            if (ModelState.IsValid)
            {
                var (success, errorMessage) = await _khoaHocRepo.UpdateKhoaHocAsync(khoaHoc);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Cập nhật khóa học '{khoaHoc.TenKhoaHoc}' thành công!";
                    return RedirectToAction(nameof(QuanLy));
                }
                ModelState.AddModelError(string.Empty, errorMessage ?? "Không thể cập nhật khóa học.");
            }

            // Tải lại dropdown khi cập nhật không thành công
            var (monHocList, giangVienList) = await _khoaHocRepo.GetDropdownDataAsync(khoaHoc.MaMonHoc, khoaHoc.MaGiangVien);
            ViewBag.MaMonHoc = monHocList;
            ViewBag.MaGiangVien = giangVienList;
            return View(khoaHoc);
        }

        // GET: /KhoaHoc/Delete/5
        // Quyền: Chỉ Admin
        [HttpGet]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var khoaHoc = await _khoaHocRepo.GetByIdForDeleteAsync(id.Value);
            if (khoaHoc == null) return NotFound();

            ViewBag.HasDangKy = await _khoaHocRepo.HasDangKyAsync(id.Value);
            return View(khoaHoc);
        }

        // POST: /KhoaHoc/Delete/5
        // Quyền: Chỉ Admin
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var (success, errorMessage) = await _khoaHocRepo.DeleteKhoaHocAsync(id);

            if (success)
                TempData["SuccessMessage"] = "Đã xóa khóa học thành công.";
            else
                TempData["ErrorMessage"] = errorMessage ?? "Không thể xóa khóa học.";

            return RedirectToAction(nameof(QuanLy));
        }

        // POST: /KhoaHoc/UpdateStatus/5
        // Cập nhật nhanh trạng thái vòng đời khóa học
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            var (success, errorMessage, statusLabel) = await _khoaHocRepo.UpdateStatusAsync(id, newStatus);

            if (success)
                TempData["SuccessMessage"] = $"Đã chuyển trạng thái khóa học sang '{statusLabel}'.";
            else
                TempData["ErrorMessage"] = errorMessage ?? "Không thể cập nhật trạng thái.";

            return RedirectToAction(nameof(QuanLy));
        }

        // Định dạng nhãn trạng thái hiển thị
        public static string GetStatusLabel(string? trangThai)
        {
            return KhoaHocRepository.FormatStatusLabel(trangThai);
        }
    }
}
