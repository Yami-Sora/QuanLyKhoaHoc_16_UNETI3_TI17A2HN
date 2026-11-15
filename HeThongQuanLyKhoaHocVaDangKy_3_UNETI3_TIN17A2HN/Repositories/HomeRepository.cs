// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Nội dung thực hiện: Module 1 - Triển khai HomeRepository (Repository Pattern)

using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeRepository> _logger;

        public HomeRepository(ApplicationDbContext context, ILogger<HomeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<HomeStatsViewModel> GetHomeDataAsync()
        {
            try
            {
                var tongSoMon = await _context.MonHocs.AsNoTracking().CountAsync();
                var dangMo = _context.MonHocs.AsNoTracking().Where(m => m.TrangThai);
                var soMonDangMo = await dangMo.CountAsync();

                var tongTinChi = await dangMo.SumAsync(m => (int?)m.SoTinChi) ?? 0;
                var hocPhiTb = await dangMo.AverageAsync(m => (decimal?)m.HocPhi) ?? 0m;
                var hocPhiThapNhat = await dangMo.MinAsync(m => (decimal?)m.HocPhi) ?? 0m;

                var tongHocVien = await _context.HocViens.AsNoTracking().CountAsync();

                var monHocs = await dangMo
                    .OrderBy(m => m.MaMonHoc)
                    .Take(4)
                    .ToListAsync();

                return new HomeStatsViewModel
                {
                    TongSoMon = tongSoMon,
                    SoMonDangMo = soMonDangMo,
                    TongTinChi = tongTinChi,
                    HocPhiTb = hocPhiTb,
                    HocPhiThapNhat = hocPhiThapNhat,
                    TongHocVien = tongHocVien,
                    MonHocTieuBieu = monHocs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu trang chủ trong HomeRepository");
                return new HomeStatsViewModel();
            }
        }
    }
}
