// Họ và tên: Nguyễn Công Thành
// Mã sinh viên: 23103100102
// Nội dung thực hiện: Module 5 - Controller Thống kê Báo cáo LINQ tổng hợp

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    [AuthorizeRole("Admin", "NhanVien")]
    public class ThongKeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThongKeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /ThongKe hoặc /ThongKe/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new ThongKeViewModel();

            // =====================================================================
            // 1. SỐ KHÓA HỌC THEO MÔN HỌC — LINQ GroupBy(), Count(), OrderByDescending()
            // =====================================================================
            model.KhoaHocTheoMon = await _context.MonHocs
                .Select(m => new ThongKeKhoaHocTheoMonViewModel
                {
                    MaMonHoc = m.MaMonHoc,
                    TenMonHoc = m.TenMonHoc,
                    SoTinChi = m.SoTinChi,
                    SoKhoaHoc = m.KhoaHocs.Count()
                })
                .OrderByDescending(x => x.SoKhoaHoc)
                .ThenBy(x => x.TenMonHoc)
                .ToListAsync();

            // =====================================================================
            // 2. SỐ HỌC VIÊN ĐĂNG KÝ THEO KHÓA — LINQ Select(), Count(), OrderByDescending()
            // =====================================================================
            model.HocVienTheoKhoa = await _context.KhoaHocs
                .Select(k => new ThongKeHocVienTheoKhoaViewModel
                {
                    MaKhoaHoc = k.MaKhoaHoc,
                    TenKhoaHoc = k.TenKhoaHoc,
                    TenMonHoc = k.MonHoc != null ? k.MonHoc.TenMonHoc : null,
                    SoHocVienDangKy = k.DangKyKhoaHocs.Count(d => d.TrangThai != "BiHuy"),
                    SoLuongToiDa = k.SoLuongToiDa
                })
                .OrderByDescending(x => x.SoHocVienDangKy)
                .ToListAsync();

            // =====================================================================
            // 3. TOP KHÓA HỌC NHIỀU HỌC VIÊN NHẤT — LINQ OrderByDescending(), Take(5)
            // =====================================================================
            model.TopKhoaHocNhieuHocVien = await _context.KhoaHocs
                .Select(k => new ThongKeTopKhoaHocViewModel
                {
                    MaKhoaHoc = k.MaKhoaHoc,
                    TenKhoaHoc = k.TenKhoaHoc,
                    TenMonHoc = k.MonHoc != null ? k.MonHoc.TenMonHoc : null,
                    TenGiangVien = k.GiangVien != null ? k.GiangVien.HoTen : null,
                    SoHocVien = k.DangKyKhoaHocs.Count(d => d.TrangThai != "BiHuy"),
                    SoLuongToiDa = k.SoLuongToiDa,
                    HocPhi = k.HocPhi
                })
                .OrderByDescending(x => x.SoHocVien)
                .Take(5)
                .ToListAsync();

            // =====================================================================
            // 4. DOANH THU HỌC PHÍ — LINQ Sum(), GroupBy(), OrderByDescending()
            // =====================================================================
            model.TongDoanhThu = await _context.DangKyKhoaHocs.SumAsync(d => (decimal?)d.SoTienDaDong) ?? 0;

            model.DoanhThuTheoKhoa = await _context.KhoaHocs
                .Select(k => new ThongKeDoanhThuTheoKhoaViewModel
                {
                    MaKhoaHoc = k.MaKhoaHoc,
                    TenKhoaHoc = k.TenKhoaHoc,
                    TenMonHoc = k.MonHoc != null ? k.MonHoc.TenMonHoc : null,
                    DoanhThu = k.DangKyKhoaHocs.Sum(d => (decimal?)d.SoTienDaDong) ?? 0,
                    SoHocVienDaDong = k.DangKyKhoaHocs.Count(d => d.SoTienDaDong > 0)
                })
                .OrderByDescending(x => x.DoanhThu)
                .ToListAsync();

            // =====================================================================
            // 5. TỔNG CÔNG NỢ HỌC PHÍ — LINQ Sum(), Where()
            // =====================================================================
            model.TongCongNo = await _context.DangKyKhoaHocs
                .Where(d => d.TrangThai != "BiHuy")
                .SumAsync(d => (decimal?)d.SoTienConLai) ?? 0;

            model.CongNoTheoKhoa = await _context.KhoaHocs
                .Select(k => new ThongKeCongNoTheoKhoaViewModel
                {
                    MaKhoaHoc = k.MaKhoaHoc,
                    TenKhoaHoc = k.TenKhoaHoc,
                    CongNo = k.DangKyKhoaHocs.Where(d => d.TrangThai != "BiHuy").Sum(d => (decimal?)d.SoTienConLai) ?? 0,
                    SoHocVienConNo = k.DangKyKhoaHocs.Count(d => d.TrangThai != "BiHuy" && d.SoTienConLai > 0)
                })
                .Where(x => x.CongNo > 0)
                .OrderByDescending(x => x.CongNo)
                .ToListAsync();

            // =====================================================================
            // 6. ĐIỂM TRUNG BÌNH THEO KHÓA — LINQ Average(), GroupBy()
            // =====================================================================
            var khoaHocList = await _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.DangKyKhoaHocs)
                    .ThenInclude(d => d.KetQuaHocTap)
                .ToListAsync();

            model.DiemTBTheoKhoa = khoaHocList.Select(k =>
            {
                var dsKetQua = k.DangKyKhoaHocs
                    .Where(d => d.KetQuaHocTap != null)
                    .Select(d => d.KetQuaHocTap!)
                    .ToList();

                var count = dsKetQua.Count;
                var dtb = count > 0 ? Math.Round(dsKetQua.Average(kq => kq.DiemTongKet), 2) : 0;
                var dat = dsKetQua.Count(kq => kq.KetQua == "Dat");
                var khongDat = dsKetQua.Count(kq => kq.KetQua == "KhongDat");

                return new ThongKeDiemTBTheoKhoaViewModel
                {
                    MaKhoaHoc = k.MaKhoaHoc,
                    TenKhoaHoc = k.TenKhoaHoc,
                    TenMonHoc = k.MonHoc?.TenMonHoc,
                    DiemTB = dtb,
                    SoHocVienCoKetQua = count,
                    SoDat = dat,
                    SoKhongDat = khongDat
                };
            })
            .Where(x => x.SoHocVienCoKetQua > 0)
            .OrderByDescending(x => x.DiemTB)
            .ToList();

            // =====================================================================
            // 7. TỶ LỆ ĐẠT / KHÔNG ĐẠT — LINQ Count(), Any()
            // =====================================================================
            model.TongSoKetQua = await _context.KetQuaHocTaps.CountAsync();
            model.SoDat = await _context.KetQuaHocTaps.CountAsync(k => k.KetQua == "Dat");
            model.SoKhongDat = await _context.KetQuaHocTaps.CountAsync(k => k.KetQua == "KhongDat");

            // =====================================================================
            // 8. SỐ ĐĂNG KÝ THEO THÁNG — LINQ GroupBy(Year, Month)
            // =====================================================================
            var dangKyGroups = await _context.DangKyKhoaHocs
                .GroupBy(d => new { d.NgayDangKy.Year, d.NgayDangKy.Month })
                .Select(g => new ThongKeDangKyTheoThangViewModel
                {
                    Nam = g.Key.Year,
                    Thang = g.Key.Month,
                    SoDangKy = g.Count()
                })
                .OrderBy(x => x.Nam)
                .ThenBy(x => x.Thang)
                .ToListAsync();

            model.DangKyTheoThang = dangKyGroups;

            return View(model);
        }
    }
}
