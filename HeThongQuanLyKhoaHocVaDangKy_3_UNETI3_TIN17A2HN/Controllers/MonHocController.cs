// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Module 1: Quản lý môn học (Sử dụng Repository Pattern)

using Microsoft.AspNetCore.Mvc;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    public class MonHocController : Controller
    {
        private readonly IMonHocRepository _monHocRepo;
        private readonly ILogger<MonHocController> _logger;

        public MonHocController(IMonHocRepository monHocRepo, ILogger<MonHocController> logger)
        {
            _monHocRepo = monHocRepo;
            _logger = logger;
        }
        // 1. CỔNG SINH VIÊN & KHÁCH (PUBLIC PORTAL - CARD GRID)

        // GET: /MonHoc
        // Danh sách môn học công khai (hỗ trợ AJAX - Phân trang 6 môn/trang)
        public async Task<IActionResult> Index(string? searchString, int? soTinChi, bool? trangThai, string? sortBy = null, int page = 1, int pageSize = 6, int? pageIndex = null)
        {
            if (pageIndex.HasValue && pageIndex.Value > 0) page = pageIndex.Value;

            var (list, totalCount, totalFiltered, totalPages) =
                await _monHocRepo.GetPublicListAsync(searchString, soTinChi, trangThai, sortBy, page, pageSize);

            ViewBag.TotalCount = totalCount;
            ViewBag.TotalFilteredItems = totalFiltered;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.SortBy = sortBy;

            if (!string.IsNullOrWhiteSpace(searchString))
                ViewBag.CurrentSearch = searchString.Trim();
            if (soTinChi.HasValue && soTinChi.Value > 0)
                ViewBag.CurrentTinChi = soTinChi.Value;
            if (trangThai.HasValue)
                ViewBag.CurrentTrangThai = trangThai;

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

            var monHoc = await _monHocRepo.GetDetailsAsync(id.Value);
            if (monHoc == null) return NotFound();

            // 1. Kiểm tra danh sách khóa học mà sinh viên đang đăng nhập đã đăng ký
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            var registeredKhoaHocIds = new List<int>();
            if (maTaiKhoan.HasValue)
            {
                registeredKhoaHocIds = await _monHocRepo.GetRegisteredKhoaHocIdsAsync(maTaiKhoan.Value);
            }
            ViewBag.RegisteredKhoaHocIds = registeredKhoaHocIds;

            // 2. Danh sách giảng viên đang dạy môn học này.
            // Phân công giảng viên nằm ở từng khóa học (lớp học phần) nên danh sách được
            // suy ra từ các lớp của chính môn học, loại lớp đã hủy và chống trùng lặp.
            // Môn chưa có lớp nào được gán giảng viên thì danh sách để trống (không lấy
            // giảng viên không liên quan lấp vào).
            var danhSachGiangVien = monHoc.KhoaHocs
                .Where(k => k.GiangVien != null && k.TrangThai != "BiHuy")
                .Select(k => k.GiangVien!)
                .GroupBy(g => g.MaGiangVien)
                .Select(g => g.First())
                .OrderBy(g => g.HoTen)
                .ToList();
            ViewBag.DanhSachGiangVien = danhSachGiangVien;
            ViewBag.GiangVienTieuBieu = danhSachGiangVien.FirstOrDefault();

            // 3. Đếm số lớp mở và kiểm tra còn chỗ
            var openClasses = monHoc.KhoaHocs.Where(k => k.TrangThai == "DangMo").ToList();
            ViewBag.OpenClassesCount = openClasses.Count;
            ViewBag.HasAvailableSeats = openClasses.Any(k => k.DangKyKhoaHocs.Count(d => d.TrangThai != "BiHuy") < k.SoLuongToiDa);

            return View(monHoc);
        }
        // 2. TRANG QUẢN TRỊ ADMIN & NHÂN VIÊN ĐÀO TẠO (DATA TABLE CRUD & STATS)

        // GET: /MonHoc/QuanLy
        // Quyền: Chỉ Admin và Nhân viên đào tạo mới được phép truy cập
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        public async Task<IActionResult> QuanLy(string? searchString, bool? trangThai, int page = 1, int pageSize = 10)
        {
            var (list, totalCount, activeCount, inactiveCount, totalTinChi, avgHocPhi, totalFiltered, totalPages) =
                await _monHocRepo.GetAdminListAsync(searchString, trangThai, page, pageSize);

            // Thống kê số liệu tổng thể phục vụ 4 thẻ Stat Cards
            ViewBag.TotalCount = totalCount;
            ViewBag.ActiveCount = activeCount;
            ViewBag.InactiveCount = inactiveCount;
            ViewBag.TotalTinChi = totalTinChi;
            ViewBag.AvgHocPhi = avgHocPhi;
            ViewBag.TotalFilteredItems = totalFiltered;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            if (!string.IsNullOrWhiteSpace(searchString))
                ViewBag.CurrentSearch = searchString;
            if (trangThai.HasValue)
                ViewBag.CurrentTrangThai = trangThai;

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
                if (await _monHocRepo.ExistsTenMonHocAsync(monHoc.TenMonHoc ?? ""))
                {
                    ModelState.AddModelError("TenMonHoc", "Tên môn học này đã tồn tại trong hệ thống. Vui lòng nhập tên khác.");
                    return View(monHoc);
                }

                var (success, error) = await _monHocRepo.CreateAsync(monHoc);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Đã thêm mới môn học '{monHoc.TenMonHoc}' thành công!";
                    return RedirectToAction(nameof(QuanLy));
                }

                ModelState.AddModelError(string.Empty, error ?? "Lỗi không xác định.");
                return View(monHoc);
            }
            return View(monHoc);
        }

        // GET: /MonHoc/Edit/5
        // Quyền: Admin và Nhân viên đều có quyền chỉnh sửa môn học
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var monHoc = await _monHocRepo.GetByIdAsync(id.Value);
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
                if (await _monHocRepo.ExistsTenMonHocAsync(monHoc.TenMonHoc ?? "", id))
                {
                    ModelState.AddModelError("TenMonHoc", "Tên môn học này đã trùng với môn học khác trong hệ thống.");
                    return View(monHoc);
                }

                var (success, error) = await _monHocRepo.UpdateAsync(id, monHoc);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Cập nhật môn học '{monHoc.TenMonHoc}' thành công!";
                    return RedirectToAction(nameof(QuanLy));
                }

                ModelState.AddModelError(string.IsNullOrEmpty(error) ? string.Empty : (error.Contains("Tạm dừng") ? nameof(monHoc.TrangThai) : string.Empty), error ?? "Lỗi không xác định.");
                return View(monHoc);
            }
            return View(monHoc);
        }

        // GET: /MonHoc/Delete/5
        // Quyền: Chỉ Admin
        [AuthorizeRole(VaiTro.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var monHoc = await _monHocRepo.GetByIdWithKhoaHocsAsync(id.Value);
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
            var (success, message) = await _monHocRepo.DeleteAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
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
            var (success, message, newStatus) = await _monHocRepo.ToggleStatusAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction(nameof(QuanLy));
        }
    }
}
