// Họ và tên: Ngọc Tùng Lâm
// Mã sinh viên: 23103100098
// Nội dung thực hiện: Module 3 - Quản lý Học viên & Hồ sơ cá nhân (Sử dụng Repository Pattern)

using Microsoft.AspNetCore.Mvc;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    public class HocVienController : Controller
    {
        private readonly IHocVienRepository _hocVienRepo;
        private readonly ILogger<HocVienController> _logger;

        public HocVienController(IHocVienRepository hocVienRepo, ILogger<HocVienController> logger)
        {
            _hocVienRepo = hocVienRepo;
            _logger = logger;
        }

        // GET: /HocVien or /HocVien/Index
        [HttpGet]
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> Index(HocVienFilterViewModel filter)
        {
            var result = await _hocVienRepo.GetFilterViewModelAsync(filter);
            return View(result);
        }

        // GET: /HocVien/Details/5
        [HttpGet]
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var viewModel = await _hocVienRepo.GetDetailsViewModelAsync(id.Value);
            if (viewModel == null) return NotFound();

            return View(viewModel);
        }

        // GET: /HocVien/Create
        [HttpGet]
        [AuthorizeRole("Admin", "NhanVien")]
        public IActionResult Create()
        {
            var model = new HocVienFormViewModel
            {
                TrangThai = true,
                TaoTaiKhoan = true,
                GioiTinh = "Nam",
                TrinhDo = "Đại học"
            };
            return View(model);
        }

        // POST: /HocVien/Create
        [HttpPost]
        [AuthorizeRole("Admin", "NhanVien")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HocVienFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Kiểm tra trùng Email
                if (await _hocVienRepo.ExistsEmailAsync(model.Email))
                {
                    ModelState.AddModelError("Email", "Địa chỉ Email này đã được đăng ký cho một học viên khác.");
                }

                // 2. Kiểm tra trùng Số điện thoại
                if (await _hocVienRepo.ExistsSoDienThoaiAsync(model.SoDienThoai))
                {
                    ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng bởi học viên khác.");
                }

                // 3. Kiểm tra thông tin tài khoản nếu chọn tạo
                if (model.TaoTaiKhoan)
                {
                    if (string.IsNullOrWhiteSpace(model.TenDangNhap))
                    {
                        ModelState.AddModelError("TenDangNhap", "Vui lòng nhập Tên đăng nhập khi bật tùy chọn tạo tài khoản.");
                    }
                    else if (await _hocVienRepo.ExistsTenDangNhapAsync(model.TenDangNhap))
                    {
                        ModelState.AddModelError("TenDangNhap", "Tên đăng nhập này đã tồn tại trong hệ thống. Vui lòng chọn tên khác.");
                    }

                    if (string.IsNullOrWhiteSpace(model.MatKhau))
                    {
                        ModelState.AddModelError("MatKhau", "Vui lòng nhập mật khẩu khởi tạo.");
                    }
                }

                if (ModelState.IsValid)
                {
                    var avatarPath = await ProcessAvatarUploadAsync(model.FileAnh, model.HinhAnh);
                    var (success, error, newId) = await _hocVienRepo.CreateHocVienAsync(model, avatarPath);

                    if (success)
                    {
                        TempData["SuccessMessage"] = $"Thêm mới học viên '{model.HoTen}' thành công!";
                        return RedirectToAction(nameof(Details), new { id = newId });
                    }

                    ModelState.AddModelError(string.Empty, error ?? "Có lỗi xảy ra khi lưu thông tin.");
                }
            }

            return View(model);
        }

        // GET: /HocVien/Edit/5
        [HttpGet]
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var hocVien = await _hocVienRepo.GetByIdWithTaiKhoanAsync(id.Value);
            if (hocVien == null) return NotFound();

            var model = new HocVienFormViewModel
            {
                MaHocVien = hocVien.MaHocVien,
                MaTaiKhoan = hocVien.MaTaiKhoan,
                HoTen = hocVien.HoTen,
                NgaySinh = hocVien.NgaySinh,
                GioiTinh = hocVien.GioiTinh ?? "Nam",
                SoDienThoai = hocVien.SoDienThoai,
                Email = hocVien.Email,
                DiaChi = hocVien.DiaChi,
                TrinhDo = hocVien.TrinhDo ?? "Đại học",
                TrangThai = hocVien.TrangThai,
                HinhAnh = hocVien.HinhAnh,
                GhiChu = hocVien.GhiChu,
                TaoTaiKhoan = false,
                TenDangNhap = hocVien.TaiKhoan?.TenDangNhap
            };

            return View(model);
        }

        // POST: /HocVien/Edit/5
        [HttpPost]
        [AuthorizeRole("Admin", "NhanVien")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HocVienFormViewModel model)
        {
            if (id != model.MaHocVien) return NotFound();

            if (ModelState.IsValid)
            {
                // Kiểm tra trùng Email
                if (await _hocVienRepo.ExistsEmailAsync(model.Email, id))
                {
                    ModelState.AddModelError("Email", "Địa chỉ Email này đã được sử dụng bởi học viên khác.");
                    return View(model);
                }

                // Kiểm tra trùng SĐT
                if (await _hocVienRepo.ExistsSoDienThoaiAsync(model.SoDienThoai, id))
                {
                    ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng bởi học viên khác.");
                    return View(model);
                }

                var avatarPath = await ProcessAvatarUploadAsync(model.FileAnh, model.HinhAnh);
                var (success, error) = await _hocVienRepo.UpdateHocVienAsync(id, model, avatarPath);

                if (success)
                {
                    TempData["SuccessMessage"] = $"Cập nhật hồ sơ học viên '{model.HoTen}' thành công!";
                    return RedirectToAction(nameof(Details), new { id });
                }

                ModelState.AddModelError(string.Empty, error ?? "Có lỗi xảy ra khi cập nhật hồ sơ.");
            }

            return View(model);
        }

        // GET: /HocVien/Delete/5
        [HttpGet]
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var hocVien = await _hocVienRepo.GetByIdAsync(id.Value);
            if (hocVien == null) return NotFound();

            return View(hocVien);
        }

        // POST: /HocVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [AuthorizeRole("Admin", "NhanVien")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var (success, error) = await _hocVienRepo.DeleteHocVienAsync(id);
            if (!success)
            {
                TempData["ErrorMessage"] = error;
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["SuccessMessage"] = "Đã xóa hồ sơ học viên thành công.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /HocVien/ToggleTrangThai/5
        [HttpPost]
        [AuthorizeRole("Admin", "NhanVien")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleTrangThai(int id)
        {
            var (success, error, newStatus) = await _hocVienRepo.ToggleTrangThaiAsync(id);
            if (!success)
            {
                TempData["ErrorMessage"] = error;
            }
            else
            {
                TempData["SuccessMessage"] = $"Đã {(newStatus ? "kích hoạt" : "tạm khóa")} hồ sơ học viên thành công.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /HocVien/HoSoCaNhan
        [HttpGet]
        [AuthorizeRole("HocVien", "Admin", "NhanVien")]
        public async Task<IActionResult> HoSoCaNhan()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!maTaiKhoan.HasValue)
            {
                return RedirectToAction("Login", "TaiKhoan", new { returnUrl = Url.Action(nameof(HoSoCaNhan)) });
            }

            var model = await _hocVienRepo.GetHoSoCaNhanAsync(maTaiKhoan.Value);
            if (model == null)
            {
                return RedirectToAction("Login", "TaiKhoan");
            }

            return View(model);
        }

        // POST: /HocVien/CapNhatHoSo
        [HttpPost]
        [AuthorizeRole("HocVien", "Admin", "NhanVien")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatHoSo(HoSoCaNhanViewModel model)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!maTaiKhoan.HasValue) return RedirectToAction("Login", "TaiKhoan");

            if (ModelState.IsValid)
            {
                var avatarPath = await ProcessAvatarUploadAsync(model.FileAnh, model.HinhAnh);
                var (success, error) = await _hocVienRepo.CapNhatHoSoAsync(maTaiKhoan.Value, model, avatarPath);

                if (success)
                {
                    HttpContext.Session.SetString("HoTen", model.HoTen);
                    TempData["SuccessMessage"] = "Hồ sơ cá nhân của bạn đã được cập nhật thành công!";
                    return RedirectToAction(nameof(HoSoCaNhan));
                }

                TempData["ErrorMessage"] = error;
            }

            return View("HoSoCaNhan", model);
        }

        /// <summary>
        /// Xử lý tải lên tệp ảnh đại diện cho học viên
        /// </summary>
        private async Task<string?> ProcessAvatarUploadAsync(IFormFile? file, string? currentAvatar)
        {
            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".svg" };
                if (allowedExtensions.Contains(extension))
                {
                    var uniqueFileName = $"hv_{Guid.NewGuid():N}{extension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    return $"/uploads/avatars/{uniqueFileName}";
                }
            }

            return currentAvatar;
        }
    }
}
