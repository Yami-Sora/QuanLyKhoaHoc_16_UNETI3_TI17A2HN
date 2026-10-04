// Họ và tên: Bạch Đức Sự
// Mã sinh viên: 23103100063
// Nội dung thực hiện: Module 4 - Tiếp nhận đăng ký, Xác nhận/Hủy đăng ký và Quản lý trạng thái.

using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Helpers
{
    public class TiepNhanDangKyService
    {
        private readonly ApplicationDbContext _context;

        public TiepNhanDangKyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TiepNhanFilterViewModel> GetDanhSachDangKyAsync(TiepNhanFilterViewModel filter)
        {
            if (filter.PageIndex < 1) filter.PageIndex = 1;
            if (filter.PageSize < 1) filter.PageSize = 10;

            // 1. LINQ Thống kê Stat Cards tổng quan
            filter.TongSoDon = await _context.DangKyKhoaHocs.CountAsync();
            filter.SoDonChoXuLy = await _context.DangKyKhoaHocs.CountAsync(d => d.TrangThai == "ChoXuLy");
            filter.SoDonDangXuLy = await _context.DangKyKhoaHocs.CountAsync(d => d.TrangThai == "DangXuLy");
            filter.SoDonHoanThanh = await _context.DangKyKhoaHocs.CountAsync(d => d.TrangThai == "HoanThanh");
            filter.SoDonBiHuy = await _context.DangKyKhoaHocs.CountAsync(d => d.TrangThai == "BiHuy");
            filter.TongDoanhThuDaThu = await _context.DangKyKhoaHocs.SumAsync(d => (decimal?)d.SoTienDaDong) ?? 0;
            filter.TongCongNoConLai = await _context.DangKyKhoaHocs.Where(d => d.TrangThai != "BiHuy").SumAsync(d => (decimal?)d.SoTienConLai) ?? 0;

            // 2. LINQ Eager Loading quan hệ Học viên, Khóa học và Môn học
            var query = _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.MonHoc)
                .AsQueryable();

            // 3. LINQ Tìm kiếm từ khóa tổng hợp
            if (!string.IsNullOrWhiteSpace(filter.SearchString))
            {
                var keyword = filter.SearchString.Trim();
                query = query.Where(d =>
                    (d.HocVien != null && (d.HocVien.HoTen.Contains(keyword) || d.HocVien.SoDienThoai.Contains(keyword) || d.HocVien.Email.Contains(keyword))) ||
                    (d.KhoaHoc != null && d.KhoaHoc.TenKhoaHoc.Contains(keyword)) ||
                    d.MaDangKy.ToString() == keyword);
            }

            // 4. LINQ Lọc theo Tên học viên
            if (!string.IsNullOrWhiteSpace(filter.SearchHocVien))
            {
                var hvKeyword = filter.SearchHocVien.Trim();
                query = query.Where(d => d.HocVien != null && d.HocVien.HoTen.Contains(hvKeyword));
            }

            // 5. LINQ Lọc theo Tên khóa học
            if (!string.IsNullOrWhiteSpace(filter.SearchKhoaHoc))
            {
                var khKeyword = filter.SearchKhoaHoc.Trim();
                query = query.Where(d => d.KhoaHoc != null && d.KhoaHoc.TenKhoaHoc.Contains(khKeyword));
            }

            // 6. LINQ Lọc theo Trạng thái (ChoXuLy, DangXuLy, HoanThanh, BiHuy)
            if (!string.IsNullOrWhiteSpace(filter.TrangThai) && filter.TrangThai != "TatCa")
            {
                query = query.Where(d => d.TrangThai == filter.TrangThai);
            }

            // 7. LINQ Lọc theo Khoảng ngày đăng ký
            if (filter.TuNgay.HasValue)
            {
                var fromDate = filter.TuNgay.Value.Date;
                query = query.Where(d => d.NgayDangKy >= fromDate);
            }

            if (filter.DenNgay.HasValue)
            {
                var toDate = filter.DenNgay.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(d => d.NgayDangKy <= toDate);
            }

            // 8. LINQ Sắp xếp dữ liệu (SortBy)
            query = (filter.SortBy ?? "ngay_desc") switch
            {
                "ngay_asc" => query.OrderBy(d => d.NgayDangKy),
                "ten_asc" => query.OrderBy(d => d.HocVien != null ? d.HocVien.HoTen : string.Empty),
                "ten_desc" => query.OrderByDescending(d => d.HocVien != null ? d.HocVien.HoTen : string.Empty),
                "khoahoc_asc" => query.OrderBy(d => d.KhoaHoc != null ? d.KhoaHoc.TenKhoaHoc : string.Empty),
                "tiendadong_desc" => query.OrderByDescending(d => d.SoTienDaDong),
                "tienconlai_desc" => query.OrderByDescending(d => d.SoTienConLai),
                _ => query.OrderByDescending(d => d.NgayDangKy)
            };

            // 9. LINQ Tính tổng số kết quả lọc
            filter.TotalItems = await query.CountAsync();

            // 10. Phân trang chuẩn LINQ (Skip / Take)
            filter.DanhSachDangKy = await query
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return filter;
        }

        public async Task<ChiTietDangKyViewModel?> GetChiTietDangKyAsync(int id)
        {
            var dangKy = await _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                    .ThenInclude(h => h!.TaiKhoan)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.MonHoc)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.GiangVien)
                .Include(d => d.KetQuaHocTap)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dangKy == null) return null;

            var siSoHienTai = await _context.DangKyKhoaHocs
                .CountAsync(d => d.MaKhoaHoc == dangKy.MaKhoaHoc && (d.TrangThai == "HoanThanh" || d.TrangThai == "DangXuLy"));

            var cacDangKyKhac = await _context.DangKyKhoaHocs
                .Include(d => d.KhoaHoc)
                .Where(d => d.MaHocVien == dangKy.MaHocVien && d.MaDangKy != dangKy.MaDangKy)
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();

            return new ChiTietDangKyViewModel
            {
                DangKy = dangKy,
                SiSoHienTai = siSoHienTai,
                SiSoToiDa = dangKy.KhoaHoc?.SoLuongToiDa ?? 0,
                CacDangKyKhac = cacDangKyKhac
            };
        }

        public async Task<CapNhatTrangThaiViewModel?> GetCapNhatTrangThaiViewModelAsync(int id)
        {
            var dangKy = await _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .Include(d => d.KhoaHoc)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dangKy == null) return null;

            var siSoHienTai = await _context.DangKyKhoaHocs
                .CountAsync(d => d.MaKhoaHoc == dangKy.MaKhoaHoc && (d.TrangThai == "HoanThanh" || d.TrangThai == "DangXuLy"));

            var trangThaiHopLe = LayDanhSachTrangThaiHopLe(dangKy.TrangThai);

            return new CapNhatTrangThaiViewModel
            {
                MaDangKy = dangKy.MaDangKy,
                HoTenHocVien = dangKy.HocVien?.HoTen ?? "N/A",
                SoDienThoai = dangKy.HocVien?.SoDienThoai ?? "N/A",
                Email = dangKy.HocVien?.Email ?? "N/A",
                TenKhoaHoc = dangKy.KhoaHoc?.TenKhoaHoc ?? "N/A",
                TrangThaiKhoaHoc = dangKy.KhoaHoc?.TrangThai ?? "N/A",
                HocPhi = dangKy.KhoaHoc?.HocPhi ?? 0,
                SiSoHienTai = siSoHienTai,
                SoLuongToiDa = dangKy.KhoaHoc?.SoLuongToiDa ?? 0,
                NgayDangKy = dangKy.NgayDangKy,
                TrangThaiHienTai = dangKy.TrangThai,
                TrangThaiMoi = trangThaiHopLe.FirstOrDefault() ?? dangKy.TrangThai,
                SoTienDaDong = dangKy.SoTienDaDong,
                SoTienConLai = dangKy.SoTienConLai,
                GhiChu = dangKy.GhiChu,
                TrangThaiHopLe = trangThaiHopLe
            };
        }

        public async Task<(bool HopLe, string ThongBaoLoi)> KiemTraDieuKienDuyetDangKyAsync(int maDangKy, string trangThaiMoi)
        {
            var dangKy = await _context.DangKyKhoaHocs
                .Include(d => d.KhoaHoc)
                .Include(d => d.HocVien)
                .FirstOrDefaultAsync(d => d.MaDangKy == maDangKy);

            if (dangKy == null)
            {
                return (false, "Không tìm thấy hồ sơ đăng ký trong hệ thống.");
            }

            if (trangThaiMoi != "DangXuLy" && trangThaiMoi != "HoanThanh")
            {
                return (true, string.Empty);
            }

            // 1. Kiểm tra LINQ: Khóa học phải ở trạng thái mở (DangMo)
            if (dangKy.KhoaHoc == null || dangKy.KhoaHoc.TrangThai != "DangMo")
            {
                var tenTrangThaiKhoaHoc = dangKy.KhoaHoc != null ? dangKy.KhoaHoc.TrangThai : "Không xác định";
                return (false, $"Không thể duyệt hồ sơ: Khóa học '{dangKy.KhoaHoc?.TenKhoaHoc}' hiện không ở trạng thái mở tiếp nhận (Trạng thái: {tenTrangThaiKhoaHoc}). Chỉ khóa học 'Đang mở' mới được tiếp nhận học viên.");
            }

            // 2. Kiểm tra LINQ: Sĩ số tối đa
            var enrolledCount = await _context.DangKyKhoaHocs
                .CountAsync(d => d.MaKhoaHoc == dangKy.MaKhoaHoc &&
                                 d.MaDangKy != dangKy.MaDangKy &&
                                 (d.TrangThai == "HoanThanh" || d.TrangThai == "DangXuLy"));

            if (enrolledCount >= dangKy.KhoaHoc.SoLuongToiDa)
            {
                return (false, $"Không thể duyệt hồ sơ: Khóa học '{dangKy.KhoaHoc.TenKhoaHoc}' đã đủ chỉ tiêu sĩ số tối đa ({enrolledCount}/{dangKy.KhoaHoc.SoLuongToiDa} học viên). Lớp học đã hết chỗ tiếp nhận!");
            }

            // 3. Kiểm tra LINQ: Chống duyệt trùng bản ghi đã hoàn thành
            var daDangKyThanhCong = await _context.DangKyKhoaHocs
                .AnyAsync(d => d.MaHocVien == dangKy.MaHocVien &&
                               d.MaKhoaHoc == dangKy.MaKhoaHoc &&
                               d.MaDangKy != dangKy.MaDangKy &&
                               d.TrangThai == "HoanThanh");

            if (daDangKyThanhCong)
            {
                return (false, $"Vi phạm nghiệp vụ chống duyệt trùng: Học viên '{dangKy.HocVien?.HoTen}' đã có bản ghi đăng ký khóa học '{dangKy.KhoaHoc.TenKhoaHoc}' ở trạng thái 'Hoàn thành' trước đó. Hệ thống không cho phép duyệt trùng!");
            }

            return (true, string.Empty);
        }

        public async Task<(bool ThanhCong, string ThongBao)> CapNhatTrangThaiAsync(CapNhatTrangThaiViewModel model)
        {
            var dangKy = await _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .Include(d => d.KhoaHoc)
                .FirstOrDefaultAsync(d => d.MaDangKy == model.MaDangKy);

            if (dangKy == null)
            {
                return (false, "Không tìm thấy hồ sơ đăng ký trong hệ thống.");
            }

            // 1. Kiểm tra luồng chuyển đổi trạng thái
            if (!KiemTraChuyenTrangThaiHopLe(dangKy.TrangThai, model.TrangThaiMoi))
            {
                return (false, $"Luồng chuyển đổi không hợp lệ! Không thể chuyển từ trạng thái '{LayTenTrangThai(dangKy.TrangThai)}' sang '{LayTenTrangThai(model.TrangThaiMoi)}'. Quy trình chuẩn: [Chờ xử lý] -> [Đang xử lý] -> [Hoàn thành] (hoặc Hủy).");
            }

            // 2. Kiểm tra Business Rules LINQ khi duyệt sang DangXuLy hoặc HoanThanh
            if (model.TrangThaiMoi == "DangXuLy" || model.TrangThaiMoi == "HoanThanh")
            {
                var checkRules = await KiemTraDieuKienDuyetDangKyAsync(model.MaDangKy, model.TrangThaiMoi);
                if (!checkRules.HopLe)
                {
                    return (false, checkRules.ThongBaoLoi);
                }
            }

            // 3. Kiểm tra tiền và ghi chú khi hủy
            if (model.SoTienDaDong < 0)
            {
                return (false, "Số tiền đã nộp không được âm.");
            }

            if (model.TrangThaiMoi == "BiHuy" && string.IsNullOrWhiteSpace(model.GhiChu))
            {
                return (false, "Vui lòng nhập lý do hủy/từ chối đơn đăng ký vào ô ghi chú.");
            }

            try
            {
                var trangThaiCu = dangKy.TrangThai;
                var hocPhiKhoaHoc = dangKy.KhoaHoc?.HocPhi ?? 0;
                var soTienConLai = Math.Max(0, hocPhiKhoaHoc - model.SoTienDaDong);

                dangKy.TrangThai = model.TrangThaiMoi;
                dangKy.SoTienDaDong = model.SoTienDaDong;
                dangKy.SoTienConLai = soTienConLai;
                dangKy.GhiChu = model.GhiChu?.Trim();

                if (model.TrangThaiMoi == "HoanThanh" || model.TrangThaiMoi == "DangXuLy")
                {
                    dangKy.NgayXacNhan = DateTime.Now;
                }

                _context.Update(dangKy);
                await _context.SaveChangesAsync();

                return (true, $"Đã cập nhật trạng thái đơn #{dangKy.MaDangKy} của học viên '{dangKy.HocVien?.HoTen}' từ '{LayTenTrangThai(trangThaiCu)}' sang '{LayTenTrangThai(dangKy.TrangThai)}' thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi hệ thống khi lưu CSDL: {ex.Message}");
            }
        }

        public async Task<(bool ThanhCong, string ThongBao)> TiepNhanNhanhAsync(int id)
        {
            var dangKy = await _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .Include(d => d.KhoaHoc)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dangKy == null)
            {
                return (false, "Không tìm thấy đơn đăng ký.");
            }

            if (dangKy.TrangThai != "ChoXuLy")
            {
                return (false, $"Chỉ đơn ở trạng thái 'Chờ xử lý' mới có thể tiếp nhận nhanh. (Trạng thái hiện tại: {LayTenTrangThai(dangKy.TrangThai)})");
            }

            var check = await KiemTraDieuKienDuyetDangKyAsync(id, "DangXuLy");
            if (!check.HopLe)
            {
                return (false, check.ThongBaoLoi);
            }

            dangKy.TrangThai = "DangXuLy";
            dangKy.NgayXacNhan = DateTime.Now;
            dangKy.GhiChu = string.IsNullOrEmpty(dangKy.GhiChu)
                ? $"[Tiếp nhận nhanh lúc {DateTime.Now:dd/MM/yyyy HH:mm}]"
                : $"{dangKy.GhiChu} | [Tiếp nhận lúc {DateTime.Now:dd/MM/yyyy HH:mm}]";

            await _context.SaveChangesAsync();
            return (true, $"Đã tiếp nhận hồ sơ #{dangKy.MaDangKy} của '{dangKy.HocVien?.HoTen}' sang trạng thái 'Đang xử lý' thành công!");
        }

        public async Task<(bool ThanhCong, string ThongBao)> XacNhanHoanThanhAsync(int id, decimal? soTienDong, string? ghiChuXacNhan)
        {
            var dangKy = await _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .Include(d => d.KhoaHoc)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dangKy == null)
            {
                return (false, "Không tìm thấy đơn đăng ký.");
            }

            if (dangKy.TrangThai != "DangXuLy")
            {
                return (false, "Chỉ đơn ở trạng thái 'Đang xử lý' mới được xác nhận Hoàn thành. Vui lòng tiếp nhận đơn trước!");
            }

            var check = await KiemTraDieuKienDuyetDangKyAsync(id, "HoanThanh");
            if (!check.HopLe)
            {
                return (false, check.ThongBaoLoi);
            }

            if (soTienDong.HasValue && soTienDong.Value >= 0)
            {
                dangKy.SoTienDaDong = soTienDong.Value;
            }
            else if (dangKy.SoTienDaDong == 0 && dangKy.KhoaHoc != null)
            {
                dangKy.SoTienDaDong = dangKy.KhoaHoc.HocPhi;
            }

            var hocPhi = dangKy.KhoaHoc?.HocPhi ?? 0;
            dangKy.SoTienConLai = Math.Max(0, hocPhi - dangKy.SoTienDaDong);
            dangKy.TrangThai = "HoanThanh";
            dangKy.NgayXacNhan = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(ghiChuXacNhan))
            {
                dangKy.GhiChu = string.IsNullOrEmpty(dangKy.GhiChu)
                    ? ghiChuXacNhan.Trim()
                    : $"{dangKy.GhiChu} | {ghiChuXacNhan.Trim()}";
            }

            await _context.SaveChangesAsync();
            return (true, $"Đã duyệt Hoàn thành đơn #{dangKy.MaDangKy} của '{dangKy.HocVien?.HoTen}' thành công!");
        }

        public async Task<(bool ThanhCong, string ThongBao)> HuyDangKyAsync(int id, string lyDoHuy)
        {
            if (string.IsNullOrWhiteSpace(lyDoHuy))
            {
                return (false, "Vui lòng cung cấp lý do hủy/từ chối đơn đăng ký.");
            }

            var dangKy = await _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dangKy == null)
            {
                return (false, "Không tìm thấy đơn đăng ký.");
            }

            dangKy.TrangThai = "BiHuy";
            dangKy.GhiChu = string.IsNullOrEmpty(dangKy.GhiChu)
                ? $"[Lý do hủy: {lyDoHuy.Trim()} - Ngày: {DateTime.Now:dd/MM/yyyy HH:mm}]"
                : $"{dangKy.GhiChu} | [Lý do hủy: {lyDoHuy.Trim()} - Ngày: {DateTime.Now:dd/MM/yyyy HH:mm}]";

            await _context.SaveChangesAsync();
            return (true, $"Đã hủy đơn đăng ký #{dangKy.MaDangKy} của '{dangKy.HocVien?.HoTen}'. Lý do: {lyDoHuy.Trim()}");
        }

        public async Task<(bool ThanhCong, string ThongBao, int MaDangKyMoi)> TaoDangKyMoiAsync(TaoDangKyMoiViewModel model)
        {
            var khoaHoc = await _context.KhoaHocs.FindAsync(model.MaKhoaHoc);
            if (khoaHoc == null)
            {
                return (false, "Khóa học đã chọn không tồn tại.", 0);
            }

            if (khoaHoc.TrangThai != "DangMo")
            {
                return (false, $"Khóa học '{khoaHoc.TenKhoaHoc}' hiện không ở trạng thái mở đăng ký (Trạng thái: {khoaHoc.TrangThai}).", 0);
            }

            var countEnrolled = await _context.DangKyKhoaHocs
                .CountAsync(d => d.MaKhoaHoc == model.MaKhoaHoc && (d.TrangThai == "HoanThanh" || d.TrangThai == "DangXuLy"));

            if (countEnrolled >= khoaHoc.SoLuongToiDa)
            {
                return (false, $"Khóa học '{khoaHoc.TenKhoaHoc}' đã hết chỗ tiếp nhận ({countEnrolled}/{khoaHoc.SoLuongToiDa} học viên).", 0);
            }

            var daDangKyThanhCong = await _context.DangKyKhoaHocs
                .AnyAsync(d => d.MaHocVien == model.MaHocVien && d.MaKhoaHoc == model.MaKhoaHoc && d.TrangThai == "HoanThanh");

            if (daDangKyThanhCong)
            {
                return (false, "Học viên này đã hoàn thành khóa học này trước đó. Không được đăng ký trùng.", 0);
            }

            var soTienConLai = Math.Max(0, khoaHoc.HocPhi - model.SoTienDaDong);

            var dangKy = new DangKyKhoaHoc
            {
                MaHocVien = model.MaHocVien,
                MaKhoaHoc = model.MaKhoaHoc,
                NgayDangKy = DateTime.Now,
                TrangThai = model.TrangThai,
                SoTienDaDong = model.SoTienDaDong,
                SoTienConLai = soTienConLai,
                GhiChu = model.GhiChu?.Trim(),
                NgayXacNhan = model.TrangThai == "DangXuLy" || model.TrangThai == "HoanThanh" ? DateTime.Now : null
            };

            _context.DangKyKhoaHocs.Add(dangKy);
            await _context.SaveChangesAsync();

            return (true, $"Tiếp nhận đơn đăng ký mới #{dangKy.MaDangKy} thành công!", dangKy.MaDangKy);
        }

        public async Task<DangKyKhoaHoc?> GetDangKyForPrintAsync(int id)
        {
            return await _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.MonHoc)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.GiangVien)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);
        }

        public async Task<(IEnumerable<object> HocViens, IEnumerable<object> KhoaHocs)> LayDuLieuDropDownListAsync()
        {
            var hocViens = await _context.HocViens
                .Where(h => h.TrangThai)
                .OrderBy(h => h.HoTen)
                .Select(h => new
                {
                    h.MaHocVien,
                    ThongTin = $"{h.HoTen} - SĐT: {h.SoDienThoai} ({h.Email})"
                })
                .ToListAsync<object>();

            var khoaHocs = await _context.KhoaHocs
                .Where(k => k.TrangThai == "DangMo")
                .OrderBy(k => k.TenKhoaHoc)
                .Select(k => new
                {
                    k.MaKhoaHoc,
                    ThongTin = $"{k.TenKhoaHoc} (Học phí: {k.HocPhi:N0} đ - Tối đa: {k.SoLuongToiDa} HV)"
                })
                .ToListAsync<object>();

            return (hocViens, khoaHocs);
        }

        public static List<string> LayDanhSachTrangThaiHopLe(string trangThaiHienTai)
        {
            return trangThaiHienTai switch
            {
                "ChoXuLy" => new List<string> { "DangXuLy", "BiHuy" },
                "DangXuLy" => new List<string> { "HoanThanh", "BiHuy", "ChoXuLy" },
                "HoanThanh" => new List<string> { "BiHuy" },
                "BiHuy" => new List<string> { "ChoXuLy" },
                _ => new List<string> { "ChoXuLy", "DangXuLy", "HoanThanh", "BiHuy" }
            };
        }

        public static bool KiemTraChuyenTrangThaiHopLe(string hienTai, string moi)
        {
            if (hienTai == moi) return true;
            var hopLe = LayDanhSachTrangThaiHopLe(hienTai);
            return hopLe.Contains(moi);
        }

        public static string LayTenTrangThai(string? trangThai)
        {
            return trangThai switch
            {
                "ChoXuLy" => "Chờ xử lý",
                "DangXuLy" => "Đang xử lý",
                "HoanThanh" => "Hoàn thành",
                "BiHuy" => "Đã hủy/Từ chối",
                _ => trangThai ?? "Không xác định"
            };
        }
    }
}
