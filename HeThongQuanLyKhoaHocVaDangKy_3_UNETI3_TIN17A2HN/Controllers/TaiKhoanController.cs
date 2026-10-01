// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Module 1: Xác thực, tài khoản và phân quyền

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
        private readonly ILogger<TaiKhoanController> _logger;
        private readonly IWebHostEnvironment _env;

        public TaiKhoanController(ApplicationDbContext context, ILogger<TaiKhoanController> logger, IWebHostEnvironment env)
        {
            _context = context;
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
                string cleanUsername = (model.TenDangNhap ?? "").Trim().ToLower();

                // Kiểm tra tài khoản (không phân biệt hoa thường)
                var user = await _context.TaiKhoans
                    .Include(t => t.HocVien)
                    .FirstOrDefaultAsync(t => t.TenDangNhap.ToLower() == cleanUsername);

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
                if (user.VaiTro == "HocVien" && user.HocVien == null)
                {
                    var newHocVien = new HocVien
                    {
                        MaTaiKhoan = user.MaTaiKhoan,
                        HoTen = user.HoTen,
                        Email = user.Email,
                        SoDienThoai = string.Empty,
                        NgayDangKy = DateTime.Now,
                        TrangThai = true
                    };
                    _context.HocViens.Add(newHocVien);
                    await _context.SaveChangesAsync();
                    user.HocVien = newHocVien;
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
                var normalizedEmail = (model.Email ?? "").Trim().ToLower();
                var user = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.Email.ToLower() == normalizedEmail);

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
                var normalizedEmail = email.Trim().ToLower();
                var user = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.Email.ToLower() == normalizedEmail);
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
                var normalizedEmail = (model.Email ?? "").Trim().ToLower();
                var user = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.Email.ToLower() == normalizedEmail);
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

                // Băm và lưu mật khẩu mới
                user.MatKhau = PasswordHelper.HashPassword(model.MatKhauMoi);
                await _context.SaveChangesAsync();

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
            string cleanHoTen = (model.HoTen ?? "").Trim();
            string cleanPhone = string.IsNullOrWhiteSpace(model.SoDienThoai) ? string.Empty : model.SoDienThoai.Trim();

            try
            {
                // Kiểm tra trùng tên đăng nhập
                bool isTaken = await _context.TaiKhoans.AnyAsync(t =>
                    t.TenDangNhap.ToLower() == cleanUsername.ToLower());

                if (isTaken)
                {
                    ModelState.AddModelError("TenDangNhap", "Tên đăng nhập này đã được sử dụng. Vui lòng chọn tên khác.");
                    return View(model);
                }

                // 3. LINQ Kiểm tra trùng Địa chỉ Email
                bool emailTaken = await _context.TaiKhoans.AnyAsync(t =>
                    t.Email.ToLower() == cleanEmail.ToLower());

                if (emailTaken)
                {
                    ModelState.AddModelError("Email", "Địa chỉ email này đã được sử dụng bởi một tài khoản khác. Vui lòng chọn email khác.");
                    return View(model);
                }

                // Dùng transaction lưu tài khoản và học viên
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Tạo tài khoản mới với vai trò Học viên
                    var taiKhoan = new TaiKhoan
                    {
                        TenDangNhap = cleanUsername,
                        MatKhau = PasswordHelper.HashPassword(model.MatKhau),
                        HoTen = cleanHoTen,
                        Email = cleanEmail,
                        VaiTro = VaiTro.HocVien,
                        TrangThai = true,
                        NgayTao = DateTime.Now
                    };

                    _context.TaiKhoans.Add(taiKhoan);
                    await _context.SaveChangesAsync();

                    // Tạo hồ sơ học viên tương ứng
                    var hocVien = new HocVien
                    {
                        MaTaiKhoan = taiKhoan.MaTaiKhoan,
                        HoTen = taiKhoan.HoTen,
                        Email = taiKhoan.Email,
                        SoDienThoai = cleanPhone,
                        NgayDangKy = DateTime.Now,
                        TrangThai = true
                    };
                    _context.HocViens.Add(hocVien);
                    await _context.SaveChangesAsync();

                    // Cam kết Transaction thành công trọn vẹn
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = "Đăng ký tài khoản học viên thành công! Vui lòng đăng nhập để bắt đầu.";
                    return RedirectToAction(nameof(Login));
                }
                catch (DbUpdateException)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc Email có thể đã được người khác sử dụng trong quá trình bạn gửi yêu cầu. Vui lòng thử lại.");
                    return View(model);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw; // Sẽ được bắt ở khối try-catch bên ngoài
                }
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

            try
            {
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
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Không thể cập nhật mật khẩu do lỗi kết nối CSDL. Vui lòng thử lại sau.");
                return View(model);
            }
        }

        // QUẢN LÝ DANH SÁCH TÀI KHOẢN (ADMIN & NHÂN VIÊN) - THEO MỤC 5.4 ĐỀ 16

        // GET: /TaiKhoan/QuanLy (Admin & Nhân viên tra cứu hồ sơ)
        [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
        [HttpGet]
        public async Task<IActionResult> QuanLy(string? searchString, string? role, bool? trangThai, int page = 1, int pageSize = 10)
        {
            var query = _context.TaiKhoans.Include(t => t.HocVien).AsQueryable();

            // LINQ Tìm kiếm đa trường (Tên đăng nhập, Họ tên, Email)
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.Trim().ToLower();
                query = query.Where(t => t.TenDangNhap.ToLower().Contains(term)
                                      || t.HoTen.ToLower().Contains(term)
                                      || t.Email.ToLower().Contains(term));
                ViewBag.CurrentSearch = searchString;
            }

            // LINQ Lọc theo vai trò (Admin, NhanVien, HocVien)
            if (!string.IsNullOrWhiteSpace(role))
            {
                query = query.Where(t => t.VaiTro == role);
                ViewBag.CurrentRole = role;
            }

            // LINQ Lọc theo trạng thái hoạt động / bị khóa
            if (trangThai.HasValue)
            {
                query = query.Where(t => t.TrangThai == trangThai.Value);
                ViewBag.CurrentTrangThai = trangThai;
            }

            ViewBag.TotalCount = await _context.TaiKhoans.CountAsync();
            var totalFiltered = await query.CountAsync();
            ViewBag.TotalFilteredItems = totalFiltered;

            if (page < 1) page = 1;
            var totalPages = (int)Math.Ceiling(totalFiltered / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages) page = totalPages;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            var list = await query.OrderByDescending(t => t.MaTaiKhoan)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

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
            string cleanHoTen = (model.HoTen ?? "").Trim();
            string cleanPhone = (model.SoDienThoai ?? "").Trim();

            // 1. Kiểm tra trùng tên đăng nhập
            if (await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap.ToLower() == cleanUsername.ToLower()))
            {
                ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập này đã được sử dụng trong hệ thống.");
                return View(model);
            }

            // 2. Kiểm tra trùng địa chỉ email
            if (await _context.TaiKhoans.AnyAsync(t => t.Email.ToLower() == cleanEmail.ToLower()))
            {
                ModelState.AddModelError(nameof(model.Email), "Địa chỉ email này đã được sử dụng bởi một tài khoản khác.");
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var taiKhoan = new TaiKhoan
                {
                    TenDangNhap = cleanUsername,
                    MatKhau = PasswordHelper.HashPassword(model.MatKhau),
                    HoTen = cleanHoTen,
                    Email = cleanEmail,
                    VaiTro = model.VaiTro,
                    TrangThai = model.TrangThai,
                    NgayTao = DateTime.Now
                };

                _context.TaiKhoans.Add(taiKhoan);
                await _context.SaveChangesAsync();

                // Nếu vai trò là Học viên, tạo hồ sơ HocVien đồng bộ
                if (model.VaiTro == VaiTro.HocVien)
                {
                    var hocVien = new HocVien
                    {
                        MaTaiKhoan = taiKhoan.MaTaiKhoan,
                        HoTen = cleanHoTen,
                        Email = cleanEmail,
                        SoDienThoai = cleanPhone,
                        NgayDangKy = DateTime.Now,
                        TrangThai = model.TrangThai
                    };
                    _context.HocViens.Add(hocVien);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                TempData["SuccessMessage"] = $"Đã tạo mới tài khoản '{taiKhoan.TenDangNhap}' ({VaiTro.LayTenHienThi(taiKhoan.VaiTro)}) thành công!";
                return RedirectToAction(nameof(QuanLy));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi tạo mới tài khoản bởi Admin");
                ModelState.AddModelError(string.Empty, "Không thể tạo tài khoản do lỗi kết nối CSDL. Vui lòng thử lại sau.");
                return View(model);
            }
        }

        // GET: /TaiKhoan/Edit/5
        [AuthorizeRole(VaiTro.Admin)]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.TaiKhoans
                .Include(t => t.HocVien)
                .FirstOrDefaultAsync(t => t.MaTaiKhoan == id);

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

            var user = await _context.TaiKhoans
                .Include(t => t.HocVien)
                .FirstOrDefaultAsync(t => t.MaTaiKhoan == id);

            if (user == null)
            {
                return NotFound();
            }

            string cleanEmail = (model.Email ?? "").Trim();
            string cleanHoTen = (model.HoTen ?? "").Trim();
            string cleanPhone = (model.SoDienThoai ?? "").Trim();

            // Kiểm tra trùng email với tài khoản khác
            if (await _context.TaiKhoans.AnyAsync(t => t.MaTaiKhoan != id && t.Email.ToLower() == cleanEmail.ToLower()))
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
            if (user.VaiTro == VaiTro.Admin && (model.VaiTro != VaiTro.Admin || !model.TrangThai))
            {
                var activeAdminCount = await _context.TaiKhoans.CountAsync(t => t.VaiTro == VaiTro.Admin && t.TrangThai);
                if (activeAdminCount <= 1)
                {
                    ModelState.AddModelError(string.Empty, "Không thể khóa hoặc hạ quyền tài khoản Quản trị viên (Admin) duy nhất còn lại trong hệ thống!");
                    return View(model);
                }
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                user.HoTen = cleanHoTen;
                user.Email = cleanEmail;
                user.VaiTro = model.VaiTro;
                user.TrangThai = model.TrangThai;

                // Đồng bộ hồ sơ học viên
                if (user.HocVien != null)
                {
                    user.HocVien.HoTen = cleanHoTen;
                    user.HocVien.Email = cleanEmail;
                    user.HocVien.SoDienThoai = cleanPhone;
                    user.HocVien.TrangThai = model.TrangThai;
                }
                else if (model.VaiTro == VaiTro.HocVien)
                {
                    // Nếu đổi sang vai trò Học viên mà chưa có hồ sơ
                    _context.HocViens.Add(new HocVien
                    {
                        MaTaiKhoan = user.MaTaiKhoan,
                        HoTen = cleanHoTen,
                        Email = cleanEmail,
                        SoDienThoai = cleanPhone,
                        NgayDangKy = DateTime.Now,
                        TrangThai = model.TrangThai
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = $"Đã cập nhật thông tin tài khoản '{user.TenDangNhap}' thành công!";
                return RedirectToAction(nameof(QuanLy));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi cập nhật thông tin tài khoản ID {Id}", id);
                ModelState.AddModelError(string.Empty, "Không thể lưu thông tin do lỗi kết nối CSDL.");
                return View(model);
            }
        }

        // POST: /TaiKhoan/ToggleStatus/5
        // Quản trị viên Khóa / Mở khóa tài khoản người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(VaiTro.Admin)]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (currentUserId == id)
            {
                TempData["ErrorMessage"] = "Bạn không thể tự khóa tài khoản Admin đang đăng nhập của chính mình!";
                return RedirectToAction(nameof(QuanLy));
            }

            try
            {
                var user = await _context.TaiKhoans.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                // Chặn khóa tài khoản Admin duy nhất còn lại đang hoạt động
                if (user.VaiTro == VaiTro.Admin && user.TrangThai)
                {
                    var activeAdminCount = await _context.TaiKhoans.CountAsync(t => t.VaiTro == VaiTro.Admin && t.TrangThai);
                    if (activeAdminCount <= 1)
                    {
                        TempData["ErrorMessage"] = "Không thể khóa tài khoản Quản trị viên (Admin) duy nhất còn lại đang hoạt động trong hệ thống!";
                        return RedirectToAction(nameof(QuanLy));
                    }
                }

                user.TrangThai = !user.TrangThai;

                // Đồng bộ trạng thái hồ sơ học viên tương ứng (nếu có)
                var hocVien = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == id);
                if (hocVien != null)
                {
                    hocVien.TrangThai = user.TrangThai;
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Đã {(user.TrangThai ? "mở khóa" : "khóa")} tài khoản '{user.TenDangNhap}' thành công.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái tài khoản ID {Id}", id);
                TempData["ErrorMessage"] = "Không thể cập nhật trạng thái tài khoản do sự cố CSDL.";
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
            try
            {
                var user = await _context.TaiKhoans.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                // Đặt lại về mật khẩu mặc định 123456
                const string defaultPassword = "123456";

                user.MatKhau = PasswordHelper.HashPassword(defaultPassword);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Đã đặt lại mật khẩu cho '{user.TenDangNhap}' về mặc định: [{defaultPassword}].";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đặt lại mật khẩu cho tài khoản ID {Id}", id);
                TempData["ErrorMessage"] = "Không thể đặt lại mật khẩu do sự cố CSDL.";
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
            if (!VaiTro.DanhSachVaiTro.Contains(vaiTro))
            {
                TempData["ErrorMessage"] = "Vai trò được chọn không hợp lệ trong hệ thống.";
                return RedirectToAction(nameof(QuanLy));
            }

            var currentUserId = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (currentUserId == id && vaiTro != VaiTro.Admin)
            {
                TempData["ErrorMessage"] = "Bạn không thể tự hạ quyền Admin của tài khoản đang đăng nhập!";
                return RedirectToAction(nameof(QuanLy));
            }

            try
            {
                var user = await _context.TaiKhoans.Include(t => t.HocVien).FirstOrDefaultAsync(t => t.MaTaiKhoan == id);
                if (user == null)
                {
                    return NotFound();
                }

                // Chặn hạ quyền Admin duy nhất còn lại trong hệ thống
                if (user.VaiTro == VaiTro.Admin && vaiTro != VaiTro.Admin)
                {
                    var activeAdminCount = await _context.TaiKhoans.CountAsync(t => t.VaiTro == VaiTro.Admin && t.TrangThai);
                    if (activeAdminCount <= 1)
                    {
                        TempData["ErrorMessage"] = "Không thể hạ quyền tài khoản Quản trị viên (Admin) duy nhất còn lại trong hệ thống!";
                        return RedirectToAction(nameof(QuanLy));
                    }
                }

                var oldRole = user.VaiTro;
                user.VaiTro = vaiTro;

                // Tự tạo hồ sơ học viên khi chuyển vai trò
                if (vaiTro == VaiTro.HocVien && user.HocVien == null)
                {
                    _context.HocViens.Add(new HocVien
                    {
                        MaTaiKhoan = user.MaTaiKhoan,
                        HoTen = user.HoTen,
                        Email = user.Email,
                        SoDienThoai = string.Empty,
                        NgayDangKy = DateTime.Now,
                        TrangThai = true
                    });
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Đã cập nhật vai trò của tài khoản '{user.TenDangNhap}' từ [{oldRole}] sang [{vaiTro}] thành công.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật vai trò cho tài khoản ID {Id}", id);
                TempData["ErrorMessage"] = "Không thể cập nhật vai trò tài khoản do sự cố CSDL.";
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
