// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Module 1: Xác thực, tài khoản và phân quyền (Sử dụng Repository Pattern)

using Microsoft.AspNetCore.Mvc;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Helpers;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly ITaiKhoanRepository _taiKhoanRepo;
        private readonly ILogger<TaiKhoanController> _logger;
        private readonly IWebHostEnvironment _env;

        public TaiKhoanController(ITaiKhoanRepository taiKhoanRepo, ILogger<TaiKhoanController> logger, IWebHostEnvironment env)
        {
            _taiKhoanRepo = taiKhoanRepo;
            _logger = logger;
            _env = env;
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

            try
            {
                // Kiểm tra tài khoản (không phân biệt hoa thường)
                var user = await _taiKhoanRepo.GetByTenDangNhapAsync(model.TenDangNhap);

                // 2. Đối soát mật khẩu băm SHA-256
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác.");
                    return View(model);
                }

                bool isPasswordValid = PasswordHelper.VerifyPassword(model.MatKhau, user.MatKhau);
                if (!isPasswordValid)
                {
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác.");
                    return View(model);
                }

                // 3. Kiểm tra tài khoản bị khóa
                if (!user.TrangThai)
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ Quản trị viên.");
                    return View(model);
                }

                // Tự tạo hồ sơ học viên nếu chưa có
                await _taiKhoanRepo.EnsureHocVienProfileAsync(user);

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

                // 5. Điều hướng an toàn sau đăng nhập (Chống Open Redirect)
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return RedirectBasedOnRole(user.VaiTro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra trong quá trình xử lý đăng nhập cho tài khoản '{Username}'", model.TenDangNhap);
                ModelState.AddModelError(string.Empty, "Đã xảy ra sự cố trong quá trình đăng nhập. Vui lòng kiểm tra lại kết nối và thử lại.");
                return View(model);
            }
        }

        // POST: /TaiKhoan/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // Xóa toàn bộ Session và chuyển về trang Login
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Bạn đã đăng xuất khỏi hệ thống thành công.";
            return RedirectToAction(nameof(Login));
        }

        // GET: /TaiKhoan/Logout
        [HttpGet]
        [ActionName("Logout")]
        public IActionResult LogoutGet()
        {
            return RedirectToAction(nameof(Login));
        }

        // GET: /TaiKhoan/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            // Thiết lập mã trạng thái chuẩn HTTP 403 Forbidden
            Response.StatusCode = 403;
            ViewBag.UserRole = HttpContext.Session.GetString("VaiTro");
            ViewBag.FullName = HttpContext.Session.GetString("HoTen");
            return View();
        }

        // GET: /TaiKhoan/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            if (HttpContext.Session.GetInt32("MaTaiKhoan") != null)
            {
                return RedirectBasedOnRole(HttpContext.Session.GetString("VaiTro"));
            }
            return View();
        }

        // POST: /TaiKhoan/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _taiKhoanRepo.GetByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Địa chỉ email này chưa được đăng ký trong hệ thống. Vui lòng kiểm tra lại!");
                    return View(model);
                }

                if (!user.TrangThai)
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản liên kết với email này đã bị khóa. Vui lòng liên hệ Quản trị viên để được hỗ trợ.");
                    return View(model);
                }

                // Chặn khôi phục email với Admin/Nhân viên
                if (user.VaiTro != VaiTro.HocVien)
                {
                    ModelState.AddModelError(string.Empty, "Vì lý do an toàn bảo mật hệ thống, tài khoản Quản trị viên và Nhân viên không hỗ trợ tự khôi phục trực tuyến. Vui lòng liên hệ trực tiếp Bộ phận Kỹ thuật.");
                    return View(model);
                }

                // Tạo token đổi mật khẩu (hạn 15 phút)
                var token = PasswordHelper.GenerateResetToken(user.Email, user.MatKhau, expireMinutes: 15);
                var resetLink = Url.Action("ResetPassword", "TaiKhoan", new { email = user.Email, token }, Request.Scheme);

                ViewBag.SuccessMessage = $"Hệ thống đã ghi nhận yêu cầu và gửi liên kết khôi phục tới hòm thư ({user.Email}). Vui lòng kiểm tra hộp thư đến (và mục Spam) để hoàn tất.";
                ViewBag.UserEmail = user.Email;

                // Hiện link test khi ở môi trường Dev
                if (_env.IsDevelopment())
                {
                    ViewBag.DevResetLink = resetLink;
                }

                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Không thể kết nối đến máy chủ CSDL hoặc xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
                return View(model);
            }
        }

        // GET: /TaiKhoan/ResetPassword
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string? email, string? token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                TempData["ErrorMessage"] = "Yêu cầu đặt lại mật khẩu không hợp lệ hoặc thiếu thông tin xác thực.";
                return RedirectToAction(nameof(ForgotPassword));
            }

            try
            {
                var user = await _taiKhoanRepo.GetByEmailAsync(email);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin tài khoản gắn với yêu cầu này.";
                    return RedirectToAction(nameof(ForgotPassword));
                }

                if (!PasswordHelper.VerifyResetToken(email, user.MatKhau, token, out var errorMsg))
                {
                    TempData["ErrorMessage"] = errorMsg;
                    return RedirectToAction(nameof(ForgotPassword));
                }

                return View(new ResetPasswordViewModel
                {
                    Email = email,
                    Token = token
                });
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Không thể kết nối đến CSDL. Vui lòng thử lại sau giây lát.";
                return RedirectToAction(nameof(ForgotPassword));
            }
        }

        // POST: /TaiKhoan/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _taiKhoanRepo.GetByEmailAsync(model.Email ?? "");
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Không tìm thấy thông tin tài khoản tương ứng.");
                    return View(model);
                }

                if (!user.TrangThai)
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản của bạn hiện đang bị khóa.");
                    return View(model);
                }

                string errorMsg = "Thông tin xác thực không hợp lệ.";
                if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Token) || !PasswordHelper.VerifyResetToken(model.Email, user.MatKhau, model.Token, out errorMsg))
                {
                    ModelState.AddModelError(string.Empty, errorMsg);
                    return View(model);
                }

                var (success, error) = await _taiKhoanRepo.ResetPasswordAsync(model.Email, model.MatKhauMoi);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, error ?? "Lỗi không xác định.");
                    return View(model);
                }

                TempData["SuccessMessage"] = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập với mật khẩu mới.";
                return RedirectToAction(nameof(Login));
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Không thể kết nối đến máy chủ CSDL hoặc xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
                return View(model);
            }
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

            // 1. Kiểm tra xác nhận đồng ý điều khoản đào tạo
            if (!model.DongYDieuKhoan)
            {
                ModelState.AddModelError(nameof(model.DongYDieuKhoan), "Bạn cần đồng ý với Quy chế đào tạo và Điều khoản sử dụng hệ thống để đăng ký.");
                return View(model);
            }

            string cleanUsername = (model.TenDangNhap ?? "").Trim();
            string cleanEmail = (model.Email ?? "").Trim();

            try
            {
                // Kiểm tra trùng tên đăng nhập
                if (await _taiKhoanRepo.ExistsTenDangNhapAsync(cleanUsername))
                {
                    ModelState.AddModelError("TenDangNhap", "Tên đăng nhập này đã được sử dụng. Vui lòng chọn tên khác.");
                    return View(model);
                }

                // 3. LINQ Kiểm tra trùng Địa chỉ Email
                if (await _taiKhoanRepo.ExistsEmailAsync(cleanEmail))
                {
                    ModelState.AddModelError("Email", "Địa chỉ email này đã được sử dụng bởi một tài khoản khác. Vui lòng chọn email khác.");
                    return View(model);
                }

                var (success, error) = await _taiKhoanRepo.RegisterAsync(model);
                if (success)
                {
                    TempData["SuccessMessage"] = "Đăng ký tài khoản học viên thành công! Vui lòng đăng nhập để bắt đầu.";
                    return RedirectToAction(nameof(Login));
                }

                ModelState.AddModelError(string.Empty, error ?? "Lỗi không xác định.");
                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Không thể kết nối đến máy chủ CSDL hoặc xảy ra sự cố trong quá trình khởi tạo tài khoản. Vui lòng kiểm tra lại kết nối SQL Server và thử lại.");
                return View(model);
            }
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

            var (success, error) = await _taiKhoanRepo.ChangePasswordAsync(maTaiKhoan.Value, model.MatKhauHienTai, model.MatKhauMoi);
            if (!success)
            {
                ModelState.AddModelError("MatKhauHienTai", error ?? "Lỗi không xác định.");
                return View(model);
            }

            var user = await _taiKhoanRepo.GetByIdAsync(maTaiKhoan.Value);
            TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
            return RedirectBasedOnRole(user?.VaiTro);
        }

        // QUẢN LÝ DANH SÁCH TÀI KHOẢN (ADMIN & NHÂN VIÊN) - THEO MỤC 5.4 ĐỀ 16

        // GET: /TaiKhoan/QuanLy (Admin & Nhân viên tra cứu hồ sơ)
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        [HttpGet]
        public async Task<IActionResult> QuanLy(string? searchString, string? role, bool? trangThai, int page = 1, int pageSize = 10)
        {
            var (list, totalCount, totalAdmins, totalNhanViens, totalHocViens, totalLocked, totalFiltered, totalPages) =
                await _taiKhoanRepo.GetListAsync(searchString, role, trangThai, page, pageSize);

            ViewBag.TotalCount = totalCount;
            ViewBag.TotalAdmins = totalAdmins;
            ViewBag.TotalNhanViens = totalNhanViens;
            ViewBag.TotalHocViens = totalHocViens;
            ViewBag.TotalLocked = totalLocked;
            ViewBag.TotalFilteredItems = totalFiltered;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            if (!string.IsNullOrWhiteSpace(searchString))
                ViewBag.CurrentSearch = searchString;
            if (!string.IsNullOrWhiteSpace(role))
                ViewBag.CurrentRole = role;
            if (trangThai.HasValue)
                ViewBag.CurrentTrangThai = trangThai;

            return View(list);
        }

        // GET: /TaiKhoan/Index -> chuyển hướng về QuanLy
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        [HttpGet]
        public IActionResult Index(string? searchString, string? role, bool? trangThai, int page = 1, int pageSize = 10)
        {
            return RedirectToAction(nameof(QuanLy), new { searchString, role, trangThai, page, pageSize });
        }

        // THÊM MỚI & CHỈNH SỬA TÀI KHOẢN TRỰC TIẾP (DÀNH CHO ADMIN)

        // GET: /TaiKhoan/Create
        [AuthorizeRole(VaiTro.Admin)]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateUserViewModel());
        }

        // POST: /TaiKhoan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(VaiTro.Admin)]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string cleanUsername = (model.TenDangNhap ?? "").Trim();
            string cleanEmail = (model.Email ?? "").Trim();

            // 1. Kiểm tra trùng tên đăng nhập
            if (await _taiKhoanRepo.ExistsTenDangNhapAsync(cleanUsername))
            {
                ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập này đã được sử dụng trong hệ thống.");
                return View(model);
            }

            // 2. Kiểm tra trùng địa chỉ email
            if (await _taiKhoanRepo.ExistsEmailAsync(cleanEmail))
            {
                ModelState.AddModelError(nameof(model.Email), "Địa chỉ email này đã được sử dụng bởi một tài khoản khác.");
                return View(model);
            }

            var (success, error) = await _taiKhoanRepo.CreateByAdminAsync(model);
            if (success)
            {
                TempData["SuccessMessage"] = $"Đã tạo mới tài khoản '{model.TenDangNhap}' ({VaiTro.LayTenHienThi(model.VaiTro)}) thành công!";
                return RedirectToAction(nameof(QuanLy));
            }

            ModelState.AddModelError(string.Empty, error ?? "Lỗi không xác định.");
            return View(model);
        }

        // GET: /TaiKhoan/Edit/5
        [AuthorizeRole(VaiTro.Admin)]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _taiKhoanRepo.GetByIdWithHocVienAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                MaTaiKhoan = user.MaTaiKhoan,
                TenDangNhap = user.TenDangNhap,
                HoTen = user.HoTen,
                Email = user.Email,
                VaiTro = user.VaiTro,
                SoDienThoai = user.HocVien?.SoDienThoai,
                TrangThai = user.TrangThai
            };

            return View(model);
        }

        // POST: /TaiKhoan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(VaiTro.Admin)]
        public async Task<IActionResult> Edit(int id, EditUserViewModel model)
        {
            if (id != model.MaTaiKhoan)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string cleanEmail = (model.Email ?? "").Trim();

            // Kiểm tra trùng email với tài khoản khác
            if (await _taiKhoanRepo.ExistsEmailAsync(cleanEmail, id))
            {
                ModelState.AddModelError(nameof(model.Email), "Địa chỉ email này đã được sử dụng bởi một tài khoản khác.");
                return View(model);
            }

            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");

            // Không cho phép tự khóa tài khoản hiện tại
            if (currentUserId == id && !model.TrangThai)
            {
                ModelState.AddModelError(nameof(model.TrangThai), "Bạn không thể tự khóa tài khoản Admin đang đăng nhập của chính mình!");
                return View(model);
            }

            // Không cho phép tự hạ quyền Admin hiện tại
            if (currentUserId == id && model.VaiTro != VaiTro.Admin)
            {
                ModelState.AddModelError(nameof(model.VaiTro), "Bạn không thể tự hạ quyền Admin của tài khoản đang đăng nhập!");
                return View(model);
            }

            // Chặn khóa hoặc hạ quyền Admin duy nhất
            var user = await _taiKhoanRepo.GetByIdWithHocVienAsync(id);
            if (user != null && user.VaiTro == VaiTro.Admin && (model.VaiTro != VaiTro.Admin || !model.TrangThai))
            {
                var activeAdminCount = await _taiKhoanRepo.CountActiveAdminsAsync();
                if (activeAdminCount <= 1)
                {
                    ModelState.AddModelError(string.Empty, "Không thể khóa hoặc hạ quyền tài khoản Quản trị viên (Admin) duy nhất còn lại trong hệ thống!");
                    return View(model);
                }
            }

            var (success, error) = await _taiKhoanRepo.UpdateByAdminAsync(id, model);
            if (success)
            {
                TempData["SuccessMessage"] = $"Đã cập nhật thông tin tài khoản '{model.TenDangNhap}' thành công!";
                return RedirectToAction(nameof(QuanLy));
            }

            ModelState.AddModelError(string.Empty, error ?? "Lỗi không xác định.");
            return View(model);
        }

        // POST: /TaiKhoan/ToggleStatus/5
        // Quản trị viên Khóa / Mở khóa tài khoản người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(VaiTro.Admin)]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan") ?? 0;
            var (success, error, newStatus) = await _taiKhoanRepo.ToggleStatusAsync(id, currentUserId);

            if (!success)
            {
                TempData["ErrorMessage"] = error;
            }
            else
            {
                var user = await _taiKhoanRepo.GetByIdAsync(id);
                TempData["SuccessMessage"] = $"Đã {(newStatus ? "mở khóa" : "khóa")} tài khoản '{user?.TenDangNhap}' thành công.";
            }

            return RedirectToAction(nameof(QuanLy));
        }

        // POST: /TaiKhoan/ResetPasswordByAdmin/5
        // Admin đặt lại mật khẩu về mặc định 123456 cho người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(VaiTro.Admin)]
        public async Task<IActionResult> ResetPasswordByAdmin(int id)
        {
            var (success, message) = await _taiKhoanRepo.ResetPasswordByAdminAsync(id);

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

        // POST: /TaiKhoan/UpdateRole/5
        // Admin phân quyền / thay đổi vai trò tài khoản
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(VaiTro.Admin)]
        public async Task<IActionResult> UpdateRole(int id, string vaiTro)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan") ?? 0;
            var (success, message) = await _taiKhoanRepo.UpdateRoleAsync(id, vaiTro, currentUserId);

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

        // Điều hướng thông minh theo vai trò
        private IActionResult RedirectBasedOnRole(string? role)
        {
            return role switch
            {
                VaiTro.Admin => RedirectToAction("Dashboard", "QuanTri"),
                VaiTro.NhanVien => RedirectToAction("Dashboard", "QuanTri"),
                _ => RedirectToAction("Index", "Home")
            };
        }
    }
}
