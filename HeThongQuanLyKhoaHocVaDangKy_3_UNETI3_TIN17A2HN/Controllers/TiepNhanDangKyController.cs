// Họ và tên: Bạch Đức Sự
// Mã sinh viên: 23103100063
// Nội dung thực hiện: Module 4 - Tiếp nhận đăng ký, Xác nhận/Hủy đăng ký và Quản lý trạng thái.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    // Họ và tên: Bạch Đức Sự
    // Mã sinh viên: 23103100063
    // Nội dung thực hiện: Module 4 - Phân quyền truy cập chỉ dành cho Quản trị viên và Nhân viên đào tạo
    [AuthorizeRole("Admin", "NhanVien")]
    public class TiepNhanDangKyController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Họ và tên: Bạch Đức Sự
        // Mã sinh viên: 23103100063
        // Nội dung thực hiện: Module 4 - Khởi tạo DbContext cho Module tiếp nhận và quản lý trạng thái đăng ký
        public TiepNhanDangKyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================================
        // 1. DANH SÁCH TIẾP NHẬN ĐĂNG KÝ, TÌM KIẾM, LỌC, SẮP XẾP & PHÂN TRANG (LINQ)
        // =========================================================================

        // Họ và tên: Bạch Đức Sự
        // Mã sinh viên: 23103100063
        // Nội dung thực hiện: Module 4 - Action Index hiển thị danh sách đơn đăng ký, hỗ trợ tìm kiếm đa tiêu chí, lọc trạng thái, khoảng ngày, sắp xếp và phân trang chuẩn LINQ
        [HttpGet]
        public async Task<IActionResult> Index(
            string? searchString,
            string? searchHocVien,
            string? searchKhoaHoc,
            string? trangThai,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? sortBy,
            int page = 1,
            int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            // 1. Thống kê Stat Cards tổng quan bằng LINQ
            var tongSoDon = await _context.DangKyKhoaHocs.CountAsync();
            var soDonChoXuLy = await _context.DangKyKhoaHocs.CountAsync(d => d.TrangThai == "ChoXuLy");
            var soDonDangXuLy = await _context.DangKyKhoaHocs.CountAsync(d => d.TrangThai == "DangXuLy");
            var soDonHoanThanh = await _context.DangKyKhoaHocs.CountAsync(d => d.TrangThai == "HoanThanh");
            var soDonBiHuy = await _context.DangKyKhoaHocs.CountAsync(d => d.TrangThai == "BiHuy");
            var tongDoanhThuDaThu = await _context.DangKyKhoaHocs.SumAsync(d => (decimal?)d.SoTienDaDong) ?? 0;
            var tongCongNoConLai = await _context.DangKyKhoaHocs.Where(d => d.TrangThai != "BiHuy").SumAsync(d => (decimal?)d.SoTienConLai) ?? 0;

            // 2. Khởi tạo truy vấn LINQ Eager Loading quan hệ Học viên, Khóa học và Môn học
            var query = _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.MonHoc)
                .AsQueryable();

            // 3. LINQ Tìm kiếm từ khóa tổng hợp (Tên học viên, SĐT, Email, Mã đăng ký, Tên khóa học)
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var keyword = searchString.Trim();
                query = query.Where(d =>
                    (d.HocVien != null && (d.HocVien.HoTen.Contains(keyword) || d.HocVien.SoDienThoai.Contains(keyword) || d.HocVien.Email.Contains(keyword))) ||
                    (d.KhoaHoc != null && d.KhoaHoc.TenKhoaHoc.Contains(keyword)) ||
                    d.MaDangKy.ToString() == keyword);
            }

            // 4. LINQ Tìm kiếm riêng theo tên Học viên
            if (!string.IsNullOrWhiteSpace(searchHocVien))
            {
                var hvKeyword = searchHocVien.Trim();
                query = query.Where(d => d.HocVien != null && d.HocVien.HoTen.Contains(hvKeyword));
            }

            // 5. LINQ Tìm kiếm riêng theo tên Khóa học
            if (!string.IsNullOrWhiteSpace(searchKhoaHoc))
            {
                var khKeyword = searchKhoaHoc.Trim();
                query = query.Where(d => d.KhoaHoc != null && d.KhoaHoc.TenKhoaHoc.Contains(khKeyword));
            }

            // 6. LINQ Lọc theo Trạng thái (ChoXuLy, DangXuLy, HoanThanh, BiHuy)
            if (!string.IsNullOrWhiteSpace(trangThai) && trangThai != "TatCa")
            {
                query = query.Where(d => d.TrangThai == trangThai);
            }

            // 7. LINQ Lọc theo Khoảng ngày đăng ký
            if (tuNgay.HasValue)
            {
                var fromDate = tuNgay.Value.Date;
                query = query.Where(d => d.NgayDangKy >= fromDate);
            }

            if (denNgay.HasValue)
            {
                var toDate = denNgay.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(d => d.NgayDangKy <= toDate);
            }

            // 8. LINQ Sắp xếp dữ liệu (SortBy)
            query = sortBy switch
            {
                "ngay_asc" => query.OrderBy(d => d.NgayDangKy),
                "ten_asc" => query.OrderBy(d => d.HocVien != null ? d.HocVien.HoTen : string.Empty),
                "ten_desc" => query.OrderByDescending(d => d.HocVien != null ? d.HocVien.HoTen : string.Empty),
                "khoahoc_asc" => query.OrderBy(d => d.KhoaHoc != null ? d.KhoaHoc.TenKhoaHoc : string.Empty),
                "tiendadong_desc" => query.OrderByDescending(d => d.SoTienDaDong),
                "tienconlai_desc" => query.OrderByDescending(d => d.SoTienConLai),
                _ => query.OrderByDescending(d => d.NgayDangKy) // Mặc định: ngày đăng ký mới nhất
            };

            // 9. Tính tổng số kết quả lọc để phân trang
            var totalItems = await query.CountAsync();

            // 10. Phân trang chuẩn LINQ (Skip / Take)
            var pagedData = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 11. Đóng gói ViewModel trả về View
            var viewModel = new TiepNhanDangKyListViewModel
            {
                DanhSachDangKy = pagedData,
                TongSoDon = tongSoDon,
                SoDonChoXuLy = soDonChoXuLy,
                SoDonDangXuLy = soDonDangXuLy,
                SoDonHoanThanh = soDonHoanThanh,
                SoDonBiHuy = soDonBiHuy,
                TongDoanhThuDaThu = tongDoanhThuDaThu,
                TongCongNoConLai = tongCongNoConLai,
                SearchString = searchString,
                SearchHocVien = searchHocVien,
                SearchKhoaHoc = searchKhoaHoc,
                TrangThai = trangThai,
                TuNgay = tuNgay,
                DenNgay = denNgay,
                SortBy = sortBy ?? "ngay_desc",
                PageIndex = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return View(viewModel);
        }

        // =========================================================================
        // 2. CHI TIẾT ĐƠN ĐĂNG KÝ (DETAILS)
        // =========================================================================

        // Họ và tên: Bạch Đức Sự
        // Mã sinh viên: 23103100063
        // Nội dung thực hiện: Module 4 - Action xem Chi tiết đơn đăng ký, thông tin học viên, khóa học và đối soát học phí
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var dangKy = await _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                    .ThenInclude(h => h!.TaiKhoan)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.MonHoc)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.GiangVien)
                .Include(d => d.KetQuaHocTap)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dangKy == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ đăng ký khóa học này.";
                return RedirectToAction(nameof(Index));
            }

            // Sĩ số học viên đã xác nhận/đang xử lý của khóa học này bằng LINQ
            ViewBag.SiSoHienTai = await _context.DangKyKhoaHocs
                .CountAsync(d => d.MaKhoaHoc == dangKy.MaKhoaHoc && (d.TrangThai == "HoanThanh" || d.TrangThai == "DangXuLy"));

            // Danh sách các đăng ký khác của học viên này
            ViewBag.CacDangKyKhac = await _context.DangKyKhoaHocs
                .Include(d => d.KhoaHoc)
                .Where(d => d.MaHocVien == dangKy.MaHocVien && d.MaDangKy != dangKy.MaDangKy)
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();

            return View(dangKy);
        }

        // =========================================================================
        // 3. QUẢN LÝ LUỒNG TRẠNG THÁI & KIỂM TRA NGHIỆP VỤ (BUSINESS RULES)
        // =========================================================================

        // Họ và tên: Bạch Đức Sự
        // Mã sinh viên: 23103100063
        // Nội dung thực hiện: Module 4 - Action GET hiển thị Form Cập nhật Trạng thái đơn và đối soát tiền
        [HttpGet]
        public async Task<IActionResult> CapNhatTrangThai(int? id)
        {
            if (id == null) return NotFound();

            var dangKy = await _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .Include(d => d.KhoaHoc)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dangKy == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin đơn đăng ký cần cập nhật.";
                return RedirectToAction(nameof(Index));
            }

            // Tính sĩ số hiện tại của khóa học bằng LINQ
            var siSoHienTai = await _context.DangKyKhoaHocs
                .CountAsync(d => d.MaKhoaHoc == dangKy.MaKhoaHoc && (d.TrangThai == "HoanThanh" || d.TrangThai == "DangXuLy"));

            // Xác định luồng chuyển đổi trạng thái hợp lệ
            var trangThaiHopLe = LayDanhSachTrangThaiHopLe(dangKy.TrangThai);

            var viewModel = new CapNhatTrangThaiViewModel
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

            return View(viewModel);
        }

        // Họ và tên: Bạch Đức Sự
        // Mã sinh viên: 23103100063
        // Nội dung thực hiện: Module 4 - Action POST thực hiện kiểm tra nghiệp vụ luồng trạng thái, kiểm tra khóa học mở, kiểm tra sĩ số, chống duyệt trùng bằng LINQ và cập nhật CSDL
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(CapNhatTrangThaiViewModel model)
        {
            var dangKy = await _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .Include(d => d.KhoaHoc)
                .FirstOrDefaultAsync(d => d.MaDangKy == model.MaDangKy);

            if (dangKy == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ đăng ký trong hệ thống.";
                return RedirectToAction(nameof(Index));
            }

            // Gán lại dữ liệu hiển thị cần thiết cho view nếu có lỗi
            model.HoTenHocVien = dangKy.HocVien?.HoTen ?? "N/A";
            model.SoDienThoai = dangKy.HocVien?.SoDienThoai ?? "N/A";
            model.Email = dangKy.HocVien?.Email ?? "N/A";
            model.TenKhoaHoc = dangKy.KhoaHoc?.TenKhoaHoc ?? "N/A";
            model.TrangThaiKhoaHoc = dangKy.KhoaHoc?.TrangThai ?? "N/A";
            model.HocPhi = dangKy.KhoaHoc?.HocPhi ?? 0;
            model.SoLuongToiDa = dangKy.KhoaHoc?.SoLuongToiDa ?? 0;
            model.NgayDangKy = dangKy.NgayDangKy;
            model.TrangThaiHienTai = dangKy.TrangThai;
            model.TrangThaiHopLe = LayDanhSachTrangThaiHopLe(dangKy.TrangThai);
            model.SiSoHienTai = await _context.DangKyKhoaHocs
                .CountAsync(d => d.MaKhoaHoc == dangKy.MaKhoaHoc && (d.TrangThai == "HoanThanh" || d.TrangThai == "DangXuLy"));

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 1. KIỂM TRA LUỒNG TRẠNG THÁI HỢP LỆ (State Machine Validation)
            bool isTransitionValid = KiemTraChuyenTrangThaiHopLe(dangKy.TrangThai, model.TrangThaiMoi);
            if (!isTransitionValid)
            {
                ModelState.AddModelError(nameof(model.TrangThaiMoi),
                    $"Luồng chuyển đổi không hợp lệ! Không thể chuyển từ trạng thái '{LayTenTrangThai(dangKy.TrangThai)}' sang '{LayTenTrangThai(model.TrangThaiMoi)}'. Quy trình chuẩn: [Chờ xử lý] -> [Đang xử lý] -> [Hoàn thành] (hoặc Hủy).");
                return View(model);
            }

            // 2. NẾU DUYỆT TRẠNG THÁI SANG 'DangXuLy' HOẶC 'HoanThanh' => KIỂM TRA 3 ĐIỀU KIỆN NGHIỆP VỤ BẰNG LINQ
            if (model.TrangThaiMoi == "DangXuLy" || model.TrangThaiMoi == "HoanThanh")
            {
                // Kiểm tra 2.1: Khóa học phải ở trạng thái Mở (DangMo)
                if (dangKy.KhoaHoc == null || dangKy.KhoaHoc.TrangThai != "DangMo")
                {
                    var tenTrangThaiKhoaHoc = dangKy.KhoaHoc != null ? dangKy.KhoaHoc.TrangThai : "Không xác định";
                    ModelState.AddModelError(string.Empty,
                        $"Không thể xác nhận/duyệt hồ sơ: Khóa học '{dangKy.KhoaHoc?.TenKhoaHoc}' hiện không ở trạng thái mở tiếp nhận (Trạng thái hiện tại: {tenTrangThaiKhoaHoc}). Chỉ khóa học 'Đang mở' mới được tiếp nhận học viên.");
                    return View(model);
                }

                // Kiểm tra 2.2: Sĩ số tối đa - Số lượng học viên đã "Hoàn thành" hoặc "Đang xử lý" không được vượt quá SoLuongToiDa
                var enrolledCount = await _context.DangKyKhoaHocs
                    .CountAsync(d => d.MaKhoaHoc == dangKy.MaKhoaHoc &&
                                     d.MaDangKy != dangKy.MaDangKy &&
                                     (d.TrangThai == "HoanThanh" || d.TrangThai == "DangXuLy"));

                if (enrolledCount >= dangKy.KhoaHoc.SoLuongToiDa)
                {
                    ModelState.AddModelError(string.Empty,
                        $"Không thể xác nhận/duyệt hồ sơ: Khóa học '{dangKy.KhoaHoc.TenKhoaHoc}' đã đủ chỉ tiêu tối đa ({enrolledCount}/{dangKy.KhoaHoc.SoLuongToiDa} học viên). Lớp học đã hết chỗ tiếp nhận!");
                    return View(model);
                }

                // Kiểm tra 2.3: Không cho phép duyệt trùng nếu học viên đã có bản ghi đăng ký khóa học đó ở trạng thái "Hoàn thành"
                var daDangKyThanhCong = await _context.DangKyKhoaHocs
                    .AnyAsync(d => d.MaHocVien == dangKy.MaHocVien &&
                                   d.MaKhoaHoc == dangKy.MaKhoaHoc &&
                                   d.MaDangKy != dangKy.MaDangKy &&
                                   d.TrangThai == "HoanThanh");

                if (daDangKyThanhCong)
                {
                    ModelState.AddModelError(string.Empty,
                        $"Vi phạm nghiệp vụ chống duyệt trùng: Học viên '{dangKy.HocVien?.HoTen}' đã có một đơn đăng ký khóa học '{dangKy.KhoaHoc.TenKhoaHoc}' ở trạng thái 'Hoàn thành' trước đó. Hệ thống không cho phép duyệt trùng lặp!");
                    return View(model);
                }
            }

            // 3. KIỂM TRA ĐỐI SOÁT HỌC PHÍ
            if (model.SoTienDaDong < 0)
            {
                ModelState.AddModelError(nameof(model.SoTienDaDong), "Số tiền đã nộp không được âm.");
                return View(model);
            }

            var hocPhiKhoaHoc = dangKy.KhoaHoc?.HocPhi ?? 0;
            var soTienConLai = Math.Max(0, hocPhiKhoaHoc - model.SoTienDaDong);

            // Bắt buộc nhập lý do nếu chuyển sang trạng thái "BiHuy"
            if (model.TrangThaiMoi == "BiHuy" && string.IsNullOrWhiteSpace(model.GhiChu))
            {
                ModelState.AddModelError(nameof(model.GhiChu), "Vui lòng nhập lý do hủy/từ chối đơn đăng ký vào ô ghi chú.");
                return View(model);
            }

            // 4. TIẾN HÀNH CẬP NHẬT CSDL
            try
            {
                var trangThaiCu = dangKy.TrangThai;
                dangKy.TrangThai = model.TrangThaiMoi;
                dangKy.SoTienDaDong = model.SoTienDaDong;
                dangKy.SoTienConLai = soTienConLai;
                dangKy.GhiChu = model.GhiChu?.Trim();

                // Cập nhật ngày xác nhận nếu chuyển sang Hoàn thành hoặc Đang xử lý
                if (model.TrangThaiMoi == "HoanThanh" || model.TrangThaiMoi == "DangXuLy")
                {
                    dangKy.NgayXacNhan = DateTime.Now;
                }

                _context.Update(dangKy);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Đã cập nhật trạng thái đơn #{dangKy.MaDangKy} của học viên '{dangKy.HocVien?.HoTen}' từ '{LayTenTrangThai(trangThaiCu)}' sang '{LayTenTrangThai(dangKy.TrangThai)}' thành công!";
                return RedirectToAction(nameof(Details), new { id = dangKy.MaDangKy });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Lỗi hệ thống khi lưu CSDL: {ex.Message}");
                return View(model);
            }
        }

        // =========================================================================
        // 4. CÁC THAO TÁC XỬ LÝ NHANH TRÊN DANH SÁCH (QUICK ACTIONS)
        // =========================================================================

        // Họ và tên: Bạch Đức Sự
        // Mã sinh viên: 23103100063
        // Nội dung thực hiện: Module 4 - Thao tác nhanh tiếp nhận đơn [Chờ xử lý] -> [Đang xử lý]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TiepNhanNhanh(int id)
        {
            var dangKy = await _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .Include(d => d.KhoaHoc)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dangKy == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn đăng ký.";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra trạng thái hiện tại
            if (dangKy.TrangThai != "ChoXuLy")
            {
                TempData["ErrorMessage"] = $"Chỉ đơn ở trạng thái 'Chờ xử lý' mới có thể tiếp nhận. (Trạng thái hiện tại: {LayTenTrangThai(dangKy.TrangThai)})";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra khóa học mở
            if (dangKy.KhoaHoc == null || dangKy.KhoaHoc.TrangThai != "DangMo")
            {
                TempData["ErrorMessage"] = $"Không thể tiếp nhận: Khóa học '{dangKy.KhoaHoc?.TenKhoaHoc}' không ở trạng thái mở tiếp nhận.";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra sĩ số tối đa bằng LINQ
            var currentEnrolled = await _context.DangKyKhoaHocs
                .CountAsync(d => d.MaKhoaHoc == dangKy.MaKhoaHoc &&
                                 d.MaDangKy != dangKy.MaDangKy &&
                                 (d.TrangThai == "HoanThanh" || d.TrangThai == "DangXuLy"));

            if (currentEnrolled >= dangKy.KhoaHoc.SoLuongToiDa)
            {
                TempData["ErrorMessage"] = $"Không thể tiếp nhận: Khóa học '{dangKy.KhoaHoc.TenKhoaHoc}' đã đủ sĩ số tối đa ({currentEnrolled}/{dangKy.KhoaHoc.SoLuongToiDa}).";
                return RedirectToAction(nameof(Index));
            }

            dangKy.TrangThai = "DangXuLy";
            dangKy.NgayXacNhan = DateTime.Now;
            dangKy.GhiChu = string.IsNullOrEmpty(dangKy.GhiChu)
                ? $"[Tiếp nhận nhanh lúc {DateTime.Now:dd/MM/yyyy HH:mm}]"
                : $"{dangKy.GhiChu} | [Tiếp nhận lúc {DateTime.Now:dd/MM/yyyy HH:mm}]";

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã tiếp nhận hồ sơ #{dangKy.MaDangKy} của '{dangKy.HocVien?.HoTen}' sang trạng thái 'Đang xử lý' thành công!";
            return RedirectToAction(nameof(Index));
        }

        // Họ và tên: Bạch Đức Sự
        // Mã sinh viên: 23103100063
        // Nội dung thực hiện: Module 4 - Thao tác nhanh duyệt xác nhận [Đang xử lý] -> [Hoàn thành]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanHoanThanh(int id, decimal? soTienDong, string? ghiChuXacNhan)
        {
            var dangKy = await _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .Include(d => d.KhoaHoc)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dangKy == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn đăng ký.";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra luồng trạng thái
            if (dangKy.TrangThai != "DangXuLy")
            {
                TempData["ErrorMessage"] = $"Chỉ đơn ở trạng thái 'Đang xử lý' mới được xác nhận Hoàn thành. Vui lòng tiếp nhận đơn trước!";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra khóa học mở
            if (dangKy.KhoaHoc == null || dangKy.KhoaHoc.TrangThai != "DangMo")
            {
                TempData["ErrorMessage"] = $"Không thể duyệt hoàn thành: Khóa học '{dangKy.KhoaHoc?.TenKhoaHoc}' không ở trạng thái mở tiếp nhận.";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra sĩ số
            var currentEnrolled = await _context.DangKyKhoaHocs
                .CountAsync(d => d.MaKhoaHoc == dangKy.MaKhoaHoc &&
                                 d.MaDangKy != dangKy.MaDangKy &&
                                 (d.TrangThai == "HoanThanh" || d.TrangThai == "DangXuLy"));

            if (currentEnrolled >= dangKy.KhoaHoc.SoLuongToiDa)
            {
                TempData["ErrorMessage"] = $"Không thể duyệt: Khóa học '{dangKy.KhoaHoc.TenKhoaHoc}' đã đủ sĩ số tối đa.";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra trùng lặp bản ghi hoàn thành
            var daCoHoanThanh = await _context.DangKyKhoaHocs
                .AnyAsync(d => d.MaHocVien == dangKy.MaHocVien &&
                               d.MaKhoaHoc == dangKy.MaKhoaHoc &&
                               d.MaDangKy != dangKy.MaDangKy &&
                               d.TrangThai == "HoanThanh");

            if (daCoHoanThanh)
            {
                TempData["ErrorMessage"] = $"Không thể duyệt: Học viên '{dangKy.HocVien?.HoTen}' đã có bản ghi đăng ký khóa học này ở trạng thái 'Hoàn thành'!";
                return RedirectToAction(nameof(Index));
            }

            // Cập nhật số tiền
            if (soTienDong.HasValue && soTienDong.Value >= 0)
            {
                dangKy.SoTienDaDong = soTienDong.Value;
            }
            else if (dangKy.SoTienDaDong == 0 && dangKy.KhoaHoc != null)
            {
                // Mặc định nếu chưa nhập thì xem như đóng đủ toàn bộ học phí
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
            TempData["SuccessMessage"] = $"Đã duyệt Hoàn thành đơn #{dangKy.MaDangKy} của '{dangKy.HocVien?.HoTen}' thành công!";
            return RedirectToAction(nameof(Index));
        }

        // Họ và tên: Bạch Đức Sự
        // Mã sinh viên: 23103100063
        // Nội dung thực hiện: Module 4 - Thao tác Hủy/Từ chối đơn đăng ký kèm lý do hủy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDangKy(int id, string lyDoHuy)
        {
            if (string.IsNullOrWhiteSpace(lyDoHuy))
            {
                TempData["ErrorMessage"] = "Vui lòng cung cấp lý do hủy/từ chối đơn đăng ký.";
                return RedirectToAction(nameof(Index));
            }

            var dangKy = await _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dangKy == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn đăng ký.";
                return RedirectToAction(nameof(Index));
            }

            dangKy.TrangThai = "BiHuy";
            dangKy.GhiChu = string.IsNullOrEmpty(dangKy.GhiChu)
                ? $"[Lý do hủy: {lyDoHuy.Trim()} - Ngày: {DateTime.Now:dd/MM/yyyy HH:mm}]"
                : $"{dangKy.GhiChu} | [Lý do hủy: {lyDoHuy.Trim()} - Ngày: {DateTime.Now:dd/MM/yyyy HH:mm}]";

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã hủy đơn đăng ký #{dangKy.MaDangKy} của '{dangKy.HocVien?.HoTen}'. Lý do: {lyDoHuy.Trim()}";
            return RedirectToAction(nameof(Index));
        }

        // =========================================================================
        // 5. THÊM ĐĂNG KÝ MỚI TRỰC TIẾP TẠI QUẦY TIẾP NHẬN (CREATE)
        // =========================================================================

        // Họ và tên: Bạch Đức Sự
        // Mã sinh viên: 23103100063
        // Nội dung thực hiện: Module 4 - Action GET hiển thị Form tiếp nhận ghi danh trực tiếp
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await NapDuLieuDropDownList();
            return View(new TaoDangKyMoiViewModel { TrangThai = "ChoXuLy", SoTienDaDong = 0 });
        }

        // Họ và tên: Bạch Đức Sự
        // Mã sinh viên: 23103100063
        // Nội dung thực hiện: Module 4 - Action POST lưu đơn đăng ký mới tại quầy, kiểm tra điều kiện đầu vào
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaoDangKyMoiViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra khóa học
                var khoaHoc = await _context.KhoaHocs.FindAsync(model.MaKhoaHoc);
                if (khoaHoc == null)
                {
                    ModelState.AddModelError("MaKhoaHoc", "Khóa học đã chọn không tồn tại.");
                    await NapDuLieuDropDownList();
                    return View(model);
                }

                // Kiểm tra trạng thái khóa học
                if (khoaHoc.TrangThai != "DangMo")
                {
                    ModelState.AddModelError("MaKhoaHoc", $"Khóa học '{khoaHoc.TenKhoaHoc}' hiện không ở trạng thái mở đăng ký (Trạng thái: {khoaHoc.TrangThai}).");
                    await NapDuLieuDropDownList();
                    return View(model);
                }

                // Kiểm tra sĩ số tối đa bằng LINQ
                var countEnrolled = await _context.DangKyKhoaHocs
                    .CountAsync(d => d.MaKhoaHoc == model.MaKhoaHoc && (d.TrangThai == "HoanThanh" || d.TrangThai == "DangXuLy"));

                if (countEnrolled >= khoaHoc.SoLuongToiDa)
                {
                    ModelState.AddModelError("MaKhoaHoc", $"Khóa học '{khoaHoc.TenKhoaHoc}' đã hết chỗ tiếp nhận ({countEnrolled}/{khoaHoc.SoLuongToiDa} học viên).");
                    await NapDuLieuDropDownList();
                    return View(model);
                }

                // Kiểm tra học viên đã hoàn thành khóa học này chưa
                var daDangKyThanhCong = await _context.DangKyKhoaHocs
                    .AnyAsync(d => d.MaHocVien == model.MaHocVien && d.MaKhoaHoc == model.MaKhoaHoc && d.TrangThai == "HoanThanh");

                if (daDangKyThanhCong)
                {
                    ModelState.AddModelError("MaHocVien", "Học viên này đã hoàn thành khóa học này trước đó. Không được đăng ký trùng.");
                    await NapDuLieuDropDownList();
                    return View(model);
                }

                // Tính học phí còn lại
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
                    NgayXacNhan = model.TrangThai == "DangXuLy" ? DateTime.Now : null
                };

                _context.DangKyKhoaHocs.Add(dangKy);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Tiếp nhận đơn đăng ký mới #{dangKy.MaDangKy} thành công!";
                return RedirectToAction(nameof(Details), new { id = dangKy.MaDangKy });
            }

            await NapDuLieuDropDownList();
            return View(model);
        }

        // =========================================================================
        // 6. IN PHIẾU ĐĂNG KÝ / BIÊN LAI THU TIỀN (IN ẤN A4)
        // =========================================================================

        // Họ và tên: Bạch Đức Sự
        // Mã sinh viên: 23103100063
        // Nội dung thực hiện: Module 4 - Action In phiếu xác nhận đăng ký và biên lai thu học phí
        [HttpGet]
        public async Task<IActionResult> InPhieuDangKy(int? id)
        {
            if (id == null) return NotFound();

            var dangKy = await _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.MonHoc)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.GiangVien)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dangKy == null) return NotFound();

            return View(dangKy);
        }

        // =========================================================================
        // HÀM TIỆN ÍCH HỖ TRỢ XỬ LÝ NGHIỆP VỤ (HELPER METHODS)
        // =========================================================================

        // Họ và tên: Bạch Đức Sự
        // Mã sinh viên: 23103100063
        // Nội dung thực hiện: Module 4 - Xác định các trạng thái hợp lệ tiếp theo theo luồng quy định
        private static List<string> LayDanhSachTrangThaiHopLe(string trangThaiHienTai)
        {
            return trangThaiHienTai switch
            {
                // Từ [Chờ xử lý] -> được phép chuyển sang [Đang xử lý] hoặc [Hủy] (Không được nhảy thẳng Hoàn thành)
                "ChoXuLy" => new List<string> { "DangXuLy", "BiHuy" },

                // Từ [Đang xử lý] -> được phép duyệt [Hoàn thành], [Hủy] hoặc quay lại [Chờ xử lý]
                "DangXuLy" => new List<string> { "HoanThanh", "BiHuy", "ChoXuLy" },

                // Từ [Hoàn thành] -> Chỉ cho phép [Hủy] nếu có yêu cầu rút hồ sơ đặc biệt
                "HoanThanh" => new List<string> { "BiHuy" },

                // Từ [Đã hủy] -> Cho phép khôi phục về [Chờ xử lý] để xem xét lại
                "BiHuy" => new List<string> { "ChoXuLy" },

                _ => new List<string> { "ChoXuLy", "DangXuLy", "HoanThanh", "BiHuy" }
            };
        }

        // Họ và tên: Bạch Đức Sự
        // Mã sinh viên: 23103100063
        // Nội dung thực hiện: Module 4 - Kiểm tra tính hợp lệ của việc chuyển đổi trạng thái
        private static bool KiemTraChuyenTrangThaiHopLe(string hienTai, string moi)
        {
            // Nếu giữ nguyên trạng thái thì luôn hợp lệ
            if (hienTai == moi) return true;

            var hopLe = LayDanhSachTrangThaiHopLe(hienTai);
            return hopLe.Contains(moi);
        }

        // Họ và tên: Bạch Đức Sự
        // Mã sinh viên: 23103100063
        // Nội dung thực hiện: Module 4 - Chuyển mã trạng thái sang tên tiếng Việt hiển thị
        private static string LayTenTrangThai(string? trangThai)
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

        // Họ và tên: Bạch Đức Sự
        // Mã sinh viên: 23103100063
        // Nội dung thực hiện: Module 4 - Nạp danh sách Học viên và Khóa học đang mở cho DropDownList
        private async Task NapDuLieuDropDownList()
        {
            var hocViens = await _context.HocViens
                .Where(h => h.TrangThai)
                .OrderBy(h => h.HoTen)
                .Select(h => new
                {
                    h.MaHocVien,
                    ThongTin = $"{h.HoTen} - SĐT: {h.SoDienThoai} ({h.Email})"
                })
                .ToListAsync();

            var khoaHocs = await _context.KhoaHocs
                .Where(k => k.TrangThai == "DangMo")
                .OrderBy(k => k.TenKhoaHoc)
                .Select(k => new
                {
                    k.MaKhoaHoc,
                    ThongTin = $"{k.TenKhoaHoc} (Học phí: {k.HocPhi:N0} đ - Tối đa: {k.SoLuongToiDa} HV)"
                })
                .ToListAsync();

            ViewBag.HocViens = new SelectList(hocViens, "MaHocVien", "ThongTin");
            ViewBag.KhoaHocs = new SelectList(khoaHocs, "MaKhoaHoc", "ThongTin");
        }
    }
}
