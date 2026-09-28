// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Nội dung thực hiện: Module 1 - Quản lý môn học (Tách biệt Cổng Sinh viên Card Grid & Trang Quản trị CRUD Admin/NV)

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

        // =========================================================================
        // 1. CỔNG SINH VIÊN & KHÁCH (PUBLIC PORTAL - CARD GRID)
        // =========================================================================

        // GET: /MonHoc
        // Quyền: Mở công khai cho mọi đối tượng (Khách vãng lai & Sinh viên) xem danh mục môn học dạng Card Grid
        public async Task<IActionResult> Index(string? searchString, int? soTinChi, bool? trangThai)
        {
            var query = _context.MonHocs.Include(m => m.KhoaHocs).AsQueryable();

            // Tìm kiếm theo tên môn học
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(m => m.TenMonHoc.Contains(searchString.Trim()));
                ViewBag.CurrentSearch = searchString;
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

            ViewBag.TotalCount = await _context.MonHocs.CountAsync();
            var list = await query.OrderByDescending(m => m.MaMonHoc).ToListAsync();
            return View(list);
        }

        // GET: /MonHoc/Details/5
        // Quyền: Mở công khai cho mọi đối tượng xem chi tiết thông tin môn học & đề cương
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var monHoc = await _context.MonHocs
                .Include(m => m.KhoaHocs)
                .FirstOrDefaultAsync(m => m.MaMonHoc == id);

            if (monHoc == null) return NotFound();

            return View(monHoc);
        }

        // =========================================================================
        // 2. TRANG QUẢN TRỊ ADMIN & NHÂN VIÊN ĐÀO TẠO (DATA TABLE CRUD & STATS)
        // =========================================================================

        // GET: /MonHoc/QuanLy
        // Quyền: Chỉ Admin và Nhân viên đào tạo mới được phép truy cập
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> QuanLy(string? searchString, bool? trangThai)
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

            // LINQ Tìm kiếm theo tên môn học
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(m => m.TenMonHoc.Contains(searchString.Trim()));
                ViewBag.CurrentSearch = searchString;
            }

            // LINQ Lọc theo trạng thái Đang mở / Tạm dừng
            if (trangThai.HasValue)
            {
                query = query.Where(m => m.TrangThai == trangThai.Value);
                ViewBag.CurrentTrangThai = trangThai;
            }

            var list = await query.OrderByDescending(m => m.MaMonHoc).ToListAsync();
            return View(list);
        }

        // GET: /MonHoc/Create
        // Quyền: Chỉ Admin mới được quyền thêm mới môn học
        [AuthorizeRole("Admin")]
        public IActionResult Create()
        {
            return View(new MonHoc { SoTinChi = 3, HocPhi = 3000000, TrangThai = true });
        }

        // POST: /MonHoc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
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
        // Quyền: Chỉ Admin mới được quyền chỉnh sửa môn học
        [AuthorizeRole("Admin")]
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
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Edit(int id, MonHoc monHoc)
        {
            if (id != monHoc.MaMonHoc) return NotFound();

            if (ModelState.IsValid)
            {
                // LINQ Kiểm tra trùng tên môn học với bản ghi khác trong CSDL an toàn
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
                    _context.Update(monHoc);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Cập nhật môn học '{monHoc.TenMonHoc}' thành công!";
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
                return RedirectToAction(nameof(QuanLy));
            }
            return View(monHoc);
        }

        // GET: /MonHoc/Delete/5
        // Quyền: Chỉ Admin
        [AuthorizeRole("Admin")]
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
        [AuthorizeRole("Admin")]
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
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var monHoc = await _context.MonHocs.FindAsync(id);
                if (monHoc != null)
                {
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
