// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Controller Trang chủ

using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dangMo = _context.MonHocs.Where(m => m.TrangThai);

            // Thống kê dữ liệu cho trang chủ
            ViewBag.TongMon = await dangMo.CountAsync();
            ViewBag.TongTinChi = await dangMo.SumAsync(m => (int?)m.SoTinChi) ?? 0;
            ViewBag.HocPhiTb = await dangMo.AverageAsync(m => (decimal?)m.HocPhi) ?? 0m;
            ViewBag.HocPhiThapNhat = await dangMo.MinAsync(m => (decimal?)m.HocPhi) ?? 0m;

            // Nạp 4 môn học tiêu biểu đang mở giảng dạy từ CSDL
            var monHocs = await dangMo
                .OrderBy(m => m.MaMonHoc)
                .Take(4)
                .ToListAsync();

            return View(monHocs);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
