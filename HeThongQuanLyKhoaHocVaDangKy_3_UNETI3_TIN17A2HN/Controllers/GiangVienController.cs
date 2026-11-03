// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Module 1: Quản lý giảng viên - dữ liệu nền (Sử dụng Repository Pattern)

using System.Text;
using Microsoft.AspNetCore.Mvc;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Helpers;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    public class GiangVienController : Controller
    {
        private readonly IGiangVienRepository _giangVienRepo;
        private readonly ILogger<GiangVienController> _logger;

        public GiangVienController(IGiangVienRepository giangVienRepo, ILogger<GiangVienController> logger)
        {
            _giangVienRepo = giangVienRepo;
            _logger = logger;
        }

        // GET: /GiangVien
        // Danh sách giảng viên (tìm kiếm + lọc trạng thái + phân trang)
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        public async Task<IActionResult> Index(string? searchString, bool? trangThai, bool? coKhoaHoc, int page = 1, int pageSize = 10)
        {
            var (list, totalCount, activeCount, inactiveCount, totalKhoaHoc, totalFiltered, totalPages) =
                await _giangVienRepo.GetListAsync(searchString, trangThai, coKhoaHoc, page, pageSize);

            // Thống kê số liệu cho thẻ Stat Cards
            ViewBag.TotalCount = totalCount;
            ViewBag.ActiveCount = activeCount;
            ViewBag.InactiveCount = inactiveCount;
            ViewBag.TotalKhoaHoc = totalKhoaHoc;
            ViewBag.TotalFilteredItems = totalFiltered;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            if (!string.IsNullOrWhiteSpace(searchString))
                ViewBag.CurrentSearch = searchString;
            if (trangThai.HasValue)
                ViewBag.CurrentTrangThai = trangThai;
            if (coKhoaHoc == true)
                ViewBag.CurrentCoKhoaHoc = true;

            return View(list);
        }

        // GET: /GiangVien/XuatDanhSach
        // Xuất danh sách giảng viên (Excel chuẩn .xlsx, thương hiệu UNETI, tự động co giãn cột, kẻ viền và bộ lọc)
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        public async Task<IActionResult> XuatDanhSach(string? searchString, bool? trangThai, bool? coKhoaHoc)
        {
            var list = await _giangVienRepo.GetExportListAsync(searchString, trangThai, coKhoaHoc);

            var headers = new[]
            {
                "STT", "Mã giảng viên", "Họ và tên", "Email", "Số điện thoại", "Học vị", "Chuyên môn", "Số khóa phụ trách", "Trạng thái"
            };

            int stt = 1;
            var rows = list.Select(g => new object?[]
            {
                stt++,
                $"GV{g.MaGiangVien:D2}",
                g.HoTen,
                g.Email,
                string.IsNullOrWhiteSpace(g.SoDienThoai) ? "--" : g.SoDienThoai,
                string.IsNullOrWhiteSpace(g.HocVi) ? "--" : g.HocVi,
                string.IsNullOrWhiteSpace(g.ChuyenMon) ? "--" : g.ChuyenMon,
                g.KhoaHocs?.Count ?? 0,
                g.TrangThai ? "Đang giảng dạy" : "Tạm nghỉ"
            });

            var bytes = ExportHelper.ToExcelBytes("Danh sách giảng viên", headers, rows);
            return File(bytes, ExportHelper.ExcelContentType, "DanhSachGiangVien.xlsx");
        }

        // GET: /GiangVien/Create
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        public IActionResult Create()
        {
            return View(new GiangVien { TrangThai = true, GioiTinh = "Nam" });
        }

        // POST: /GiangVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        public async Task<IActionResult> Create(GiangVien giangVien)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                          (Request.Headers.Accept.ToString()?.Contains("application/json") ?? false);

            if (ModelState.IsValid)
            {
                // Kiểm tra trùng email giảng viên
                if (await _giangVienRepo.ExistsEmailAsync(giangVien.Email ?? ""))
                {
                    var emailError = "Email này đã được sử dụng cho giảng viên khác. Vui lòng nhập email khác.";
                    if (isAjax) return Json(new { success = false, message = emailError });
                    ModelState.AddModelError("Email", emailError);
                    return View(giangVien);
                }

                var (success, error) = await _giangVienRepo.CreateAsync(giangVien);
                if (success)
                {
                    if (isAjax)
                    {
                        return Json(new { success = true, message = $"Đã thêm giảng viên '{giangVien.HoTen}' thành công!" });
                    }
                    TempData["SuccessMessage"] = $"Đã thêm giảng viên '{giangVien.HoTen}' thành công!";
                    return RedirectToAction(nameof(Index));
                }

                if (isAjax) return Json(new { success = false, message = error ?? "Lỗi không xác định khi thêm giảng viên." });
                ModelState.AddModelError(string.Empty, error ?? "Lỗi không xác định.");
            }
            else
            {
                if (isAjax)
                {
                    var firstError = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .FirstOrDefault(msg => !string.IsNullOrWhiteSpace(msg))
                        ?? "Thông tin nhập không hợp lệ.";
                    return Json(new { success = false, message = firstError });
                }
            }
            return View(giangVien);
        }

        // GET: /GiangVien/Edit/5
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var giangVien = await _giangVienRepo.GetByIdAsync(id.Value);
            if (giangVien == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin giảng viên." });
                }
                return NotFound();
            }

            var (hasActive, activeCourses) = await _giangVienRepo.GetActiveTeachingCoursesAsync(id.Value);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = true,
                    data = giangVien,
                    hasActiveCourses = hasActive,
                    activeCourseCount = activeCourses.Count,
                    activeCourses = activeCourses
                });
            }

            ViewBag.HasActiveCourses = hasActive;
            ViewBag.ActiveCourseCount = activeCourses.Count;
            ViewBag.ActiveCourses = activeCourses;

            return View(giangVien);
        }

        // POST: /GiangVien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        public async Task<IActionResult> Edit(int id, GiangVien giangVien)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                          (Request.Headers.Accept.ToString()?.Contains("application/json") ?? false);

            if (id != giangVien.MaGiangVien)
            {
                if (isAjax) return Json(new { success = false, message = "Dữ liệu yêu cầu không hợp lệ." });
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Kiểm tra trùng email với giảng viên khác
                if (await _giangVienRepo.ExistsEmailAsync(giangVien.Email ?? "", id))
                {
                    var emailError = "Email này đã được sử dụng cho giảng viên khác. Vui lòng nhập email khác.";
                    if (isAjax) return Json(new { success = false, message = emailError });
                    ModelState.AddModelError("Email", emailError);
                    return View(giangVien);
                }

                var (success, error) = await _giangVienRepo.UpdateAsync(id, giangVien);
                if (success)
                {
                    if (isAjax)
                    {
                        return Json(new { success = true, message = $"Cập nhật giảng viên '{giangVien.HoTen}' thành công!" });
                    }
                    TempData["SuccessMessage"] = $"Cập nhật giảng viên '{giangVien.HoTen}' thành công!";
                    return RedirectToAction(nameof(Index));
                }

                if (isAjax) return Json(new { success = false, message = error ?? "Lỗi không xác định khi cập nhật giảng viên." });
                ModelState.AddModelError(string.Empty, error ?? "Lỗi không xác định.");
            }
            else
            {
                if (isAjax)
                {
                    var firstError = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .FirstOrDefault(msg => !string.IsNullOrWhiteSpace(msg))
                        ?? "Thông tin nhập không hợp lệ.";
                    return Json(new { success = false, message = firstError });
                }
            }
            return View(giangVien);
        }

        // GET: /GiangVien/Delete/5
        // Quyền: Chỉ Admin
        [AuthorizeRole(VaiTro.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var giangVien = await _giangVienRepo.GetByIdWithKhoaHocsAsync(id.Value);
            if (giangVien == null) return NotFound();

            // Cho view biết giảng viên có đang phụ trách khóa học nào không
            ViewBag.HasKhoaHoc = giangVien.KhoaHocs.Any();
            ViewBag.SoKhoaHoc = giangVien.KhoaHocs.Count;
            return View(giangVien);
        }

        // POST: /GiangVien/Delete/5
        // Quyền: Chỉ Admin
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(VaiTro.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                          (Request.Headers.Accept.ToString()?.Contains("application/json") ?? false);

            var (success, message) = await _giangVienRepo.DeleteAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = message;
                if (isAjax) return Json(new { success = true, message });
            }
            else
            {
                TempData["ErrorMessage"] = message;
                if (isAjax) return Json(new { success = false, message });
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /GiangVien/ToggleStatus/5
        // Đổi nhanh trạng thái Đang giảng dạy / Tạm nghỉ
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var (success, message, newStatus) = await _giangVienRepo.ToggleStatusAsync(id);

            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                          (Request.Headers.Accept.ToString()?.Contains("application/json") ?? false);

            if (isAjax)
            {
                return Json(new { success, message, newStatus });
            }

            if (success)
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
