// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Module 1: Quản lý môn học

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    public class MonHocController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MonHocController(ApplicationDbContext context)
        {
            _context = context;
        }
        // 1. CỔNG SINH VIÊN & KHÁCH (PUBLIC PORTAL - CARD GRID)

        // GET: /MonHoc
        // Danh sách môn học công khai (hỗ trợ AJAX)
        public async Task<IActionResult> Index(string? searchString, int? soTinChi, bool? trangThai, string? sortBy = null, int page = 1, int pageSize = 12)
        {
            var query = _context.MonHocs.AsQueryable();

            // Tìm kiếm theo tên môn học hoặc mã môn học
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var trimmed = searchString.Trim();
                var term = trimmed.ToLower();
                var cleanCode = term.Replace("#", "").Replace("mh", "").TrimStart('0');
                bool isNumeric = int.TryParse(cleanCode, out int searchId);

                query = query.Where(m => m.TenMonHoc.Contains(trimmed)
                                      || (isNumeric && m.MaMonHoc == searchId)
                                      || m.MaMonHoc.ToString().Contains(term));
                ViewBag.CurrentSearch = trimmed;
            }

            // Lọc theo số tín chỉ (2, 3, 4, 5 tín chỉ)
            if (soTinChi.HasValue && soTinChi.Value > 0)
            {
                query = query.Where(m => m.SoTinChi == soTinChi.Value);
                ViewBag.CurrentTinChi = soTinChi.Value;
            }

            // Lọc theo trạng thái Đang mở / Tạm dừng
            if (trangThai.HasValue)
            {
                query = query.Where(m => m.TrangThai == trangThai.Value);
                ViewBag.CurrentTrangThai = trangThai;
            }

            // Sắp xếp dữ liệu linh hoạt
            ViewBag.SortBy = sortBy;
            query = sortBy switch
            {
                "ten_asc" => query.OrderBy(m => m.TenMonHoc),
                "ten_desc" => query.OrderByDescending(m => m.TenMonHoc),
                "tinchi_asc" => query.OrderBy(m => m.SoTinChi).ThenBy(m => m.TenMonHoc),
                "tinchi_desc" => query.OrderByDescending(m => m.SoTinChi).ThenBy(m => m.TenMonHoc),
                "hocphi_asc" => query.OrderBy(m => m.HocPhi).ThenBy(m => m.TenMonHoc),
                "hocphi_desc" => query.OrderByDescending(m => m.HocPhi).ThenBy(m => m.TenMonHoc),
                _ => query.OrderBy(m => m.MaMonHoc)
            };

            // Tổng số môn toàn trường và số môn sau khi lọc
            ViewBag.TotalCount = await _context.MonHocs.CountAsync();
            var totalFiltered = await query.CountAsync();
            ViewBag.TotalFilteredItems = totalFiltered;

            // Phân trang
            if (page < 1) page = 1;
            var totalPages = (int)Math.Ceiling(totalFiltered / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages) page = totalPages;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            var list = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Phản hồi AJAX nếu yêu cầu từ phía client
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_MonHocGrid", list);
            }

            return View(list);
        }

        // GET: /MonHoc/Details/5
        // Chi tiết môn học công khai
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var monHoc = await _context.MonHocs
                .Include(m => m.KhoaHocs)
                .FirstOrDefaultAsync(m => m.MaMonHoc == id);

            if (monHoc == null) return NotFound();

            return View(monHoc);
        }
        // 2. TRANG QUẢN TRỊ ADMIN & NHÂN VIÊN ĐÀO TẠO (DATA TABLE CRUD & STATS)

        // GET: /MonHoc/QuanLy
        // Quyền: Chỉ Admin và Nhân viên đào tạo mới được phép truy cập
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> QuanLy(string? searchString, bool? trangThai, int page = 1, int pageSize = 10)
        {
            // Thống kê số liệu tổng thể phục vụ 4 thẻ Stat Cards
            ViewBag.TotalCount = await _context.MonHocs.CountAsync();
            ViewBag.ActiveCount = await _context.MonHocs.CountAsync(m => m.TrangThai);
            ViewBag.InactiveCount = await _context.MonHocs.CountAsync(m => !m.TrangThai);
            ViewBag.TotalTinChi = await _context.MonHocs.SumAsync(m => m.SoTinChi);
            ViewBag.AvgHocPhi = (int)(await _context.MonHocs.AnyAsync()
                ? await _context.MonHocs.AverageAsync(m => (double)m.HocPhi)
                : 0);

            var query = _context.MonHocs.Include(m => m.KhoaHocs).AsQueryable();

            // Tìm kiếm theo tên hoặc mã môn
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.Trim().ToLower();
                var cleanCode = term.Replace("#", "").Replace("mh", "").TrimStart('0');
                bool isNumeric = int.TryParse(cleanCode, out int searchId);

                query = query.Where(m => m.TenMonHoc.ToLower().Contains(term)
                                      || (isNumeric && m.MaMonHoc == searchId)
                                      || m.MaMonHoc.ToString().Contains(term));
                ViewBag.CurrentSearch = searchString;
            }

            // LINQ Lọc theo trạng thái Đang mở / Tạm dừng
            if (trangThai.HasValue)
            {
                query = query.Where(m => m.TrangThai == trangThai.Value);
                ViewBag.CurrentTrangThai = trangThai;
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
                .OrderByDescending(m => m.MaMonHoc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return View(list);
        }

        // GET: /MonHoc/Create
        // Quyền: Admin và Nhân viên đều có quyền thêm mới môn học (Mục 25.2 Đề 16)
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        public IActionResult Create()
        {
            return View(new MonHoc { SoTinChi = 3, HocPhi = 3000000, TrangThai = true });
        }

        // POST: /MonHoc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        public async Task<IActionResult> Create(MonHoc monHoc)
        {
            if (ModelState.IsValid)
            {
                // LINQ Kiểm tra trùng lặp tên môn học an toàn
                var tenMonTrim = monHoc.TenMonHoc?.Trim().ToLower() ?? string.Empty;
                bool exists = await _context.MonHocs.AnyAsync(m =>
                    m.TenMonHoc.Trim().ToLower() == tenMonTrim);

                if (exists)
                {
                    ModelState.AddModelError("TenMonHoc", "Tên môn học này đã tồn tại trong hệ thống. Vui lòng nhập tên khác.");
                    return View(monHoc);
                }

                try
                {
                    _context.Add(monHoc);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã thêm mới môn học '{monHoc.TenMonHoc}' thành công!";
                    return RedirectToAction(nameof(QuanLy));
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Không thể lưu môn học do lỗi kết nối CSDL. Vui lòng thử lại sau.");
                    return View(monHoc);
                }
            }
            return View(monHoc);
        }

        // GET: /MonHoc/Edit/5
        // Quyền: Admin và Nhân viên đều có quyền chỉnh sửa môn học
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var monHoc = await _context.MonHocs.FindAsync(id);
            if (monHoc == null) return NotFound();

            return View(monHoc);
        }

        // POST: /MonHoc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        public async Task<IActionResult> Edit(int id, MonHoc monHoc)
        {
            if (id != monHoc.MaMonHoc) return NotFound();

            if (ModelState.IsValid)
            {
                // Kiểm tra trùng tên môn học
                var tenMonTrim = monHoc.TenMonHoc?.Trim().ToLower() ?? string.Empty;
                bool exists = await _context.MonHocs.AnyAsync(m =>
                    m.MaMonHoc != id && m.TenMonHoc.Trim().ToLower() == tenMonTrim);

                if (exists)
                {
                    ModelState.AddModelError("TenMonHoc", "Tên môn học này đã trùng với môn học khác trong hệ thống.");
                    return View(monHoc);
                }

                try
                {
                    // Cập nhật các trường được phép sửa
                    var existing = await _context.MonHocs.FindAsync(id);
                    if (existing == null)
                    {
                        return NotFound();
                    }

                    // Kiểm tra khóa học đang mở trước khi tạm dừng
                    if (!monHoc.TrangThai && existing.TrangThai)
                    {
                        bool coKhoaHocDangMo = await _context.KhoaHocs.AnyAsync(k => k.MaMonHoc == id && k.TrangThai == "DangMo");
                        if (coKhoaHocDangMo)
                        {
                            ModelState.AddModelError(nameof(monHoc.TrangThai), "Không thể chuyển môn học sang 'Tạm dừng' vì vẫn còn lớp khóa học đang mở.");
                            return View(monHoc);
                        }
                    }

                    existing.TenMonHoc = monHoc.TenMonHoc?.Trim() ?? string.Empty;
                    existing.SoTinChi = monHoc.SoTinChi;
                    existing.HocPhi = monHoc.HocPhi;
                    existing.MoTa = monHoc.MoTa;
                    existing.TrangThai = monHoc.TrangThai;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Cập nhật môn học '{existing.TenMonHoc}' thành công!";
                    return RedirectToAction(nameof(QuanLy));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.MonHocs.AnyAsync(e => e.MaMonHoc == monHoc.MaMonHoc))
                    {
                        return NotFound();
                    }
                    throw;
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Không thể cập nhật môn học do sự cố CSDL. Vui lòng thử lại.");
                    return View(monHoc);
                }
            }
            return View(monHoc);
        }

        // GET: /MonHoc/Delete/5
        // Quyền: Chỉ Admin
        [AuthorizeRole(VaiTro.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var monHoc = await _context.MonHocs
                .Include(m => m.KhoaHocs)
                .FirstOrDefaultAsync(m => m.MaMonHoc == id);

            if (monHoc == null) return NotFound();

            // Kiểm tra xem môn học đã có khóa học tham chiếu chưa
            ViewBag.HasKhoaHoc = monHoc.KhoaHocs.Any();
            return View(monHoc);
        }

        // POST: /MonHoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(VaiTro.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // BẢO VỆ TOÀN VẸN DỮ LIỆU BẮT BUỘC: Kiểm tra trước khi xóa
                bool daCoKhoaHoc = await _context.KhoaHocs.AnyAsync(k => k.MaMonHoc == id);

                if (daCoKhoaHoc)
                {
                    TempData["ErrorMessage"] = "Không thể xóa môn học này vì đã có khóa học đang liên kết. Bạn có thể sửa trạng thái môn học sang 'Tạm dừng'!";
                    return RedirectToAction(nameof(QuanLy));
                }

                var monHoc = await _context.MonHocs.FindAsync(id);
                if (monHoc != null)
                {
                    _context.MonHocs.Remove(monHoc);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã xóa môn học '{monHoc.TenMonHoc}' thành công.";
                }
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Không thể xóa môn học này vì có dữ liệu liên kết khác trong hệ thống.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Đã xảy ra sự cố khi xóa môn học. Vui lòng thử lại sau.";
            }

            return RedirectToAction(nameof(QuanLy));
        }

        // POST: /MonHoc/ToggleStatus/5
        // Đổi trạng thái nhanh Đang mở / Tạm dừng
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var monHoc = await _context.MonHocs.FindAsync(id);
                if (monHoc != null)
                {
                    // Kiểm tra khóa học đang mở trước khi xóa
                    if (monHoc.TrangThai)
                    {
                        bool coKhoaHocDangMo = await _context.KhoaHocs.AnyAsync(k => k.MaMonHoc == id && k.TrangThai == "DangMo");
                        if (coKhoaHocDangMo)
                        {
                            TempData["ErrorMessage"] = $"Không thể tạm dừng môn học '{monHoc.TenMonHoc}' vì vẫn còn lớp khóa học đang mở đào tạo. Vui lòng đóng hoặc hoàn tất các khóa học trước.";
                            return RedirectToAction(nameof(QuanLy));
                        }
                    }

                    monHoc.TrangThai = !monHoc.TrangThai;
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã đổi trạng thái môn '{monHoc.TenMonHoc}' sang {(monHoc.TrangThai ? "Đang mở" : "Tạm dừng")}.";
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Không thể đổi trạng thái môn học do sự cố CSDL.";
            }
            return RedirectToAction(nameof(QuanLy));
        }
    }
}
