// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Nội dung thực hiện: Module 1 - Triển khai GiangVienRepository (Repository Pattern)

using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories
{
    public class GiangVienRepository : IGiangVienRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GiangVienRepository> _logger;

        public GiangVienRepository(ApplicationDbContext context, ILogger<GiangVienRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 1. Danh sách giảng viên (tìm kiếm, lọc, phân trang) với stat cards
        public async Task<(List<GiangVien> Items, int TotalCount, int ActiveCount, int InactiveCount, int TotalKhoaHoc, int TotalFiltered, int TotalPages)> GetListAsync(string? searchString, bool? trangThai, bool? coKhoaHoc, int page, int pageSize)
        {
            // Thống kê số liệu cho thẻ Stat Cards
            var totalCount = await _context.GiangViens.CountAsync();
            var activeCount = await _context.GiangViens.CountAsync(g => g.TrangThai);
            var inactiveCount = await _context.GiangViens.CountAsync(g => !g.TrangThai);
            var totalKhoaHoc = await _context.KhoaHocs.CountAsync();

            var query = _context.GiangViens.Include(g => g.KhoaHocs).AsQueryable();

            // Tìm kiếm theo họ tên, email, số điện thoại, chuyên môn hoặc mã giảng viên
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.Trim().ToLower();
                var digits = new string(term.Where(char.IsDigit).ToArray());
                bool isNumeric = int.TryParse(digits, out int searchId);

                query = query.Where(g => g.HoTen.ToLower().Contains(term)
                                      || g.Email.ToLower().Contains(term)
                                      || g.SoDienThoai.Contains(term)
                                      || (g.ChuyenMon != null && g.ChuyenMon.ToLower().Contains(term))
                                      || (isNumeric && g.MaGiangVien == searchId));
            }

            // Lọc theo trạng thái công tác
            if (trangThai.HasValue)
            {
                query = query.Where(g => g.TrangThai == trangThai.Value);
            }

            // Lọc nhanh từ thẻ thống kê "Khóa học phụ trách": chỉ giảng viên đang có lớp
            if (coKhoaHoc == true)
            {
                query = query.Where(g => g.KhoaHocs.Any());
            }

            var totalFiltered = await query.CountAsync();

            if (page < 1) page = 1;
            var totalPages = (int)Math.Ceiling(totalFiltered / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages) page = totalPages;

            var items = await query
                .OrderByDescending(g => g.MaGiangVien)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount, activeCount, inactiveCount, totalKhoaHoc, totalFiltered, totalPages);
        }

        // 2. Xuất danh sách giảng viên (theo điều kiện lọc)
        public async Task<List<GiangVien>> GetExportListAsync(string? searchString, bool? trangThai, bool? coKhoaHoc)
        {
            var query = _context.GiangViens.Include(g => g.KhoaHocs).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.Trim().ToLower();
                query = query.Where(g => g.HoTen.ToLower().Contains(term)
                                      || g.Email.ToLower().Contains(term)
                                      || g.SoDienThoai.Contains(term)
                                      || (g.ChuyenMon != null && g.ChuyenMon.ToLower().Contains(term)));
            }

            if (trangThai.HasValue)
            {
                query = query.Where(g => g.TrangThai == trangThai.Value);
            }

            if (coKhoaHoc == true)
            {
                query = query.Where(g => g.KhoaHocs.Any());
            }

            return await query.OrderBy(g => g.HoTen).ToListAsync();
        }

        // 3. Lấy giảng viên theo Id
        public async Task<GiangVien?> GetByIdAsync(int id)
        {
            return await _context.GiangViens.FindAsync(id);
        }

        // 4. Lấy giảng viên theo Id kèm danh sách khóa học
        public async Task<GiangVien?> GetByIdWithKhoaHocsAsync(int id)
        {
            return await _context.GiangViens
                .Include(g => g.KhoaHocs)
                .FirstOrDefaultAsync(g => g.MaGiangVien == id);
        }

        // 5. Kiểm tra trùng email giảng viên
        public async Task<bool> ExistsEmailAsync(string email, int? excludeId = null)
        {
            var clean = (email ?? "").Trim().ToLower();
            if (excludeId.HasValue)
            {
                return await _context.GiangViens.AnyAsync(g => g.MaGiangVien != excludeId.Value && g.Email.Trim().ToLower() == clean);
            }
            return await _context.GiangViens.AnyAsync(g => g.Email.Trim().ToLower() == clean);
        }

        // 6. Kiểm tra giảng viên có đang phụ trách khóa học
        public async Task<bool> HasAnyKhoaHocAsync(int maGiangVien)
        {
            return await _context.KhoaHocs.AnyAsync(k => k.MaGiangVien == maGiangVien);
        }

        // 7. Thêm mới giảng viên
        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(GiangVien giangVien)
        {
            try
            {
                giangVien.HoTen = giangVien.HoTen?.Trim() ?? string.Empty;
                giangVien.Email = giangVien.Email?.Trim() ?? string.Empty;
                giangVien.SoDienThoai = giangVien.SoDienThoai?.Trim() ?? string.Empty;
                giangVien.ChuyenMon = giangVien.ChuyenMon?.Trim();
                _context.Add(giangVien);
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm mới giảng viên");
                return (false, "Không thể lưu giảng viên do lỗi kết nối CSDL. Vui lòng thử lại sau.");
            }
        }

        // 8. Cập nhật giảng viên
        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, GiangVien giangVien)
        {
            try
            {
                var existing = await _context.GiangViens.FindAsync(id);
                if (existing == null)
                {
                    return (false, "Không tìm thấy giảng viên.");
                }

                existing.HoTen = giangVien.HoTen?.Trim() ?? string.Empty;
                existing.Email = giangVien.Email?.Trim() ?? string.Empty;
                existing.SoDienThoai = giangVien.SoDienThoai?.Trim() ?? string.Empty;
                existing.ChuyenMon = giangVien.ChuyenMon?.Trim();
                existing.HocVi = giangVien.HocVi;
                existing.GioiTinh = giangVien.GioiTinh;
                existing.TrangThai = giangVien.TrangThai;

                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.GiangViens.AnyAsync(e => e.MaGiangVien == id))
                {
                    return (false, "Không tìm thấy giảng viên.");
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật giảng viên ID {Id}", id);
                return (false, "Không thể cập nhật giảng viên do sự cố CSDL. Vui lòng thử lại.");
            }
        }

        // 9. Xóa giảng viên
        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            try
            {
                // BẢO VỆ TOÀN VẸN DỮ LIỆU: không xóa giảng viên đang phụ trách khóa học
                bool dangPhuTrach = await HasAnyKhoaHocAsync(id);
                if (dangPhuTrach)
                {
                    return (false, "Không thể xóa giảng viên này vì đang phụ trách khóa học trong hệ thống. Vui lòng vào Quản lý khóa học để bàn giao hoặc phân công giảng viên khác thay thế trước khi xóa!");
                }

                var giangVien = await _context.GiangViens.FindAsync(id);
                if (giangVien == null)
                {
                    return (false, "Không tìm thấy giảng viên cần xóa hoặc giảng viên đã được xóa trước đó.");
                }

                _context.GiangViens.Remove(giangVien);
                await _context.SaveChangesAsync();
                return (true, $"Đã xóa giảng viên '{giangVien.HoTen}' thành công.");
            }
            catch (DbUpdateException)
            {
                return (false, "Không thể xóa giảng viên này vì có dữ liệu liên kết khác trong hệ thống.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa giảng viên ID {Id}", id);
                return (false, "Đã xảy ra sự cố khi xóa giảng viên. Vui lòng thử lại sau.");
            }
        }

        // 10. Đổi trạng thái nhanh Đang giảng dạy / Tạm nghỉ
        public async Task<(bool Success, string? ErrorMessage, bool NewStatus)> ToggleStatusAsync(int id)
        {
            try
            {
                var giangVien = await _context.GiangViens.FindAsync(id);
                if (giangVien == null)
                {
                    return (false, "Không tìm thấy thông tin giảng viên cần đổi trạng thái.", false);
                }

                giangVien.TrangThai = !giangVien.TrangThai;
                await _context.SaveChangesAsync();

                string message;
                if (!giangVien.TrangThai)
                {
                    var (hasActive, activeCourses) = await GetActiveTeachingCoursesAsync(id);
                    if (hasActive)
                    {
                        var summary = string.Join(", ", activeCourses.Take(2)) + (activeCourses.Count > 2 ? $" và {activeCourses.Count - 2} khóa khác" : "");
                        message = $"Đã chuyển giảng viên '{giangVien.HoTen}' sang Tạm nghỉ. Lưu ý: Giảng viên đang phụ trách {activeCourses.Count} khóa học ({summary}), vui lòng phân công người dạy thay nếu cần!";
                    }
                    else
                    {
                        message = $"Đã chuyển giảng viên '{giangVien.HoTen}' sang trạng thái Tạm nghỉ.";
                    }
                }
                else
                {
                    message = $"Đã kích hoạt giảng viên '{giangVien.HoTen}' sang trạng thái Đang giảng dạy (Sẵn sàng nhận lớp).";
                }

                return (true, message, giangVien.TrangThai);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đổi trạng thái giảng viên ID {Id}", id);
                return (false, "Không thể đổi trạng thái giảng viên do sự cố CSDL.", false);
            }
        }

        // 11. Kiểm tra giảng viên có đang phụ trách khóa học hoạt động / có lịch dạy không
        public async Task<(bool HasActiveCourses, List<string> CourseNames)> GetActiveTeachingCoursesAsync(int maGiangVien)
        {
            var activeCourses = await _context.KhoaHocs
                .Where(k => k.MaGiangVien == maGiangVien && k.TrangThai != "DaKetThuc" && k.TrangThai != "BiHuy")
                .OrderBy(k => k.TenKhoaHoc)
                .Select(k => k.TenKhoaHoc)
                .ToListAsync();

            return (activeCourses.Any(), activeCourses);
        }
    }
}
