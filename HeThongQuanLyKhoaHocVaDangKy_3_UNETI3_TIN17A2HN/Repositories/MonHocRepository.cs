// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Nội dung thực hiện: Module 1 - Triển khai MonHocRepository (Repository Pattern)

using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories
{
    public class MonHocRepository : IMonHocRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MonHocRepository> _logger;

        public MonHocRepository(ApplicationDbContext context, ILogger<MonHocRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 1. Danh sách môn học công khai (tìm kiếm, lọc, sắp xếp, phân trang)
        public async Task<(List<MonHoc> Items, int TotalCount, int TotalFiltered, int TotalPages)> GetPublicListAsync(string? searchString, int? soTinChi, bool? trangThai, string? sortBy, int page, int pageSize)
        {
            var query = _context.MonHocs.AsNoTracking().AsQueryable();

            // Tìm kiếm theo tên môn học hoặc mã môn học
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var trimmed = searchString.Trim();
                var term = trimmed.ToLower();
                var cleanCode = term.Replace("#", "").Replace("mh", "").TrimStart('0');
                bool isNumeric = int.TryParse(cleanCode, out int searchId);

                query = query.Where(m => m.TenMonHoc.ToLower().Contains(term)
                                      || (isNumeric && m.MaMonHoc == searchId)
                                      || m.MaMonHoc.ToString().Contains(term));
            }

            // Lọc theo số tín chỉ (2, 3, 4, 5 tín chỉ)
            if (soTinChi.HasValue && soTinChi.Value > 0)
            {
                query = query.Where(m => m.SoTinChi == soTinChi.Value);
            }

            // Lọc theo trạng thái Đang mở / Tạm dừng
            if (trangThai.HasValue)
            {
                query = query.Where(m => m.TrangThai == trangThai.Value);
            }

            // Sắp xếp dữ liệu linh hoạt
            query = sortBy switch
            {
                "ten_asc" => query.OrderBy(m => m.TenMonHoc),
                "ten_desc" => query.OrderByDescending(m => m.TenMonHoc),
                "tinchi_asc" => query.OrderBy(m => m.SoTinChi).ThenBy(m => m.TenMonHoc),
                "tinchi_desc" => query.OrderByDescending(m => m.SoTinChi).ThenBy(m => m.TenMonHoc),
                "hocphi_asc" => query.OrderBy(m => m.HocPhi).ThenBy(m => m.TenMonHoc),
                "hocphi_desc" => query.OrderByDescending(m => m.HocPhi).ThenBy(m => m.TenMonHoc),
                _ => query.OrderBy(m => m.MaMonHoc)
            };

            // Tổng số môn toàn trường và số môn sau khi lọc
            var totalCount = await _context.MonHocs.CountAsync();
            var totalFiltered = await query.CountAsync();

            // Phân trang
            if (page < 1) page = 1;
            var totalPages = (int)Math.Ceiling(totalFiltered / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages) page = totalPages;

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount, totalFiltered, totalPages);
        }

        // 2. Chi tiết môn học kèm các khóa học liên quan
        public async Task<MonHoc?> GetDetailsAsync(int id)
        {
            return await _context.MonHocs
                .Include(m => m.KhoaHocs)
                    .ThenInclude(k => k.GiangVien)
                .Include(m => m.KhoaHocs)
                    .ThenInclude(k => k.DangKyKhoaHocs)
                .FirstOrDefaultAsync(m => m.MaMonHoc == id);
        }

        // 3. Danh sách khóa học mà học viên đã đăng ký (theo mã tài khoản)
        public async Task<List<int>> GetRegisteredKhoaHocIdsAsync(int maTaiKhoan)
        {
            var hocVien = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);
            if (hocVien == null) return new List<int>();

            return await _context.DangKyKhoaHocs
                .Where(d => d.MaHocVien == hocVien.MaHocVien && d.TrangThai != "BiHuy")
                .Select(d => d.MaKhoaHoc)
                .ToListAsync();
        }

        // 4. Danh sách quản trị (tìm kiếm, lọc, phân trang) với stat cards
        public async Task<(List<MonHoc> Items, int TotalCount, int ActiveCount, int InactiveCount, int TotalTinChi, int AvgHocPhi, int TotalFiltered, int TotalPages)> GetAdminListAsync(string? searchString, bool? trangThai, int page, int pageSize)
        {
            // Thống kê số liệu tổng thể phục vụ 4 thẻ Stat Cards
            var totalCount = await _context.MonHocs.CountAsync();
            var activeCount = await _context.MonHocs.CountAsync(m => m.TrangThai);
            var inactiveCount = await _context.MonHocs.CountAsync(m => !m.TrangThai);
            var totalTinChi = await _context.MonHocs.SumAsync(m => m.SoTinChi);
            var avgHocPhi = (int)(await _context.MonHocs.AnyAsync()
                ? await _context.MonHocs.AverageAsync(m => (double)m.HocPhi)
                : 0);

            var query = _context.MonHocs.Include(m => m.KhoaHocs).AsQueryable();

            // Tìm kiếm theo tên hoặc mã môn
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.Trim().ToLower();
                var cleanCode = term.Replace("#", "").Replace("mh", "").TrimStart('0');
                bool isNumeric = int.TryParse(cleanCode, out int searchId);

                query = query.Where(m => m.TenMonHoc.ToLower().Contains(term)
                                      || (isNumeric && m.MaMonHoc == searchId)
                                      || m.MaMonHoc.ToString().Contains(term));
            }

            // LINQ Lọc theo trạng thái Đang mở / Tạm dừng
            if (trangThai.HasValue)
            {
                query = query.Where(m => m.TrangThai == trangThai.Value);
            }

            var totalFiltered = await query.CountAsync();

            if (page < 1) page = 1;
            var totalPages = (int)Math.Ceiling(totalFiltered / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages) page = totalPages;

            var items = await query
                .OrderByDescending(m => m.MaMonHoc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount, activeCount, inactiveCount, totalTinChi, avgHocPhi, totalFiltered, totalPages);
        }

        // 5. Lấy môn học theo Id
        public async Task<MonHoc?> GetByIdAsync(int id)
        {
            return await _context.MonHocs.FindAsync(id);
        }

        // 6. Lấy môn học theo Id kèm danh sách khóa học
        public async Task<MonHoc?> GetByIdWithKhoaHocsAsync(int id)
        {
            return await _context.MonHocs
                .Include(m => m.KhoaHocs)
                .FirstOrDefaultAsync(m => m.MaMonHoc == id);
        }

        // 7. Kiểm tra trùng tên môn học
        public async Task<bool> ExistsTenMonHocAsync(string tenMonHoc, int? excludeId = null)
        {
            var clean = (tenMonHoc ?? "").Trim().ToLower();
            if (excludeId.HasValue)
            {
                return await _context.MonHocs.AnyAsync(m => m.MaMonHoc != excludeId.Value && m.TenMonHoc.Trim().ToLower() == clean);
            }
            return await _context.MonHocs.AnyAsync(m => m.TenMonHoc.Trim().ToLower() == clean);
        }

        // 8. Kiểm tra môn học có khóa học đang hoạt động
        public async Task<bool> HasActiveKhoaHocAsync(int maMonHoc)
        {
            return await _context.KhoaHocs.AnyAsync(k => k.MaMonHoc == maMonHoc && (k.TrangThai == "DangMo" || k.TrangThai == "DangHoc" || k.TrangThai == "SapMo"));
        }

        // 9. Kiểm tra môn học có khóa học tham chiếu
        public async Task<bool> HasAnyKhoaHocAsync(int maMonHoc)
        {
            return await _context.KhoaHocs.AnyAsync(k => k.MaMonHoc == maMonHoc);
        }

        // 10. Thêm mới môn học
        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(MonHoc monHoc)
        {
            try
            {
                monHoc.TenMonHoc = monHoc.TenMonHoc?.Trim() ?? string.Empty;
                monHoc.MoTa = monHoc.MoTa?.Trim();
                _context.Add(monHoc);
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm mới môn học");
                return (false, "Không thể lưu môn học do lỗi kết nối CSDL. Vui lòng thử lại sau.");
            }
        }

        // 11. Cập nhật môn học
        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, MonHoc monHoc)
        {
            try
            {
                var existing = await _context.MonHocs.FindAsync(id);
                if (existing == null)
                {
                    return (false, "Không tìm thấy môn học.");
                }

                existing.TenMonHoc = monHoc.TenMonHoc?.Trim() ?? string.Empty;
                existing.SoTinChi = monHoc.SoTinChi;
                existing.HocPhi = monHoc.HocPhi;
                existing.MoTa = monHoc.MoTa?.Trim();
                existing.TrangThai = monHoc.TrangThai;

                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.MonHocs.AnyAsync(e => e.MaMonHoc == id))
                {
                    return (false, "Không tìm thấy môn học.");
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật môn học ID {Id}", id);
                return (false, "Không thể cập nhật môn học do sự cố CSDL. Vui lòng thử lại.");
            }
        }

        // 12. Xóa môn học
        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            try
            {
                // BẢO VỆ TOÀN VẸN DỮ LIỆU BẮT BUỘC: Kiểm tra trước khi xóa
                bool daCoKhoaHoc = await HasAnyKhoaHocAsync(id);
                if (daCoKhoaHoc)
                {
                    return (false, "Không thể xóa môn học này vì đã có khóa học đang liên kết. Bạn có thể sửa trạng thái môn học sang 'Tạm dừng'!");
                }

                var monHoc = await _context.MonHocs.FindAsync(id);
                if (monHoc == null)
                {
                    return (false, "Không tìm thấy môn học cần xóa hoặc môn học đã được xóa trước đó.");
                }

                _context.MonHocs.Remove(monHoc);
                await _context.SaveChangesAsync();
                return (true, $"Đã xóa môn học '{monHoc.TenMonHoc}' thành công.");
            }
            catch (DbUpdateException)
            {
                return (false, "Không thể xóa môn học này vì có dữ liệu liên kết khác trong hệ thống.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa môn học ID {Id}", id);
                return (false, "Đã xảy ra sự cố khi xóa môn học. Vui lòng thử lại sau.");
            }
        }

        // 13. Đổi trạng thái nhanh Đang mở / Tạm dừng
        public async Task<(bool Success, string? ErrorMessage, bool NewStatus)> ToggleStatusAsync(int id)
        {
            try
            {
                var monHoc = await _context.MonHocs.FindAsync(id);
                if (monHoc == null)
                {
                    return (false, "Không tìm thấy thông tin môn học cần đổi trạng thái.", false);
                }

                monHoc.TrangThai = !monHoc.TrangThai;
                await _context.SaveChangesAsync();
                return (true, $"Đã đổi trạng thái môn '{monHoc.TenMonHoc}' sang {(monHoc.TrangThai ? "Đang mở" : "Tạm dừng")}.", monHoc.TrangThai);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đổi trạng thái môn học ID {Id}", id);
                return (false, "Không thể đổi trạng thái môn học do sự cố CSDL.", false);
            }
        }

        // 14. Xuất danh sách môn học (theo điều kiện lọc)
        public async Task<List<MonHoc>> GetExportListAsync(string? searchString, bool? trangThai)
        {
            var query = _context.MonHocs.Include(m => m.KhoaHocs).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.Trim().ToLower();
                var cleanCode = term.Replace("#", "").Replace("mh", "").TrimStart('0');
                bool isNumeric = int.TryParse(cleanCode, out int searchId);

                query = query.Where(m => m.TenMonHoc.ToLower().Contains(term)
                                      || (isNumeric && m.MaMonHoc == searchId)
                                      || m.MaMonHoc.ToString().Contains(term));
            }

            if (trangThai.HasValue)
            {
                query = query.Where(m => m.TrangThai == trangThai.Value);
            }

            return await query.OrderBy(m => m.MaMonHoc).ToListAsync();
        }
    }
}
