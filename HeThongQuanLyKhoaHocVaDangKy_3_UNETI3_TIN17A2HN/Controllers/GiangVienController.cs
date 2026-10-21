// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Module 1: Quản lý giảng viên (dữ liệu nền)

using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    public class GiangVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GiangVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /GiangVien
        // Danh sách giảng viên (tìm kiếm + lọc trạng thái + phân trang)
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        public async Task<IActionResult> Index(string? searchString, bool? trangThai, bool? coKhoaHoc, int page = 1, int pageSize = 10)
        {
            // Thống kê số liệu cho thẻ Stat Cards
            ViewBag.TotalCount = await _context.GiangViens.CountAsync();
            ViewBag.ActiveCount = await _context.GiangViens.CountAsync(g => g.TrangThai);
            ViewBag.InactiveCount = await _context.GiangViens.CountAsync(g => !g.TrangThai);
            ViewBag.TotalKhoaHoc = await _context.KhoaHocs.CountAsync();

            var query = _context.GiangViens.Include(g => g.KhoaHocs).AsQueryable();

            // Tìm kiếm theo họ tên, email, số điện thoại, chuyên môn hoặc mã giảng viên
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.Trim().ToLower();
                var digits = new string(term.Where(char.IsDigit).ToArray());
                bool isNumeric = int.TryParse(digits, out int searchId);

                query = query.Where(g => g.HoTen.ToLower().Contains(term)
                                      || g.Email.ToLower().Contains(term)
                                      || g.SoDienThoai.Contains(term)
                                      || (g.ChuyenMon != null && g.ChuyenMon.ToLower().Contains(term))
                                      || (isNumeric && g.MaGiangVien == searchId));
                ViewBag.CurrentSearch = searchString;
            }

            // Lọc theo trạng thái công tác
            if (trangThai.HasValue)
            {
                query = query.Where(g => g.TrangThai == trangThai.Value);
                ViewBag.CurrentTrangThai = trangThai;
            }

            // Lọc nhanh từ thẻ thống kê "Khóa học phụ trách": chỉ giảng viên đang có lớp
            if (coKhoaHoc == true)
            {
                query = query.Where(g => g.KhoaHocs.Any());
                ViewBag.CurrentCoKhoaHoc = true;
            }

            var totalFiltered = await query.CountAsync();
            ViewBag.TotalFilteredItems = totalFiltered;

            if (page < 1) page = 1;
            var totalPages = (int)Math.Ceiling(totalFiltered / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages) page = totalPages;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            var list = await query
                .OrderByDescending(g => g.MaGiangVien)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return View(list);
        }

        // GET: /GiangVien/XuatDanhSach
        // Xuất danh sách giảng viên (CSV, mở được bằng Excel) theo đúng điều kiện đang lọc
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        public async Task<IActionResult> XuatDanhSach(string? searchString, bool? trangThai, bool? coKhoaHoc)
        {
            var query = _context.GiangViens.Include(g => g.KhoaHocs).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.Trim().ToLower();
                query = query.Where(g => g.HoTen.ToLower().Contains(term)
                                      || g.Email.ToLower().Contains(term)
                                      || g.SoDienThoai.Contains(term)
                                      || (g.ChuyenMon != null && g.ChuyenMon.ToLower().Contains(term)));
            }

            if (trangThai.HasValue)
            {
                query = query.Where(g => g.TrangThai == trangThai.Value);
            }

            if (coKhoaHoc == true)
            {
                query = query.Where(g => g.KhoaHocs.Any());
            }

            var list = await query.OrderBy(g => g.HoTen).ToListAsync();

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
                var emailTrim = giangVien.Email?.Trim().ToLower() ?? string.Empty;
                bool emailExists = await _context.GiangViens
                    .AnyAsync(g => g.Email.Trim().ToLower() == emailTrim);

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng cho giảng viên khác. Vui lòng nhập email khác.");
                    return View(giangVien);
                }

                try
                {
                    giangVien.HoTen = giangVien.HoTen?.Trim() ?? string.Empty;
                    giangVien.Email = giangVien.Email?.Trim() ?? string.Empty;
                    giangVien.SoDienThoai = giangVien.SoDienThoai?.Trim() ?? string.Empty;
                    giangVien.ChuyenMon = giangVien.ChuyenMon?.Trim();
                    _context.Add(giangVien);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã thêm giảng viên '{giangVien.HoTen}' thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Không thể lưu giảng viên do lỗi kết nối CSDL. Vui lòng thử lại sau.");
                }
            }
            return View(giangVien);
        }

        // GET: /GiangVien/Edit/5
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var giangVien = await _context.GiangViens.FindAsync(id);
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
                var emailTrim = giangVien.Email?.Trim().ToLower() ?? string.Empty;
                bool emailExists = await _context.GiangViens
                    .AnyAsync(g => g.MaGiangVien != id && g.Email.Trim().ToLower() == emailTrim);

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng cho giảng viên khác. Vui lòng nhập email khác.");
                    return View(giangVien);
                }

                try
                {
                    var existing = await _context.GiangViens.FindAsync(id);
                    if (existing == null) return NotFound();

                    existing.HoTen = giangVien.HoTen?.Trim() ?? string.Empty;
                    existing.Email = giangVien.Email?.Trim() ?? string.Empty;
                    existing.SoDienThoai = giangVien.SoDienThoai?.Trim() ?? string.Empty;
                    existing.ChuyenMon = giangVien.ChuyenMon?.Trim();
                    existing.HocVi = giangVien.HocVi;
                    existing.GioiTinh = giangVien.GioiTinh;
                    existing.TrangThai = giangVien.TrangThai;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Cập nhật giảng viên '{existing.HoTen}' thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.GiangViens.AnyAsync(e => e.MaGiangVien == giangVien.MaGiangVien))
                    {
                        return NotFound();
                    }
                    throw;
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Không thể cập nhật giảng viên do sự cố CSDL. Vui lòng thử lại.");
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

            var giangVien = await _context.GiangViens
                .Include(g => g.KhoaHocs)
                .FirstOrDefaultAsync(g => g.MaGiangVien == id);

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
            try
            {
                // BẢO VỆ TOÀN VẸN DỮ LIỆU: không xóa giảng viên đang phụ trách khóa học
                bool dangPhuTrach = await _context.KhoaHocs.AnyAsync(k => k.MaGiangVien == id);

                if (dangPhuTrach)
                {
                    TempData["ErrorMessage"] = "Không thể xóa giảng viên này vì đang phụ trách khóa học. Hãy chuyển các khóa học sang giảng viên khác hoặc cho giảng viên nghỉ (Tạm nghỉ) thay vì xóa!";
                    return RedirectToAction(nameof(Index));
                }

                var giangVien = await _context.GiangViens.FindAsync(id);
                if (giangVien != null)
                {
                    _context.GiangViens.Remove(giangVien);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã xóa giảng viên '{giangVien.HoTen}' thành công.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy giảng viên cần xóa hoặc giảng viên đã được xóa trước đó.";
                }
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Không thể xóa giảng viên này vì có dữ liệu liên kết khác trong hệ thống.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Đã xảy ra sự cố khi xóa giảng viên. Vui lòng thử lại sau.";
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
            try
            {
                var giangVien = await _context.GiangViens.FindAsync(id);
                if (giangVien != null)
                {
                    giangVien.TrangThai = !giangVien.TrangThai;
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã đổi trạng thái giảng viên '{giangVien.HoTen}' sang {(giangVien.TrangThai ? "Đang giảng dạy" : "Tạm nghỉ")}.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin giảng viên cần đổi trạng thái.";
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Không thể đổi trạng thái giảng viên do sự cố CSDL.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
