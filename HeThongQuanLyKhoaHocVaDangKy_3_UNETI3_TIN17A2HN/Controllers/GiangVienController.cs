// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Module 1: Quản lý giảng viên - dữ liệu nền (Sử dụng Repository Pattern)

using System.Text;
using Microsoft.AspNetCore.Mvc;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
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
        // Xuất danh sách giảng viên (CSV, mở được bằng Excel) theo đúng điều kiện đang lọc
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        public async Task<IActionResult> XuatDanhSach(string? searchString, bool? trangThai, bool? coKhoaHoc)
        {
            var list = await _giangVienRepo.GetExportListAsync(searchString, trangThai, coKhoaHoc);

            static string O(string? value) => "\"" + (value ?? string.Empty).Replace("\"", "\"\"") + "\"";

            var sb = new StringBuilder();
            sb.AppendLine("MaGiangVien;HoTen;Email;SoDienThoai;HocVi;ChuyenMon;SoKhoaPhuTrach;TrangThai");
            foreach (var g in list)
            {
                sb.AppendLine(string.Join(";",
                    g.MaGiangVien,
                    O(g.HoTen), O(g.Email), O(g.SoDienThoai), O(g.HocVi), O(g.ChuyenMon),
                    g.KhoaHocs?.Count ?? 0,
                    O(g.TrangThai ? "Đang giảng dạy" : "Tạm nghỉ")));
            }

            // BOM UTF-8 để Excel hiển thị đúng tiếng Việt
            var bytes = new UTF8Encoding(true).GetBytes(sb.ToString());
            return File(bytes, "text/csv", "DanhSachGiangVien.csv");
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
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng email giảng viên
                if (await _giangVienRepo.ExistsEmailAsync(giangVien.Email ?? ""))
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng cho giảng viên khác. Vui lòng nhập email khác.");
                    return View(giangVien);
                }

                var (success, error) = await _giangVienRepo.CreateAsync(giangVien);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Đã thêm giảng viên '{giangVien.HoTen}' thành công!";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, error ?? "Lỗi không xác định.");
            }
            return View(giangVien);
        }

        // GET: /GiangVien/Edit/5
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var giangVien = await _giangVienRepo.GetByIdAsync(id.Value);
            if (giangVien == null) return NotFound();

            return View(giangVien);
        }

        // POST: /GiangVien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        public async Task<IActionResult> Edit(int id, GiangVien giangVien)
        {
            if (id != giangVien.MaGiangVien) return NotFound();

            if (ModelState.IsValid)
            {
                // Kiểm tra trùng email với giảng viên khác
                if (await _giangVienRepo.ExistsEmailAsync(giangVien.Email ?? "", id))
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng cho giảng viên khác. Vui lòng nhập email khác.");
                    return View(giangVien);
                }

                var (success, error) = await _giangVienRepo.UpdateAsync(id, giangVien);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Cập nhật giảng viên '{giangVien.HoTen}' thành công!";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, error ?? "Lỗi không xác định.");
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
            var (success, message) = await _giangVienRepo.DeleteAsync(id);

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

        // POST: /GiangVien/ToggleStatus/5
        // Đổi nhanh trạng thái Đang giảng dạy / Tạm nghỉ
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var (success, message, newStatus) = await _giangVienRepo.ToggleStatusAsync(id);

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
