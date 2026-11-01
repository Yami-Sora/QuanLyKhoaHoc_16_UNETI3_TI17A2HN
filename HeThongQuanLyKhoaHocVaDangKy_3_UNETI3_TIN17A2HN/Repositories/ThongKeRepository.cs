// Họ và tên: Nguyễn Công Thành
// Mã sinh viên: 23103100102
// Nội dung thực hiện: Module 5 - Triển khai ThongKeRepository (Repository Pattern)

using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories
{
    public class ThongKeRepository : IThongKeRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ThongKeRepository> _logger;

        public ThongKeRepository(ApplicationDbContext context, ILogger<ThongKeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Thống kê tổng hợp toàn diện (8 sections báo cáo LINQ)
        public async Task<ThongKeViewModel> GetThongKeAsync()
        {
            var model = new ThongKeViewModel();

            // 1. Số khóa học theo môn học — LINQ GroupBy(), Count(), OrderByDescending()
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

            // 2. Số học viên đăng ký theo khóa — LINQ Select(), Count(), OrderByDescending()
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

            // 3. Top khóa học nhiều học viên nhất — LINQ OrderByDescending(), Take(5)
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

            // 4. Doanh thu học phí — LINQ Sum(), GroupBy(), OrderByDescending()
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

            // 5. Tổng công nợ học phí — LINQ Sum(), Where()
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

            // 6. Điểm trung bình theo khóa — LINQ Average(), GroupBy()
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

            // 7. Tỷ lệ Đạt / Không đạt — LINQ Count(), Any()
            model.TongSoKetQua = await _context.KetQuaHocTaps.CountAsync();
            model.SoDat = await _context.KetQuaHocTaps.CountAsync(k => k.KetQua == "Dat");
            model.SoKhongDat = await _context.KetQuaHocTaps.CountAsync(k => k.KetQua == "KhongDat");

            // 8. Số đăng ký theo tháng — LINQ GroupBy(Year, Month) (Mặc định 12 tháng gần nhất)
            var now8 = DateTime.Today;
            var twelveMonthsAgo = new DateTime(now8.Year, now8.Month, 1).AddMonths(-11);

            var dangKyRaw = await _context.DangKyKhoaHocs
                .Where(d => d.NgayDangKy >= twelveMonthsAgo)
                .GroupBy(d => new { d.NgayDangKy.Year, d.NgayDangKy.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync();

            // Zero-fill: Đảm bảo đủ 12 tháng liên tục, tháng nào không có đăng ký thì gán = 0
            var dangKyGroups = new List<ThongKeDangKyTheoThangViewModel>();
            for (int i = 0; i < 12; i++)
            {
                var target = twelveMonthsAgo.AddMonths(i);
                var found = dangKyRaw.FirstOrDefault(x => x.Year == target.Year && x.Month == target.Month);
                dangKyGroups.Add(new ThongKeDangKyTheoThangViewModel
                {
                    Nam = target.Year,
                    Thang = target.Month,
                    SoDangKy = found?.Count ?? 0
                });
            }
            model.DangKyTheoThang = dangKyGroups;

            // Danh sách các năm có dữ liệu đăng ký (phục vụ bộ lọc biểu đồ Section 8)
            model.DanhSachNamCoDuLieu = await _context.DangKyKhoaHocs
                .Select(d => d.NgayDangKy.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            return model;
        }

        // Lọc số đăng ký theo tháng phục vụ biểu đồ (Hỗ trợ: 12months, year, all)
        public async Task<List<ThongKeDangKyTheoThangViewModel>> GetDangKyTheoThangAsync(string filterType = "12months", int? year = null)
        {
            var result = new List<ThongKeDangKyTheoThangViewModel>();

            if (filterType == "year" && year.HasValue)
            {
                // Lọc theo năm cụ thể: Zero-fill đủ 12 tháng T1 → T12
                var raw = await _context.DangKyKhoaHocs
                    .Where(d => d.NgayDangKy.Year == year.Value)
                    .GroupBy(d => d.NgayDangKy.Month)
                    .Select(g => new { Month = g.Key, Count = g.Count() })
                    .ToListAsync();

                for (int m = 1; m <= 12; m++)
                {
                    var found = raw.FirstOrDefault(x => x.Month == m);
                    result.Add(new ThongKeDangKyTheoThangViewModel
                    {
                        Nam = year.Value,
                        Thang = m,
                        SoDangKy = found?.Count ?? 0
                    });
                }
            }
            else if (filterType == "all")
            {
                // Toàn bộ thời gian: Lấy từ tháng đầu tiên đến tháng hiện tại, zero-fill
                var allRaw = await _context.DangKyKhoaHocs
                    .GroupBy(d => new { d.NgayDangKy.Year, d.NgayDangKy.Month })
                    .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                    .ToListAsync();

                if (allRaw.Any())
                {
                    var minDate = allRaw.Select(x => new DateTime(x.Year, x.Month, 1)).Min();
                    var maxDate = allRaw.Select(x => new DateTime(x.Year, x.Month, 1)).Max();

                    for (var d = minDate; d <= maxDate; d = d.AddMonths(1))
                    {
                        var found = allRaw.FirstOrDefault(x => x.Year == d.Year && x.Month == d.Month);
                        result.Add(new ThongKeDangKyTheoThangViewModel
                        {
                            Nam = d.Year,
                            Thang = d.Month,
                            SoDangKy = found?.Count ?? 0
                        });
                    }
                }
            }
            else
            {
                // Mặc định: 12 tháng gần nhất, zero-fill
                var now = DateTime.Today;
                var start = new DateTime(now.Year, now.Month, 1).AddMonths(-11);
                var raw = await _context.DangKyKhoaHocs
                    .Where(d => d.NgayDangKy >= start)
                    .GroupBy(d => new { d.NgayDangKy.Year, d.NgayDangKy.Month })
                    .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                    .ToListAsync();

                for (int i = 0; i < 12; i++)
                {
                    var target = start.AddMonths(i);
                    var found = raw.FirstOrDefault(x => x.Year == target.Year && x.Month == target.Month);
                    result.Add(new ThongKeDangKyTheoThangViewModel
                    {
                        Nam = target.Year,
                        Thang = target.Month,
                        SoDangKy = found?.Count ?? 0
                    });
                }
            }

            return result;
        }
    }
}
