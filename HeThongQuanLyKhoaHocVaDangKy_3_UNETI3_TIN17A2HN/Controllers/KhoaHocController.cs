// Họ và tên: Nguyễn Văn Mạnh
// Mã sinh viên: 23103100096
// Nội dung thực hiện: Module 2 - Quản lý khóa học

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

        // =========================================================================
        // 1. Cổng tra cứu khóa học
        // =========================================================================

        // GET: /KhoaHoc or /KhoaHoc/Index
        [HttpGet]
        public async Task<IActionResult> Index(KhoaHocFilterViewModel filter, int? page = null)
        {
            // Tra cứu, lọc kết hợp, sắp xếp và phân trang danh sách khóa học
            var (model, monHocList) = await _khoaHocRepo.GetFilterViewModelAsync(filter, page);
            ViewBag.MonHocList = monHocList;

            // Xử lý phản hồi AJAX khi tìm kiếm và lọc tức thì
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_KhoaHocGrid", model);
            }

            // Hiển thị giao diện danh sách khóa học
            return View(model);
        }

        // =========================================================================
        // 2. Trang quản trị khóa học
        // =========================================================================

        // GET: /KhoaHoc/QuanLy
        [HttpGet]
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> QuanLy(
            string? searchString,
            int? maMonHoc,
            string? trangThai,
            string? hinhThuc,
            string? khoangHocPhi,
            string? tinhTrangCho,
            string? sortBy = "id_desc",
            int page = 1,
            int pageSize = 10)
        {
            // Thực hiện tra cứu, lọc đa tiêu chí, sắp xếp và phân trang quản trị
            var result = await _khoaHocRepo.GetQuanLyDataAsync(
                searchString, maMonHoc, trangThai, hinhThuc, khoangHocPhi, tinhTrangCho, sortBy, page, pageSize);

            // Thống kê số liệu phục vụ các thẻ thống kê
            ViewBag.TotalCount = result.TotalCount;
            ViewBag.DangMoCount = result.DangMoCount;
            ViewBag.DangHocCount = result.DangHocCount;
            ViewBag.DaKetThucCount = result.DaKetThucCount;
            ViewBag.AvgHocPhi = result.AvgHocPhi;
            ViewBag.MonHocList = result.MonHocList;

            // Lưu giữ trạng thái tìm kiếm và bộ lọc đang kích hoạt
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentMaMonHoc = maMonHoc;
            ViewBag.CurrentTrangThai = trangThai;
            ViewBag.CurrentHinhThuc = hinhThuc;
            ViewBag.CurrentKhoangHocPhi = khoangHocPhi;
            ViewBag.CurrentTinhTrangCho = tinhTrangCho;
            ViewBag.CurrentSortBy = sortBy;

            // Thiết lập thông tin phân trang
            ViewBag.PageIndex = result.PageIndex;
            ViewBag.PageSize = result.PageSize;
            ViewBag.TotalItems = result.TotalItems;
            ViewBag.TotalPages = result.TotalPages;

            // Trả về view quản trị kèm danh sách khóa học
            return View(result.Items);
        }

        // =========================================================================
        // 3. Chi tiết khóa học
        // =========================================================================

        // GET: /KhoaHoc/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // Lấy thông tin chi tiết khóa học kèm môn học, giảng viên và danh sách đăng ký
            var khoaHoc = await _khoaHocRepo.GetByIdWithDetailsAsync(id.Value);
            if (khoaHoc == null) return NotFound();

            // Kiểm tra trạng thái học viên đang đăng nhập đã đăng ký khóa học này hay chưa
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            bool daDangKy = false;
            if (maTaiKhoan.HasValue)
            {
                daDangKy = await _khoaHocRepo.CheckHocVienDaDangKyAsync(id.Value, maTaiKhoan.Value);
            }
            ViewBag.DaDangKy = daDangKy;

            // Trả về view chi tiết khóa học
            return View(khoaHoc);
        }

        // =========================================================================
        // 4. Thêm mới khóa học
        // =========================================================================

        // GET: /KhoaHoc/Create
        [HttpGet]
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> Create()
        {
            // Lấy dữ liệu dropdown môn học và giảng viên từ cơ sở dữ liệu
            var (monHocList, giangVienList) = await _khoaHocRepo.GetDropdownDataAsync();
            ViewBag.MaMonHoc = monHocList;
            ViewBag.MaGiangVien = giangVienList;

            // Khởi tạo đối tượng khóa học với giá trị mặc định
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
            // Kiểm tra ràng buộc nghiệp vụ: Ngày kết thúc phải sau ngày bắt đầu
            if (khoaHoc.NgayKetThuc <= khoaHoc.NgayBatDau)
            {
                ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải sau ngày bắt đầu khóa học.");
            }

            // Kiểm tra tính hợp lệ của dữ liệu trước khi lưu
            if (ModelState.IsValid)
            {
                // Thêm mới khóa học vào cơ sở dữ liệu
                var (success, errorMessage, _) = await _khoaHocRepo.CreateKhoaHocAsync(khoaHoc);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Đã thêm mới khóa học '{khoaHoc.TenKhoaHoc}' thành công!";
                    return RedirectToAction(nameof(QuanLy));
                }
                ModelState.AddModelError(string.Empty, errorMessage ?? "Không thể thêm mới khóa học.");
            }

            // Tải lại dữ liệu dropdown khi thêm mới không thành công
            var (monHocList, giangVienList) = await _khoaHocRepo.GetDropdownDataAsync(khoaHoc.MaMonHoc, khoaHoc.MaGiangVien);
            ViewBag.MaMonHoc = monHocList;
            ViewBag.MaGiangVien = giangVienList;
            return View(khoaHoc);
        }

        // =========================================================================
        // 5. Chỉnh sửa khóa học
        // =========================================================================

        // GET: /KhoaHoc/Edit/5
        [HttpGet]
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // Tìm kiếm khóa học theo mã để chỉnh sửa
            var khoaHoc = await _khoaHocRepo.GetByIdAsync(id.Value);
            if (khoaHoc == null) return NotFound();

            // Lấy dữ liệu dropdown môn học và giảng viên từ cơ sở dữ liệu
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

            // Kiểm tra ràng buộc nghiệp vụ: Ngày kết thúc phải sau ngày bắt đầu
            if (khoaHoc.NgayKetThuc <= khoaHoc.NgayBatDau)
            {
                ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải sau ngày bắt đầu khóa học.");
            }

            // Kiểm tra tính hợp lệ của dữ liệu trước khi cập nhật
            if (ModelState.IsValid)
            {
                // Cập nhật thông tin khóa học trong cơ sở dữ liệu
                var (success, errorMessage) = await _khoaHocRepo.UpdateKhoaHocAsync(khoaHoc);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Cập nhật khóa học '{khoaHoc.TenKhoaHoc}' thành công!";
                    return RedirectToAction(nameof(QuanLy));
                }
                ModelState.AddModelError(string.Empty, errorMessage ?? "Không thể cập nhật khóa học.");
            }

            // Tải lại dữ liệu dropdown khi cập nhật không thành công
            var (monHocList, giangVienList) = await _khoaHocRepo.GetDropdownDataAsync(khoaHoc.MaMonHoc, khoaHoc.MaGiangVien);
            ViewBag.MaMonHoc = monHocList;
            ViewBag.MaGiangVien = giangVienList;
            return View(khoaHoc);
        }

        // =========================================================================
        // 6. Xóa khóa học
        // =========================================================================

        // GET: /KhoaHoc/Delete/5
        [HttpGet]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            // Lấy thông tin khóa học phục vụ xác nhận xóa
            var khoaHoc = await _khoaHocRepo.GetByIdForDeleteAsync(id.Value);
            if (khoaHoc == null) return NotFound();

            // Kiểm tra khóa học đã phát sinh lịch sử đăng ký chưa để bảo vệ toàn vẹn dữ liệu
            ViewBag.HasDangKy = await _khoaHocRepo.HasDangKyAsync(id.Value);
            return View(khoaHoc);
        }

        // POST: /KhoaHoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Thực hiện xóa an toàn và kiểm tra bảo vệ toàn vẹn dữ liệu
            var (success, errorMessage) = await _khoaHocRepo.DeleteKhoaHocAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Đã xóa khóa học thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage ?? "Không thể xóa khóa học.";
            }

            return RedirectToAction(nameof(QuanLy));
        }

        // =========================================================================
        // 7. Cập nhật trạng thái khóa học
        // =========================================================================

        // POST: /KhoaHoc/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            // Chuyển đổi trạng thái vòng đời của khóa học
            var (success, errorMessage, statusLabel) = await _khoaHocRepo.UpdateStatusAsync(id, newStatus);
            if (success)
            {
                TempData["SuccessMessage"] = $"Đã chuyển trạng thái khóa học sang '{statusLabel}'.";
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage ?? "Không thể cập nhật trạng thái.";
            }

            return RedirectToAction(nameof(QuanLy));
        }

        // =========================================================================
        // 8. Định dạng trạng thái hiển thị
        // =========================================================================
        public static string GetStatusLabel(string? trangThai)
        {
            // Lấy nhãn hiển thị trạng thái tiếng Việt
            return KhoaHocRepository.FormatStatusLabel(trangThai);
        }
    }
}
