// Họ và tên: Nguyễn Văn Mạnh
// Mã sinh viên: 23103100096
// Nội dung thực hiện: Module 2 - Quản lý Khóa học (CRUD, Tìm kiếm đa tiêu chí, Cập nhật trạng thái)

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    public class KhoaHocController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KhoaHocController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================================
        // 1. TRANG QUẢN TRỊ KHÓA HỌC (ADMIN & NHÂN VIÊN)
        // =========================================================================

        // GET: /KhoaHoc/QuanLy
        // Quyền: Chỉ Admin và Nhân viên đào tạo mới được phép truy cập
        // Tìm kiếm theo: Tên khóa học, Tên môn học, Tên giảng viên
        // Lọc theo: Trạng thái, Hình thức đào tạo
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> QuanLy(string? searchString, string? trangThai, string? hinhThuc)
        {
            // Thống kê số liệu tổng thể phục vụ 4 thẻ Stat Cards
            ViewBag.TotalCount = await _context.KhoaHocs.CountAsync();
            ViewBag.DangMoCount = await _context.KhoaHocs.CountAsync(k => k.TrangThai == "DangMo");
            ViewBag.DangHocCount = await _context.KhoaHocs.CountAsync(k => k.TrangThai == "DangHoc");
            ViewBag.DaKetThucCount = await _context.KhoaHocs.CountAsync(k => k.TrangThai == "DaKetThuc");
            ViewBag.AvgHocPhi = await _context.KhoaHocs.AnyAsync()
                ? (long)await _context.KhoaHocs.AverageAsync(k => (double)k.HocPhi)
                : 0;

            var query = _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.GiangVien)
                .Include(k => k.DangKyKhoaHocs)
                .AsQueryable();

            // LINQ Tìm kiếm đa tiêu chí: Tên khóa học, Tên môn học, Tên giảng viên
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var keyword = searchString.Trim().ToLower();
                query = query.Where(k =>
                    k.TenKhoaHoc.ToLower().Contains(keyword) ||
                    (k.MonHoc != null && k.MonHoc.TenMonHoc.ToLower().Contains(keyword)) ||
                    (k.GiangVien != null && k.GiangVien.HoTen.ToLower().Contains(keyword))
                );
                ViewBag.CurrentSearch = searchString;
            }

            // LINQ Lọc theo trạng thái khóa học
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(k => k.TrangThai == trangThai);
                ViewBag.CurrentTrangThai = trangThai;
            }

            // LINQ Lọc theo hình thức đào tạo
            if (!string.IsNullOrWhiteSpace(hinhThuc))
            {
                query = query.Where(k => k.HinhThuc == hinhThuc);
                ViewBag.CurrentHinhThuc = hinhThuc;
            }

            var list = await query.OrderByDescending(k => k.MaKhoaHoc).ToListAsync();
            return View(list);
        }

        // GET: /KhoaHoc/Details/5
        // Quyền: Admin và Nhân viên
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.GiangVien)
                .Include(k => k.DangKyKhoaHocs)
                .FirstOrDefaultAsync(k => k.MaKhoaHoc == id);

            if (khoaHoc == null) return NotFound();

            return View(khoaHoc);
        }

        // GET: /KhoaHoc/Create
        // Quyền: Admin và Nhân viên
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> Create()
        {
            await LoadDropdownDataAsync();
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
            // Kiểm tra nghiệp vụ: Ngày kết thúc phải sau ngày bắt đầu
            if (khoaHoc.NgayKetThuc <= khoaHoc.NgayBatDau)
            {
                ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải sau ngày bắt đầu khóa học.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(khoaHoc);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã thêm mới khóa học '{khoaHoc.TenKhoaHoc}' thành công!";
                    return RedirectToAction(nameof(QuanLy));
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Không thể lưu khóa học do lỗi kết nối CSDL. Vui lòng thử lại sau.");
                }
            }

            await LoadDropdownDataAsync(khoaHoc.MaMonHoc, khoaHoc.MaGiangVien);
            return View(khoaHoc);
        }

        // GET: /KhoaHoc/Edit/5
        // Quyền: Admin và Nhân viên
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var khoaHoc = await _context.KhoaHocs.FindAsync(id);
            if (khoaHoc == null) return NotFound();

            await LoadDropdownDataAsync(khoaHoc.MaMonHoc, khoaHoc.MaGiangVien);
            return View(khoaHoc);
        }

        // POST: /KhoaHoc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> Edit(int id, KhoaHoc khoaHoc)
        {
            if (id != khoaHoc.MaKhoaHoc) return NotFound();

            // Kiểm tra nghiệp vụ: Ngày kết thúc phải sau ngày bắt đầu
            if (khoaHoc.NgayKetThuc <= khoaHoc.NgayBatDau)
            {
                ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải sau ngày bắt đầu khóa học.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khoaHoc);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Cập nhật khóa học '{khoaHoc.TenKhoaHoc}' thành công!";
                    return RedirectToAction(nameof(QuanLy));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.KhoaHocs.AnyAsync(e => e.MaKhoaHoc == khoaHoc.MaKhoaHoc))
                    {
                        return NotFound();
                    }
                    throw;
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Không thể cập nhật khóa học do sự cố CSDL. Vui lòng thử lại.");
                }
            }

            await LoadDropdownDataAsync(khoaHoc.MaMonHoc, khoaHoc.MaGiangVien);
            return View(khoaHoc);
        }

        // GET: /KhoaHoc/Delete/5
        // Quyền: Chỉ Admin
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.GiangVien)
                .Include(k => k.DangKyKhoaHocs)
                .FirstOrDefaultAsync(k => k.MaKhoaHoc == id);

            if (khoaHoc == null) return NotFound();

            // Kiểm tra xem khóa học đã có đăng ký học viên chưa
            ViewBag.HasDangKy = khoaHoc.DangKyKhoaHocs.Any();
            return View(khoaHoc);
        }

        // POST: /KhoaHoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // BẢO VỆ TOÀN VẸN DỮ LIỆU BẮT BUỘC: Kiểm tra trước khi xóa
                bool daCoDangKy = await _context.DangKyKhoaHocs.AnyAsync(dk => dk.MaKhoaHoc == id);

                if (daCoDangKy)
                {
                    TempData["ErrorMessage"] = "Không thể xóa khóa học này vì đã có học viên đăng ký. Bạn có thể chuyển trạng thái sang 'Đã hủy'!";
                    return RedirectToAction(nameof(QuanLy));
                }

                var khoaHoc = await _context.KhoaHocs.FindAsync(id);
                if (khoaHoc != null)
                {
                    _context.KhoaHocs.Remove(khoaHoc);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã xóa khóa học '{khoaHoc.TenKhoaHoc}' thành công.";
                }
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Không thể xóa khóa học này vì có dữ liệu liên kết khác trong hệ thống.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Đã xảy ra sự cố khi xóa khóa học. Vui lòng thử lại sau.";
            }

            return RedirectToAction(nameof(QuanLy));
        }

        // POST: /KhoaHoc/UpdateStatus/5
        // Cập nhật trạng thái khóa học: SapMo -> DangMo -> DangHoc -> DaKetThuc / BiHuy
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            try
            {
                var khoaHoc = await _context.KhoaHocs.FindAsync(id);
                if (khoaHoc != null)
                {
                    var validStatuses = new[] { "SapMo", "DangMo", "DangHoc", "DaKetThuc", "BiHuy" };
                    if (!validStatuses.Contains(newStatus))
                    {
                        TempData["ErrorMessage"] = "Trạng thái không hợp lệ.";
                        return RedirectToAction(nameof(QuanLy));
                    }

                    khoaHoc.TrangThai = newStatus;
                    await _context.SaveChangesAsync();

                    var statusLabel = GetStatusLabel(newStatus);
                    TempData["SuccessMessage"] = $"Đã chuyển trạng thái khóa học '{khoaHoc.TenKhoaHoc}' sang '{statusLabel}'.";
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Không thể cập nhật trạng thái khóa học do sự cố CSDL.";
            }
            return RedirectToAction(nameof(QuanLy));
        }

        // =========================================================================
        // HELPER METHODS
        // =========================================================================

        /// <summary>
        /// Nạp danh sách Môn học và Giảng viên vào SelectList cho dropdown
        /// Khóa ngoại phải sử dụng select lấy dữ liệu từ Database
        /// </summary>
        private async Task LoadDropdownDataAsync(int? selectedMonHoc = null, int? selectedGiangVien = null)
        {
            // Chỉ lấy Môn học đang mở giảng dạy
            ViewBag.MaMonHoc = new SelectList(
                await _context.MonHocs
                    .Where(m => m.TrangThai)
                    .OrderBy(m => m.TenMonHoc)
                    .Select(m => new { m.MaMonHoc, Display = m.TenMonHoc + " (" + m.SoTinChi + " TC)" })
                    .ToListAsync(),
                "MaMonHoc", "Display", selectedMonHoc);

            // Chỉ lấy Giảng viên đang công tác
            ViewBag.MaGiangVien = new SelectList(
                await _context.GiangViens
                    .Where(g => g.TrangThai)
                    .OrderBy(g => g.HoTen)
                    .Select(g => new { g.MaGiangVien, Display = g.HoTen + " - " + (g.ChuyenMon ?? "N/A") })
                    .ToListAsync(),
                "MaGiangVien", "Display", selectedGiangVien);
        }

        /// <summary>
        /// Lấy nhãn hiển thị cho trạng thái khóa học
        /// </summary>
        public static string GetStatusLabel(string trangThai)
        {
            return trangThai switch
            {
                "SapMo" => "Sắp mở",
                "DangMo" => "Đang mở đăng ký",
                "DangHoc" => "Đang giảng dạy",
                "DaKetThuc" => "Đã kết thúc",
                "BiHuy" => "Đã hủy",
                _ => trangThai
            };
        }
    }
}
