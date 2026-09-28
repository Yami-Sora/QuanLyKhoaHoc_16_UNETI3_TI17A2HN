// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Nội dung thực hiện: Module 1 - Controller Cổng Quản trị / Bàn làm việc & Dashboard trung tâm

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    [AuthorizeRole("Admin", "NhanVien")]
    public class QuanTriController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuanTriController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /QuanTri hoặc /QuanTri/Dashboard
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            // Số liệu từ Module 1 (Trần Văn Thành phụ trách)
            ViewBag.TongSoMonHoc = await _context.MonHocs.CountAsync();
            ViewBag.MonHocDangMo = await _context.MonHocs.CountAsync(m => m.TrangThai);
            ViewBag.TongSoTaiKhoan = await _context.TaiKhoans.CountAsync();
            ViewBag.TaiKhoanHoatDong = await _context.TaiKhoans.CountAsync(t => t.TrangThai);

            return View();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Dashboard));
        }
    }
}
