// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Nội dung thực hiện: Module 1 - Action Filter kiểm tra phân quyền truy cập tại cấp Controller

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly string[] _acceptedRoles;

        // Cho phép truyền 0, 1 hoặc nhiều vai trò: [AuthorizeRole("Admin")], [AuthorizeRole("Admin", "NhanVien")]
        public AuthorizeRoleAttribute(params string[] roles)
        {
            _acceptedRoles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var maTaiKhoan = session.GetInt32("MaTaiKhoan");
            var vaiTro = session.GetString("VaiTro");

            // 1. Chưa đăng nhập: Chuyển hướng về trang Login kèm tham số returnUrl
            if (!maTaiKhoan.HasValue || string.IsNullOrEmpty(vaiTro))
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult("Login", "TaiKhoan", new { returnUrl });
                return;
            }

            // 2. Không thuộc vai trò được phép: Chuyển hướng sang trang AccessDenied
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
