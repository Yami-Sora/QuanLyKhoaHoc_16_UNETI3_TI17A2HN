// Họ và tên: Nguyễn Công Thành
// Mã sinh viên: 23103100102
// Nội dung thực hiện: Module 5 - Controller Thống kê Báo cáo LINQ tổng hợp

using Microsoft.AspNetCore.Mvc;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    [AuthorizeRole("Admin", "NhanVien")]
    public class ThongKeController : Controller
    {
        private readonly IThongKeRepository _thongKeRepo;
        private readonly ILogger<ThongKeController> _logger;

        public ThongKeController(IThongKeRepository thongKeRepo, ILogger<ThongKeController> logger)
        {
            _thongKeRepo = thongKeRepo;
            _logger = logger;
        }

        // GET: /ThongKe hoặc /ThongKe/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _thongKeRepo.GetThongKeAsync();
            return View(model);
        }

        // API: Lọc số đăng ký theo tháng phục vụ biểu đồ (Hỗ trợ: 12months, year, all)
        [HttpGet]
        public async Task<IActionResult> GetDangKyTheoThang(string filterType = "12months", int? year = null)
        {
            var result = await _thongKeRepo.GetDangKyTheoThangAsync(filterType, year);
            return Json(result.Select(r => new { label = r.NhanThang, value = r.SoDangKy }));
        }
    }
}
