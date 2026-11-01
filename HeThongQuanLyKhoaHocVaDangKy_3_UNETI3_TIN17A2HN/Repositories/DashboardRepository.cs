// Họ và tên: Nguyễn Công Thành
// Mã sinh viên: 23103100102
// Nội dung thực hiện: Module 5 - Triển khai DashboardRepository (Repository Pattern)

using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DashboardRepository> _logger;

        public DashboardRepository(ApplicationDbContext context, ILogger<DashboardRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(DashboardViewModel Model, int MonHocDangMo, int TongTaiKhoan, int TaiKhoanHoatDong)> GetDashboardAsync()
        {
            var model = new DashboardViewModel();

            // 1. LINQ query 9 Stat Cards chính
            model.TongSoMonHoc = await _context.MonHocs.CountAsync();
            model.TongSoKhoaHoc = await _context.KhoaHocs.CountAsync();
            model.SoKhoaHocKhaDung = await _context.KhoaHocs.CountAsync(k => k.TrangThai == "DangMo");
            model.TongSoHocVien = await _context.HocViens.CountAsync();
            model.TongSoGiaoDich = await _context.DangKyKhoaHocs.CountAsync();
            model.SoGiaoDichChoXuLy = await _context.DangKyKhoaHocs.CountAsync(d => d.TrangThai == "ChoXuLy");
            model.SoGiaoDichDangXuLy = await _context.DangKyKhoaHocs.CountAsync(d => d.TrangThai == "DangXuLy");
            model.SoGiaoDichHoanThanh = await _context.DangKyKhoaHocs.CountAsync(d => d.TrangThai == "HoanThanh");
            model.TongDoanhThu = await _context.DangKyKhoaHocs.SumAsync(d => (decimal?)d.SoTienDaDong) ?? 0;

            // 2. Các chỉ số bổ sung phục vụ quản trị toàn diện
            model.TongCongNo = await _context.DangKyKhoaHocs
                .Where(d => d.TrangThai != "BiHuy")
                .SumAsync(d => (decimal?)d.SoTienConLai) ?? 0;
            model.SoGiaoDichBiHuy = await _context.DangKyKhoaHocs.CountAsync(d => d.TrangThai == "BiHuy");
            model.SoKhoaHocDangHoc = await _context.KhoaHocs.CountAsync(k => k.TrangThai == "DangHoc");
            model.SoKhoaHocDaKetThuc = await _context.KhoaHocs.CountAsync(k => k.TrangThai == "DaKetThuc");
            model.TongSoGiangVien = await _context.GiangViens.CountAsync();

            // Tính toán các chỉ số bổ trợ cho ViewBag
            int monHocDangMo = await _context.MonHocs.CountAsync(m => m.TrangThai);
            int tongTaiKhoan = await _context.TaiKhoans.CountAsync();
            int taiKhoanHoatDong = await _context.TaiKhoans.CountAsync(t => t.TrangThai);

            // 3. Dữ liệu biểu đồ Chart.js (LINQ GroupBy & Aggregate)

            // Biểu đồ 1: Trạng thái giao dịch (Doughnut)
            model.BieuDoDangKyTheoTrangThai = new List<ChartDataItem>
            {
                new ChartDataItem { Label = "Chờ xử lý", Value = model.SoGiaoDichChoXuLy, Color = "#f59e0b" },
                new ChartDataItem { Label = "Đang xử lý", Value = model.SoGiaoDichDangXuLy, Color = "#3b82f6" },
                new ChartDataItem { Label = "Hoàn thành", Value = model.SoGiaoDichHoanThanh, Color = "#10b981" },
                new ChartDataItem { Label = "Đã hủy", Value = model.SoGiaoDichBiHuy, Color = "#f43f5e" }
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
                Label = x.TenKhoa,
                Value = x.SoLuong,
                Color = "#3b82f6"
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
                    Color = "#6366f1"
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
                new ChartDataItem { Label = "Không đạt", Value = soKetQuaKhongDat, Color = "#f43f5e" },
                new ChartDataItem { Label = "Chưa đánh giá", Value = soChuaDanhGia, Color = "#94a3b8" }
            };

            return (model, monHocDangMo, tongTaiKhoan, taiKhoanHoatDong);
        }
    }
}
