// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Nội dung thực hiện: Module 1 - Controller Xác thực, Session, Đăng nhập, Đăng xuất, Đăng ký và Đổi mật khẩu

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Helpers;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaiKhoanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /TaiKhoan/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // Nếu đã có Session đăng nhập -> chuyển hướng thẳng theo vai trò
            if (HttpContext.Session.GetInt32("MaTaiKhoan") != null)
            {
                return RedirectBasedOnRole(HttpContext.Session.GetString("VaiTro"));
            }

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        // POST: /TaiKhoan/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 1. LINQ kiểm tra tài khoản
            var user = await _context.TaiKhoans
                .Include(t => t.HocVien)
                .FirstOrDefaultAsync(t => t.TenDangNhap.ToLower() == model.TenDangNhap.Trim().ToLower());

            // 2. Đối soát mật khẩu băm SHA-256
            if (user == null || !PasswordHelper.VerifyPassword(model.MatKhau, user.MatKhau))
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác.");
                return View(model);
            }

            // 3. Kiểm tra tài khoản bị khóa
            if (!user.TrangThai)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.");
                return View(model);
            }

            // 4. Thiết lập 4 khóa Session thiết yếu theo quy chuẩn Đề 16
            HttpContext.Session.SetInt32("MaTaiKhoan", user.MaTaiKhoan);
            HttpContext.Session.SetString("TenDangNhap", user.TenDangNhap);
            HttpContext.Session.SetString("HoTen", user.HoTen);
            HttpContext.Session.SetString("VaiTro", user.VaiTro);
            if (user.HocVien != null)
            {
                HttpContext.Session.SetInt32("MaHocVien", user.HocVien.MaHocVien);
            }

            TempData["SuccessMessage"] = $"Xin chào {user.HoTen}, bạn đã đăng nhập thành công!";

            // 5. Điều hướng sau đăng nhập
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectBasedOnRole(user.VaiTro);
        }

        // GET/POST: /TaiKhoan/Logout
        public IActionResult Logout()
        {
            // Xóa toàn bộ Session và chuyển về trang Login
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Bạn đã đăng xuất khỏi hệ thống thành công.";
            return RedirectToAction(nameof(Login));
        }

        // GET: /TaiKhoan/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: /TaiKhoan/Register
        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetInt32("MaTaiKhoan") != null)
            {
                return RedirectBasedOnRole(HttpContext.Session.GetString("VaiTro"));
            }
            return View();
        }

        // POST: /TaiKhoan/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // LINQ Kiểm tra trùng Tên đăng nhập
            bool isTaken = await _context.TaiKhoans.AnyAsync(t => 
                t.TenDangNhap.ToLower() == model.TenDangNhap.Trim().ToLower());

            if (isTaken)
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập này đã được sử dụng. Vui lòng chọn tên khác.");
                return View(model);
            }

            // Tạo tài khoản mới với vai trò Học viên
            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = model.TenDangNhap.Trim(),
                MatKhau = PasswordHelper.HashPassword(model.MatKhau),
                HoTen = model.HoTen.Trim(),
                Email = model.Email.Trim(),
                VaiTro = "HocVien",
                TrangThai = true,
                NgayTao = DateTime.Now
            };

            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();

            // Khởi tạo hồ sơ học viên tương ứng
            var hocVien = new HocVien
            {
                MaTaiKhoan = taiKhoan.MaTaiKhoan,
                HoTen = taiKhoan.HoTen,
                Email = taiKhoan.Email,
                SoDienThoai = model.SoDienThoai.Trim(),
                NgayDangKy = DateTime.Now,
                TrangThai = true
            };
            _context.HocViens.Add(hocVien);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập để bắt đầu.";
            return RedirectToAction(nameof(Login));
        }

        // GET: /TaiKhoan/ChangePassword
        [AuthorizeRole]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: /TaiKhoan/ChangePassword
        [AuthorizeRole]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
            {
                return RedirectToAction(nameof(Login));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.TaiKhoans.FindAsync(maTaiKhoan.Value);
            if (user == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Kiểm tra mật khẩu hiện tại
            if (!PasswordHelper.VerifyPassword(model.MatKhauHienTai, user.MatKhau))
            {
                ModelState.AddModelError("MatKhauHienTai", "Mật khẩu hiện tại không chính xác.");
                return View(model);
            }

            // Cập nhật mật khẩu mới đã băm
            user.MatKhau = PasswordHelper.HashPassword(model.MatKhauMoi);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
            return RedirectBasedOnRole(user.VaiTro);
        }

        // Điều hướng thông minh theo vai trò
        private IActionResult RedirectBasedOnRole(string? role)
        {
            return role switch
            {
                "Admin" => RedirectToAction("Index", "MonHoc"),
                "NhanVien" => RedirectToAction("Index", "Home"),
                _ => RedirectToAction("Index", "Home")
            };
        }
    }
}
