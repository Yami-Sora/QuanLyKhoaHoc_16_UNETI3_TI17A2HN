// Họ và tên: Trần Văn Thành - MSV: 23103100076 (Module 1: Dashboard quản trị khung nền)
// Họ và tên: Nguyễn Công Thành - MSV: 23103100102 (Module 5: Nâng cấp 9 Stat Cards & Biểu đồ Chart.js thống kê LINQ → Repository Pattern)

using Microsoft.AspNetCore.Mvc;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
    public class QuanTriController : Controller
    {
        private readonly IDashboardRepository _dashboardRepo;
        private readonly ILogger<QuanTriController> _logger;

        public QuanTriController(IDashboardRepository dashboardRepo, ILogger<QuanTriController> logger)
        {
            _dashboardRepo = dashboardRepo;
            _logger = logger;
        }

        // GET: /QuanTri hoặc /QuanTri/Dashboard
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var (model, monHocDangMo, tongTaiKhoan, taiKhoanHoatDong) = await _dashboardRepo.GetDashboardAsync();

            // Duy trì ViewBag cho tương thích ngược nếu có view phụ dùng
            ViewBag.TongSoMonHoc = model.TongSoMonHoc;
            ViewBag.MonHocDangMo = monHocDangMo;
            ViewBag.TongSoTaiKhoan = tongTaiKhoan;
            ViewBag.TaiKhoanHoatDong = taiKhoanHoatDong;

            return View(model);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Dashboard));
        }
    }
}
