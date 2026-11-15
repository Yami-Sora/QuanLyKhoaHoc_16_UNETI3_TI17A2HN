// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Controller Trang chủ (Đã chuyển sang Repository Pattern)

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeRepository _homeRepo;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IHomeRepository homeRepo, ILogger<HomeController> logger)
        {
            _homeRepo = homeRepo;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _homeRepo.GetHomeDataAsync();

            // Đồng bộ dữ liệu thống kê sang ViewBag để Views/Home/Index.cshtml hiển thị
            ViewBag.TongSoMon = data.TongSoMon;
            ViewBag.TongMonDangMo = data.SoMonDangMo;
            ViewBag.TongTinChi = data.TongTinChi;
            ViewBag.HocPhiTb = data.HocPhiTb;
            ViewBag.HocPhiThapNhat = data.HocPhiThapNhat;
            ViewBag.TongHocVien = data.TongHocVien;

            return View(data.MonHocTieuBieu);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
