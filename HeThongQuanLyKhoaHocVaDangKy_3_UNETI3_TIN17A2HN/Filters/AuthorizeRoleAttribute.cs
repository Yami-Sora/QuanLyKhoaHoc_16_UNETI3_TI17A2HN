// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Module 1: Bộ lọc phân quyền

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly string[] _acceptedRoles;

        // Nhận danh sách vai trò hợp lệ
        public AuthorizeRoleAttribute(params string[] roles)
        {
            _acceptedRoles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var maTaiKhoan = session.GetInt32("MaTaiKhoan");
            var vaiTro = session.GetString("VaiTro");

            // Chưa đăng nhập -> chuyển về Login
            if (!maTaiKhoan.HasValue || string.IsNullOrEmpty(vaiTro))
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult("Login", "TaiKhoan", new { returnUrl });
                return;
            }

            // Kiểm tra trạng thái tài khoản từ CSDL
            var db = context.HttpContext.RequestServices.GetService<ApplicationDbContext>();
            if (db != null)
            {
                var user = db.TaiKhoans.AsNoTracking().FirstOrDefault(t => t.MaTaiKhoan == maTaiKhoan.Value);
                if (user == null || !user.TrangThai)
                {
                    // Khóa tài khoản -> xóa session và về Login
                    session.Clear();
                    context.Result = new RedirectToActionResult("Login", "TaiKhoan", null);
                    if (context.Controller is Controller controller)
                    {
                        controller.TempData["ErrorMessage"] = "Tài khoản của bạn đã bị khóa hoặc không còn hiệu lực. Vui lòng liên hệ Quản trị viên.";
                    }
                    return;
                }

                // Đồng bộ vai trò nếu Admin vừa đổi
                if (!string.Equals(user.VaiTro, vaiTro, StringComparison.OrdinalIgnoreCase))
                {
                    vaiTro = user.VaiTro;
                    session.SetString("VaiTro", vaiTro);
                    session.SetString("HoTen", user.HoTen);
                }
            }

            // Không đúng quyền -> sang AccessDenied
            if (_acceptedRoles.Length > 0 && !_acceptedRoles.Contains(vaiTro))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "TaiKhoan", null);
                return;
            }

            // 3. Đủ quyền: Ngăn chặn trình duyệt lưu cache HTML trang nội bộ
            var response = context.HttpContext.Response;
            response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            response.Headers["Pragma"] = "no-cache";
            response.Headers["Expires"] = "0";

            base.OnActionExecuting(context);
        }
    }
}
