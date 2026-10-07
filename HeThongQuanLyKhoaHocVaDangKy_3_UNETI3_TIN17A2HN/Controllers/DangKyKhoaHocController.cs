// Họ và tên: Ngọc Tùng Lâm
// Mã sinh viên: 23103100098
// Nội dung thực hiện: Module 3 - Đăng ký Khóa học & Theo dõi Đăng ký

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    public class DangKyKhoaHocController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DangKyKhoaHocController> _logger;

        public DangKyKhoaHocController(ApplicationDbContext context, ILogger<DangKyKhoaHocController> logger)
        {
            _context = context;
            _logger = logger;
        }

        
        [HttpGet]
        public async Task<IActionResult> DangKy(int? maKhoaHoc)
        {
            if (maKhoaHoc == null)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn một khóa học cụ thể để tiến hành đăng ký.";
                return RedirectToAction("Index", "KhoaHoc");
            }

            // 1. Kiểm tra trạng thái đăng nhập
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!maTaiKhoan.HasValue)
            {
                return RedirectToAction("Login", "TaiKhoan", new { returnUrl = Url.Action(nameof(DangKy), new { maKhoaHoc }) });
            }

            // 2. Tìm hoặc khởi tạo hồ sơ học viên liên kết
            var hocVien = await _context.HocViens
                .Include(h => h.TaiKhoan)
                .FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan.Value);

            if (hocVien == null)
            {
                var taiKhoan = await _context.TaiKhoans.FindAsync(maTaiKhoan.Value);
                if (taiKhoan == null || !taiKhoan.TrangThai)
                {
                    HttpContext.Session.Clear();
                    TempData["ErrorMessage"] = "Tài khoản của bạn không hợp lệ hoặc đã bị khóa.";
                    return RedirectToAction("Login", "TaiKhoan");
                }

                hocVien = new HocVien
                {
                    MaTaiKhoan = taiKhoan.MaTaiKhoan,
                    HoTen = taiKhoan.HoTen,
                    Email = taiKhoan.Email,
                    SoDienThoai = string.Empty,
                    NgayDangKy = DateTime.Now,
                    TrangThai = true,
                    TrinhDo = "Đại học",
                    GioiTinh = "Nam"
                };
                _context.HocViens.Add(hocVien);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetInt32("MaHocVien", hocVien.MaHocVien);
            }

            // 3. Kiểm tra tài khoản học viên có bị khóa không
            if (!hocVien.TrangThai)
            {
                TempData["ErrorMessage"] = "Hồ sơ học viên của bạn đang ở trạng thái tạm khóa. Vui lòng liên hệ Văn phòng đào tạo để được hỗ trợ.";
                return RedirectToAction("Index", "KhoaHoc");
            }

            // 4. Lấy thông tin chi tiết Khóa học
            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.GiangVien)
                .Include(k => k.DangKyKhoaHocs)
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.MaKhoaHoc == maKhoaHoc.Value);

            if (khoaHoc == null)
            {
                TempData["ErrorMessage"] = "Khóa học được yêu cầu không tồn tại trong hệ thống.";
                return RedirectToAction("Index", "KhoaHoc");
            }

            // 5. Tính toán sĩ số hiện tại và số chỗ còn lại
            var siSoHienTai = khoaHoc.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy");
            var soChoConLai = Math.Max(0, khoaHoc.SoLuongToiDa - siSoHienTai);

            // 6. Kiểm tra giao dịch trùng lặp theo Mục 7.3 & 8.3 Đề 16:
            // "Một học viên không được đăng ký trùng cùng khóa học"
            var donDangKyHienCo = await _context.DangKyKhoaHocs
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.MaHocVien == hocVien.MaHocVien && d.MaKhoaHoc == maKhoaHoc.Value && d.TrangThai != "BiHuy");

            var model = new DangKyKhoaHocViewModel
            {
                MaKhoaHoc = khoaHoc.MaKhoaHoc,
                TenKhoaHoc = khoaHoc.TenKhoaHoc,
                TenMonHoc = khoaHoc.MonHoc?.TenMonHoc ?? "N/A",
                SoTinChi = khoaHoc.MonHoc?.SoTinChi ?? 0,
                HocPhi = khoaHoc.HocPhi,
                TenGiangVien = khoaHoc.GiangVien?.HoTen,
                HocVi = khoaHoc.GiangVien?.HocVi,
                NgayBatDau = khoaHoc.NgayBatDau,
                NgayKetThuc = khoaHoc.NgayKetThuc,
                HinhThuc = khoaHoc.HinhThuc,
                TrangThaiKhoaHoc = khoaHoc.TrangThai,
                SoLuongToiDa = khoaHoc.SoLuongToiDa,
                SiSoHienTai = siSoHienTai,
                MoTaKhoaHoc = khoaHoc.MoTa,

                MaHocVien = hocVien.MaHocVien,
                HoTenHocVien = hocVien.HoTen,
                EmailHocVien = hocVien.Email,
                SoDienThoaiHocVien = hocVien.SoDienThoai,

                DaDangKy = donDangKyHienCo != null,
                MaDangKyCu = donDangKyHienCo?.MaDangKy,
                TrangThaiDangKyCu = donDangKyHienCo?.TrangThai
            };

            // Xác định lý do không khả dụng (nếu có)
            if (khoaHoc.TrangThai != "DangMo")
            {
                model.LyDoKhongKhaDung = $"Khóa học hiện không ở trạng thái mở đăng ký (Trạng thái: {khoaHoc.TrangThai}).";
            }
            else if (soChoConLai <= 0)
            {
                model.LyDoKhongKhaDung = "Khóa học đã đủ sĩ số tối đa, không còn chỗ nhận thêm.";
            }
            else if (khoaHoc.NgayBatDau.Date < DateTime.Today)
            {
                model.LyDoKhongKhaDung = "Khóa học đã quá hạn ngày khai giảng bắt đầu.";
            }

            return View(model);
        }

        // POST: /DangKyKhoaHoc/DangKy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(DangKyKhoaHocViewModel model)
        {
            // 1. Kiểm tra xác thực Session
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!maTaiKhoan.HasValue)
            {
                return RedirectToAction("Login", "TaiKhoan", new { returnUrl = Url.Action(nameof(DangKy), new { maKhoaHoc = model.MaKhoaHoc }) });
            }

            // 2. Xác định học viên thực tế từ Session để chống giả mạo MaHocVien
            var hocVien = await _context.HocViens
                .Include(h => h.TaiKhoan)
                .FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan.Value);

            if (hocVien == null || !hocVien.TrangThai)
            {
                TempData["ErrorMessage"] = "Hồ sơ học viên không hợp lệ hoặc đã bị khóa.";
                return RedirectToAction("Index", "KhoaHoc");
            }

            // 3. Tải khóa học và kiểm tra điều kiện
            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.DangKyKhoaHocs)
                .FirstOrDefaultAsync(k => k.MaKhoaHoc == model.MaKhoaHoc);

            if (khoaHoc == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy khóa học cần đăng ký.";
                return RedirectToAction("Index", "KhoaHoc");
            }

            // 4. Kiểm tra điều kiện khả dụng của khóa học
            if (khoaHoc.TrangThai != "DangMo")
            {
                ModelState.AddModelError(string.Empty, "Khóa học hiện không trong thời gian mở tiếp nhận đăng ký.");
            }

            var siSoHienTai = khoaHoc.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy");
            if (siSoHienTai >= khoaHoc.SoLuongToiDa)
            {
                ModelState.AddModelError(string.Empty, "Khóa học này đã đủ số lượng học viên tối đa.");
            }

            if (khoaHoc.NgayBatDau.Date < DateTime.Today)
            {
                ModelState.AddModelError(string.Empty, "Khóa học đã quá ngày bắt đầu khai giảng, không thể tiếp nhận thêm học viên.");
            }

            // 5. Kiểm tra chống đăng ký trùng giao dịch theo Mục 7.3 & 13 Đề 16
            var daDangKy = await _context.DangKyKhoaHocs.AnyAsync(d =>
                d.MaHocVien == hocVien.MaHocVien &&
                d.MaKhoaHoc == model.MaKhoaHoc &&
                d.TrangThai != "BiHuy");

            if (daDangKy)
            {
                ModelState.AddModelError(string.Empty, "Bạn đã đăng ký khóa học này trước đó. Không được tạo giao dịch trùng lặp.");
            }

            if (!model.DongYQuyChe)
            {
                ModelState.AddModelError(nameof(model.DongYQuyChe), "Bạn phải đồng ý với quy chế đào tạo để tiếp tục đăng ký.");
            }

            if (ModelState.IsValid)
            {
                // 6. Khởi tạo đơn đăng ký: Thời điểm do hệ thống xác định, trạng thái ChoXuLy
                var dangKyMoi = new DangKyKhoaHoc
                {
                    MaHocVien = hocVien.MaHocVien,
                    MaKhoaHoc = khoaHoc.MaKhoaHoc,
                    NgayDangKy = DateTime.Now, // Do hệ thống xác định (Mục 7.3)
                    TrangThai = "ChoXuLy",     // Mặc định ban đầu
                    SoTienDaDong = 0,          // Mới đăng ký trực tuyến chưa thanh toán
                    SoTienConLai = khoaHoc.HocPhi,
                    GhiChu = string.IsNullOrWhiteSpace(model.GhiChu) ? null : model.GhiChu.Trim(),
                    NgayXacNhan = null
                };

                _context.DangKyKhoaHocs.Add(dangKyMoi);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Đăng ký khóa học '{khoaHoc.TenKhoaHoc}' thành công! Mã giao dịch của bạn là #{dangKyMoi.MaDangKy}. Đơn của bạn đang ở trạng thái 'Chờ xử lý'.";
                return RedirectToAction(nameof(ChiTiet), new { id = dangKyMoi.MaDangKy });
            }

            // Nếu lỗi, nạp lại dữ liệu hiển thị trên View
            model.TenKhoaHoc = khoaHoc.TenKhoaHoc;
            model.TenMonHoc = khoaHoc.MonHoc?.TenMonHoc ?? "N/A";
            model.SoTinChi = khoaHoc.MonHoc?.SoTinChi ?? 0;
            model.HocPhi = khoaHoc.HocPhi;
            model.NgayBatDau = khoaHoc.NgayBatDau;
            model.NgayKetThuc = khoaHoc.NgayKetThuc;
            model.HinhThuc = khoaHoc.HinhThuc;
            model.TrangThaiKhoaHoc = khoaHoc.TrangThai;
            model.SoLuongToiDa = khoaHoc.SoLuongToiDa;
            model.SiSoHienTai = siSoHienTai;
            model.MaHocVien = hocVien.MaHocVien;
            model.HoTenHocVien = hocVien.HoTen;
            model.EmailHocVien = hocVien.Email;
            model.SoDienThoaiHocVien = hocVien.SoDienThoai;

            return View(model);
        }

        [HttpGet]
        [AuthorizeRole("HocVien", "Admin", "NhanVien")]
        public async Task<IActionResult> Index(TheoDoiDangKyFilterViewModel filter)
        {
            if (filter.PageIndex < 1) filter.PageIndex = 1;
            if (filter.PageSize < 1) filter.PageSize = 8;

            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!maTaiKhoan.HasValue)
            {
                return RedirectToAction("Login", "TaiKhoan", new { returnUrl = Url.Action(nameof(Index)) });
            }

            var hocVien = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan.Value);
            if (hocVien == null)
            {
                // Nếu chưa có hồ sơ học viên, chuyển về trang cập nhật hồ sơ
                return RedirectToAction("HoSoCaNhan", "HocVien");
            }

            // 1. Thống kê Stat Cards cá nhân bằng LINQ
            var myDonQuery = _context.DangKyKhoaHocs
                .Where(d => d.MaHocVien == hocVien.MaHocVien)
                .AsNoTracking();

            filter.TongSoDon = await myDonQuery.CountAsync();
            filter.SoDonChoXuLy = await myDonQuery.CountAsync(d => d.TrangThai == "ChoXuLy");
            filter.SoDonDangXuLy = await myDonQuery.CountAsync(d => d.TrangThai == "DangXuLy");
            filter.SoDonHoanThanh = await myDonQuery.CountAsync(d => d.TrangThai == "HoanThanh");
            filter.SoDonBiHuy = await myDonQuery.CountAsync(d => d.TrangThai == "BiHuy");
            filter.TongTienDaDong = await myDonQuery.SumAsync(d => (decimal?)d.SoTienDaDong) ?? 0;
            filter.TongTienConLai = await myDonQuery.Where(d => d.TrangThai != "BiHuy").SumAsync(d => (decimal?)d.SoTienConLai) ?? 0;

            // 2. Query danh sách giao dịch cá nhân
            var query = _context.DangKyKhoaHocs
                .Where(d => d.MaHocVien == hocVien.MaHocVien)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.MonHoc)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.GiangVien)
                .Include(d => d.KetQuaHocTap)
                .AsNoTracking()
                .AsQueryable();

            // 3. Lọc theo từ khóa khóa học / môn học / mã đơn (hỗ trợ dạng 1, #1, 01, ĐK1...)
            if (!string.IsNullOrWhiteSpace(filter.SearchKhoaHoc))
            {
                var keyword = filter.SearchKhoaHoc.Trim();
                int? parsedId = null;
                var digits = System.Text.RegularExpressions.Regex.Replace(keyword, @"\D", "");
                if (int.TryParse(digits, out int numVal) && numVal > 0)
                {
                    parsedId = numVal;
                }

                if (parsedId.HasValue)
                {
                    int id = parsedId.Value;
                    query = query.Where(d =>
                        d.MaDangKy == id ||
                        (d.KhoaHoc != null && d.KhoaHoc.TenKhoaHoc.Contains(keyword)) ||
                        (d.KhoaHoc != null && d.KhoaHoc.MonHoc != null && d.KhoaHoc.MonHoc.TenMonHoc.Contains(keyword)));
                }
                else
                {
                    query = query.Where(d =>
                        (d.KhoaHoc != null && d.KhoaHoc.TenKhoaHoc.Contains(keyword)) ||
                        (d.KhoaHoc != null && d.KhoaHoc.MonHoc != null && d.KhoaHoc.MonHoc.TenMonHoc.Contains(keyword)));
                }
            }

            // 4. Lọc theo trạng thái đơn
            if (!string.IsNullOrWhiteSpace(filter.TrangThai) && filter.TrangThai != "TatCa")
            {
                query = query.Where(d => d.TrangThai == filter.TrangThai);
            }

            // 5. Sắp xếp LINQ
            query = (filter.SortBy ?? "ngay_desc") switch
            {
                "ngay_asc" => query.OrderBy(d => d.NgayDangKy),
                "hocphi_desc" => query.OrderByDescending(d => d.KhoaHoc != null ? d.KhoaHoc.HocPhi : 0),
                "hocphi_asc" => query.OrderBy(d => d.KhoaHoc != null ? d.KhoaHoc.HocPhi : 0),
                _ => query.OrderByDescending(d => d.NgayDangKy)
            };

            // 6. Tổng số kết quả lọc
            filter.TotalItems = await query.CountAsync();

            // 7. Phân trang Skip / Take
            var listItems = await query
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(d => new TheoDoiDangKyItemViewModel
                {
                    MaDangKy = d.MaDangKy,
                    MaKhoaHoc = d.MaKhoaHoc,
                    TenKhoaHoc = d.KhoaHoc != null ? d.KhoaHoc.TenKhoaHoc : "N/A",
                    TenMonHoc = d.KhoaHoc != null && d.KhoaHoc.MonHoc != null ? d.KhoaHoc.MonHoc.TenMonHoc : "N/A",
                    SoTinChi = d.KhoaHoc != null && d.KhoaHoc.MonHoc != null ? d.KhoaHoc.MonHoc.SoTinChi : 0,
                    TenGiangVien = d.KhoaHoc != null && d.KhoaHoc.GiangVien != null ? d.KhoaHoc.GiangVien.HoTen : null,
                    NgayBatDau = d.KhoaHoc != null ? d.KhoaHoc.NgayBatDau : DateTime.MinValue,
                    NgayKetThuc = d.KhoaHoc != null ? d.KhoaHoc.NgayKetThuc : DateTime.MinValue,
                    HinhThuc = d.KhoaHoc != null ? d.KhoaHoc.HinhThuc : "Trực tiếp",
                    NgayDangKy = d.NgayDangKy,
                    TrangThai = d.TrangThai,
                    HocPhi = d.KhoaHoc != null ? d.KhoaHoc.HocPhi : 0,
                    SoTienDaDong = d.SoTienDaDong,
                    SoTienConLai = d.SoTienConLai,
                    NgayXacNhan = d.NgayXacNhan,
                    GhiChu = d.GhiChu,
                    DiemTongKet = d.KetQuaHocTap != null ? d.KetQuaHocTap.DiemTongKet : null,
                    XepLoai = d.KetQuaHocTap != null ? d.KetQuaHocTap.XepLoai : null,
                    KetQua = d.KetQuaHocTap != null ? d.KetQuaHocTap.KetQua : null
                })
                .ToListAsync();

            filter.DanhSachDangKy = listItems;
            return View(filter);
        }

        [HttpGet]
        [AuthorizeRole("HocVien", "Admin", "NhanVien")]
        public async Task<IActionResult> ChiTiet(int? id)
        {
            if (id == null) return NotFound();

            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!maTaiKhoan.HasValue)
            {
                return RedirectToAction("Login", "TaiKhoan");
            }

            var dangKy = await _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.MonHoc)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.GiangVien)
                .Include(d => d.KetQuaHocTap)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.MaDangKy == id.Value);

            if (dangKy == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ đăng ký khóa học yêu cầu.";
                return RedirectToAction(nameof(Index));
            }

            // KIỂM TRA QUYỀN TRUY CẬP DỮ LIỆU CHÍNH CHỦ (DATA OWNERSHIP CHECK)
            var hocVien = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan.Value);
            bool isStaffOrAdmin = vaiTro == VaiTro.Admin || vaiTro == VaiTro.NhanVien;

            if (!isStaffOrAdmin && (hocVien == null || dangKy.MaHocVien != hocVien.MaHocVien))
            {
                _logger.LogWarning("Phát hiện truy cập trái phép mã đơn đăng ký #{MaDangKy} bởi MaTaiKhoan {MaTaiKhoan}", id, maTaiKhoan);
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập hồ sơ đăng ký của học viên khác!";
                return RedirectToAction(nameof(Index));
            }

            return View(dangKy);
        }

        // "Chỉ được hủy trong các trạng thái cho phép. Các giao dịch đã hoàn tất hoặc đã phát sinh kết quả cuối cùng không được hủy tùy ý."
        [HttpPost]
        [AuthorizeRole("HocVien", "Admin", "NhanVien")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDangKy(int id, string? lyDoHuy)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!maTaiKhoan.HasValue) return RedirectToAction("Login", "TaiKhoan");

            var dangKy = await _context.DangKyKhoaHocs
                .Include(d => d.KhoaHoc)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dangKy == null) return NotFound();

            // Kiểm tra quyền dữ liệu
            var hocVien = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan.Value);
            bool isStaffOrAdmin = vaiTro == VaiTro.Admin || vaiTro == VaiTro.NhanVien;

            if (!isStaffOrAdmin && (hocVien == null || dangKy.MaHocVien != hocVien.MaHocVien))
            {
                TempData["ErrorMessage"] = "Bạn không thể hủy đơn đăng ký của học viên khác!";
                return RedirectToAction(nameof(Index));
            }

            // KIỂM TRA ĐIỀU KIỆN TRẠNG THÁI CHO PHÉP HỦY
            // Học viên chỉ được hủy khi đơn còn ở trạng thái "ChoXuLy"
            if (!isStaffOrAdmin && dangKy.TrangThai != "ChoXuLy")
            {
                TempData["ErrorMessage"] = $"Đơn đăng ký #{dangKy.MaDangKy} đang ở trạng thái '{dangKy.TrangThai}'. Theo quy chế, đơn đã tiếp nhận hoặc hoàn tất không được hủy tự do trực tuyến. Vui lòng liên hệ trực tiếp Bộ phận đào tạo.";
                return RedirectToAction(nameof(ChiTiet), new { id = dangKy.MaDangKy });
            }

            if (dangKy.TrangThai == "BiHuy")
            {
                TempData["ErrorMessage"] = "Đơn đăng ký này đã được hủy trước đó.";
                return RedirectToAction(nameof(ChiTiet), new { id = dangKy.MaDangKy });
            }

            // Tiến hành hủy đơn
            dangKy.TrangThai = "BiHuy";
            dangKy.SoTienConLai = 0; // Khi hủy đơn, công nợ học phí của giao dịch này về 0
            var lyDo = string.IsNullOrWhiteSpace(lyDoHuy) ? "Học viên tự hủy qua cổng trực tuyến" : lyDoHuy.Trim();
            var thongTinHuy = $"[Học viên tự hủy: {lyDo} - Thời gian: {DateTime.Now:dd/MM/yyyy HH:mm}]";

            dangKy.GhiChu = string.IsNullOrEmpty(dangKy.GhiChu)
                ? thongTinHuy
                : $"{dangKy.GhiChu} | {thongTinHuy}";

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã hủy đơn đăng ký #{dangKy.MaDangKy} thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /DangKyKhoaHoc/InPhieu/5
        [HttpGet]
        [AuthorizeRole("HocVien", "Admin", "NhanVien")]
        public async Task<IActionResult> InPhieu(int? id)
        {
            if (id == null) return NotFound();

            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!maTaiKhoan.HasValue) return RedirectToAction("Login", "TaiKhoan");

            var dangKy = await _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.MonHoc)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.GiangVien)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.MaDangKy == id.Value);

            if (dangKy == null) return NotFound();

            // Kiểm tra bảo mật
            var hocVien = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan.Value);
            bool isStaffOrAdmin = vaiTro == VaiTro.Admin || vaiTro == VaiTro.NhanVien;

            if (!isStaffOrAdmin && (hocVien == null || dangKy.MaHocVien != hocVien.MaHocVien))
            {
                TempData["ErrorMessage"] = "Bạn không có quyền in phiếu đăng ký của người khác!";
                return RedirectToAction(nameof(Index));
            }

            return View(dangKy);
        }
    }
}
