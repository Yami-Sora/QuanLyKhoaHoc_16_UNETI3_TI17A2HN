// Họ và tên: Trần Văn Thành - MSV: 23103100076 (Module 1: Dashboard quản trị khung nền)
// Họ và tên: Nguyễn Công Thành - MSV: 23103100102 (Module 5: Nâng cấp 9 Stat Cards & Biểu đồ Chart.js thống kê LINQ)

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    [AuthorizeRole(VaiTro.Admin, VaiTro.NhanVien)]
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
            var model = new DashboardViewModel();

            // =================================================================
            // 1. LINQ QUERY 9 STAT CARDS CHÍNH THEO YÊU CẦU ĐỀ BÀI (Mục 9.3)
            // =================================================================
            model.TongSoMonHoc = await _context.MonHocs.CountAsync();
            model.TongSoKhoaHoc = await _context.KhoaHocs.CountAsync();
            model.SoKhoaHocKhaDung = await _context.KhoaHocs.CountAsync(k => k.TrangThai == "DangMo");
            model.TongSoHocVien = await _context.HocViens.CountAsync();
            model.TongSoGiaoDich = await _context.DangKyKhoaHocs.CountAsync();
            model.SoGiaoDichChoXuLy = await _context.DangKyKhoaHocs.CountAsync(d => d.TrangThai == "ChoXuLy");
            model.SoGiaoDichDangXuLy = await _context.DangKyKhoaHocs.CountAsync(d => d.TrangThai == "DangXuLy");
            model.SoGiaoDichHoanThanh = await _context.DangKyKhoaHocs.CountAsync(d => d.TrangThai == "HoanThanh");
            model.TongDoanhThu = await _context.DangKyKhoaHocs.SumAsync(d => (decimal?)d.SoTienDaDong) ?? 0;

            // =================================================================
            // 2. CÁC CHỈ SỐ BỔ SUNG PHỤC VỤ QUẢN TRỊ TOÀN DIỆN
            // =================================================================
            model.TongCongNo = await _context.DangKyKhoaHocs
                .Where(d => d.TrangThai != "BiHuy")
                .SumAsync(d => (decimal?)d.SoTienConLai) ?? 0;
            model.SoGiaoDichBiHuy = await _context.DangKyKhoaHocs.CountAsync(d => d.TrangThai == "BiHuy");
            model.SoKhoaHocDangHoc = await _context.KhoaHocs.CountAsync(k => k.TrangThai == "DangHoc");
            model.SoKhoaHocDaKetThuc = await _context.KhoaHocs.CountAsync(k => k.TrangThai == "DaKetThuc");
            model.TongSoGiangVien = await _context.GiangViens.CountAsync();

            // Duy trì ViewBag cho tương thích ngược nếu có view phụ dùng
            ViewBag.TongSoMonHoc = model.TongSoMonHoc;
            ViewBag.MonHocDangMo = await _context.MonHocs.CountAsync(m => m.TrangThai);
            ViewBag.TongSoTaiKhoan = await _context.TaiKhoans.CountAsync();
            ViewBag.TaiKhoanHoatDong = await _context.TaiKhoans.CountAsync(t => t.TrangThai);

            // =================================================================
            // 3. DỮ LIỆU BIỂU ĐỒ CHART.JS (LINQ GROUPBY & AGGREGATE)
            // =================================================================

            // Biểu đồ 1: Trạng thái giao dịch (Doughnut)
            model.BieuDoDangKyTheoTrangThai = new List<ChartDataItem>
            {
                new ChartDataItem { Label = "Chờ xử lý", Value = model.SoGiaoDichChoXuLy, Color = "#f59e0b" },
                new ChartDataItem { Label = "Đang xử lý", Value = model.SoGiaoDichDangXuLy, Color = "#3b82f6" },
                new ChartDataItem { Label = "Hoàn thành", Value = model.SoGiaoDichHoanThanh, Color = "#10b981" },
                new ChartDataItem { Label = "Đã hủy", Value = model.SoGiaoDichBiHuy, Color = "#ef4444" }
            };

            // Biểu đồ 2: Top khóa học có nhiều học viên đăng ký nhất (Bar)
            var topKhoaHocData = await _context.KhoaHocs
                .Select(k => new
                {
                    TenKhoa = k.TenKhoaHoc,
                    SoLuong = k.DangKyKhoaHocs.Count()
                })
                .OrderByDescending(x => x.SoLuong)
                .Take(6)
                .ToListAsync();

            model.BieuDoHocVienTheoKhoa = topKhoaHocData.Select(x => new ChartDataItem
            {
                Label = x.TenKhoa.Length > 25 ? x.TenKhoa.Substring(0, 22) + "..." : x.TenKhoa,
                Value = x.SoLuong,
                Color = "#2563eb"
            }).ToList();

            // Biểu đồ 3: Đăng ký theo 6 tháng gần nhất (Line)
            var now = DateTime.Today;
            var sixMonthsAgo = new DateTime(now.Year, now.Month, 1).AddMonths(-5);
            var dangKyTheoThangRaw = await _context.DangKyKhoaHocs
                .Where(d => d.NgayDangKy >= sixMonthsAgo)
                .GroupBy(d => new { d.NgayDangKy.Year, d.NgayDangKy.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Count = g.Count()
                })
                .ToListAsync();

            var chartTheoThang = new List<ChartDataItem>();
            for (int i = 0; i < 6; i++)
            {
                var targetDate = sixMonthsAgo.AddMonths(i);
                var found = dangKyTheoThangRaw.FirstOrDefault(x => x.Year == targetDate.Year && x.Month == targetDate.Month);
                chartTheoThang.Add(new ChartDataItem
                {
                    Label = $"T{targetDate.Month}/{targetDate.Year}",
                    Value = found?.Count ?? 0,
                    Color = "#8b5cf6"
                });
            }
            model.BieuDoDangKyTheoThang = chartTheoThang;

            // Biểu đồ 4: Tỷ lệ Đạt / Không đạt (Pie)
            var soKetQuaDat = await _context.KetQuaHocTaps.CountAsync(k => k.KetQua == "Dat");
            var soKetQuaKhongDat = await _context.KetQuaHocTaps.CountAsync(k => k.KetQua == "KhongDat");
            var soChuaDanhGia = await _context.KetQuaHocTaps.CountAsync(k => k.KetQua != "Dat" && k.KetQua != "KhongDat");

            model.BieuDoTyLeDatKhongDat = new List<ChartDataItem>
            {
                new ChartDataItem { Label = "Đạt yêu cầu", Value = soKetQuaDat, Color = "#10b981" },
                new ChartDataItem { Label = "Không đạt", Value = soKetQuaKhongDat, Color = "#ef4444" },
                new ChartDataItem { Label = "Chưa đánh giá", Value = soChuaDanhGia, Color = "#94a3b8" }
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Dashboard));
        }
    }
}
