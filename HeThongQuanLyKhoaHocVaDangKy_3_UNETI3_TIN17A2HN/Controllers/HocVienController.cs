// Họ và tên: Ngọc Tùng Lâm
// Mã sinh viên: 23103100098
// Nội dung thực hiện: Module 3 - Quản lý Học viên & Hồ sơ cá nhân

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Helpers;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    public class HocVienController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HocVienController> _logger;

        public HocVienController(ApplicationDbContext context, ILogger<HocVienController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /HocVien or /HocVien/Index or /HocVien/QuanLy
        [HttpGet]
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> Index(HocVienFilterViewModel filter)
        {
            if (filter.PageIndex < 1) filter.PageIndex = 1;
            if (filter.PageSize < 1) filter.PageSize = 10;

            // 1. Thống kê Stat Cards tổng quan bằng LINQ
            var allQuery = _context.HocViens.AsNoTracking();
            filter.TongSoHocVien = await allQuery.CountAsync();
            filter.SoHocVienHoatDong = await allQuery.CountAsync(h => h.TrangThai);
            filter.SoHocVienBiKhoa = await allQuery.CountAsync(h => !h.TrangThai);
            filter.SoHocVienCoDangKy = await allQuery.CountAsync(h => h.DangKyKhoaHocs.Any(d => d.TrangThai != "BiHuy"));

            // 2. Query cơ sở kèm nạp Eager Loading
            var query = _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyKhoaHocs)
                .AsNoTracking()
                .AsQueryable();

            // 3. Tìm kiếm từ khóa tổng hợp (Họ tên, SĐT, Email, Mã học viên)
            if (!string.IsNullOrWhiteSpace(filter.SearchString))
            {
                var keyword = filter.SearchString.Trim();
                query = query.Where(h =>
                    h.HoTen.Contains(keyword) ||
                    h.SoDienThoai.Contains(keyword) ||
                    h.Email.Contains(keyword) ||
                    (h.TaiKhoan != null && h.TaiKhoan.TenDangNhap.Contains(keyword)) ||
                    h.MaHocVien.ToString() == keyword);
            }

            // 4. Lọc theo Trạng thái hồ sơ
            if (!string.IsNullOrWhiteSpace(filter.TrangThai) && filter.TrangThai != "TatCa")
            {
                if (filter.TrangThai == "HoatDong")
                {
                    query = query.Where(h => h.TrangThai);
                }
                else if (filter.TrangThai == "BiKhoa")
                {
                    query = query.Where(h => !h.TrangThai);
                }
            }

            // 5. Lọc theo Trình độ học vấn
            if (!string.IsNullOrWhiteSpace(filter.TrinhDo) && filter.TrinhDo != "TatCa")
            {
                query = query.Where(h => h.TrinhDo == filter.TrinhDo);
            }

            // 6. Sắp xếp bằng LINQ
            query = (filter.SortBy ?? "ngay_desc") switch
            {
                "ten_asc" => query.OrderBy(h => h.HoTen),
                "ten_desc" => query.OrderByDescending(h => h.HoTen),
                "ngay_asc" => query.OrderBy(h => h.NgayDangKy),
                _ => query.OrderByDescending(h => h.NgayDangKy)
            };

            // 7. Tổng số kết quả lọc
            filter.TotalItems = await query.CountAsync();

            // 8. Phân trang bằng Skip() và Take()
            var pagedData = await query
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(h => new HocVienItemViewModel
                {
                    MaHocVien = h.MaHocVien,
                    MaTaiKhoan = h.MaTaiKhoan,
                    TenDangNhap = h.TaiKhoan != null ? h.TaiKhoan.TenDangNhap : null,
                    HoTen = h.HoTen,
                    NgaySinh = h.NgaySinh,
                    GioiTinh = h.GioiTinh,
                    SoDienThoai = h.SoDienThoai,
                    Email = h.Email,
                    DiaChi = h.DiaChi,
                    TrinhDo = h.TrinhDo,
                    NgayDangKy = h.NgayDangKy,
                    TrangThai = h.TrangThai,
                    GhiChu = h.GhiChu,
                    SoKhoaHocDaDangKy = h.DangKyKhoaHocs.Count(d => d.TrangThai != "BiHuy"),
                    SoKhoaHocHoanThanh = h.DangKyKhoaHocs.Count(d => d.TrangThai == "HoanThanh"),
                    TongHocPhiDaDong = h.DangKyKhoaHocs.Sum(d => d.SoTienDaDong),
                    TongHocPhiConLai = h.DangKyKhoaHocs.Where(d => d.TrangThai != "BiHuy").Sum(d => d.SoTienConLai)
                })
                .ToListAsync();

            filter.DanhSachHocVien = pagedData;
            return View(filter);
        }

        // GET: /HocVien/Details/5
        [HttpGet]
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var hocVien = await _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyKhoaHocs)
                    .ThenInclude(d => d.KhoaHoc)
                        .ThenInclude(k => k!.MonHoc)
                .Include(h => h.DangKyKhoaHocs)
                    .ThenInclude(d => d.KhoaHoc)
                        .ThenInclude(k => k!.GiangVien)
                .Include(h => h.DangKyKhoaHocs)
                    .ThenInclude(d => d.KetQuaHocTap)
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.MaHocVien == id.Value);

            if (hocVien == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ học viên yêu cầu.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new HocVienDetailsViewModel
            {
                HocVien = hocVien,
                TaiKhoan = hocVien.TaiKhoan,
                LịchSuDangKy = hocVien.DangKyKhoaHocs
                    .OrderByDescending(d => d.NgayDangKy)
                    .Select(d => new HocVienDangKyLichSuViewModel
                    {
                        MaDangKy = d.MaDangKy,
                        MaKhoaHoc = d.MaKhoaHoc,
                        TenKhoaHoc = d.KhoaHoc?.TenKhoaHoc ?? "N/A",
                        TenMonHoc = d.KhoaHoc?.MonHoc?.TenMonHoc ?? "N/A",
                        SoTinChi = d.KhoaHoc?.MonHoc?.SoTinChi ?? 0,
                        TenGiangVien = d.KhoaHoc?.GiangVien?.HoTen,
                        NgayDangKy = d.NgayDangKy,
                        TrangThai = d.TrangThai,
                        HocPhi = d.KhoaHoc?.HocPhi ?? 0,
                        SoTienDaDong = d.SoTienDaDong,
                        SoTienConLai = d.SoTienConLai,
                        NgayXacNhan = d.NgayXacNhan,
                        GhiChu = d.GhiChu,
                        DiemTongKet = d.KetQuaHocTap?.DiemTongKet,
                        XepLoai = d.KetQuaHocTap?.XepLoai,
                        KetQua = d.KetQuaHocTap?.KetQua
                    })
                    .ToList()
            };

            return View(viewModel);
        }

        // GET: /HocVien/Create
        [HttpGet]
        [AuthorizeRole("Admin", "NhanVien")]
        public IActionResult Create()
        {
            var model = new HocVienFormViewModel
            {
                TrangThai = true,
                TaoTaiKhoan = true,
                GioiTinh = "Nam",
                TrinhDo = "Đại học"
            };
            return View(model);
        }

        // POST: /HocVien/Create
        [HttpPost]
        [AuthorizeRole("Admin", "NhanVien")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HocVienFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng Email
                var cleanEmail = model.Email.Trim().ToLower();
                if (await _context.HocViens.AnyAsync(h => h.Email.ToLower() == cleanEmail))
                {
                    ModelState.AddModelError("Email", "Địa chỉ Email này đã được đăng ký cho một học viên khác.");
                }

                // Kiểm tra trùng Số điện thoại
                var cleanPhone = model.SoDienThoai.Trim();
                if (await _context.HocViens.AnyAsync(h => h.SoDienThoai == cleanPhone))
                {
                    ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng bởi học viên khác.");
                }

                // Kiểm tra tài khoản nếu có chọn tạo
                TaiKhoan? newTaiKhoan = null;
                if (model.TaoTaiKhoan)
                {
                    if (string.IsNullOrWhiteSpace(model.TenDangNhap))
                    {
                        ModelState.AddModelError("TenDangNhap", "Vui lòng nhập Tên đăng nhập khi bật tùy chọn tạo tài khoản.");
                    }
                    else
                    {
                        var cleanUsername = model.TenDangNhap.Trim().ToLower();
                        if (await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap.ToLower() == cleanUsername))
                        {
                            ModelState.AddModelError("TenDangNhap", "Tên đăng nhập này đã tồn tại trong hệ thống. Vui lòng chọn tên khác.");
                        }

                        if (await _context.TaiKhoans.AnyAsync(t => t.Email.ToLower() == cleanEmail))
                        {
                            ModelState.AddModelError("Email", "Email này đã được liên kết với một tài khoản người dùng khác.");
                        }

                        if (string.IsNullOrWhiteSpace(model.MatKhau))
                        {
                            ModelState.AddModelError("MatKhau", "Vui lòng nhập mật khẩu khởi tạo.");
                        }
                    }
                }

                if (ModelState.IsValid)
                {
                    // 1. Tạo tài khoản nếu yêu cầu
                    if (model.TaoTaiKhoan)
                    {
                        newTaiKhoan = new TaiKhoan
                        {
                            TenDangNhap = model.TenDangNhap!.Trim(),
                            MatKhau = PasswordHelper.HashPassword(model.MatKhau!),
                            HoTen = model.HoTen.Trim(),
                            Email = cleanEmail,
                            VaiTro = VaiTro.HocVien,
                            TrangThai = model.TrangThai,
                            NgayTao = DateTime.Now
                        };
                        _context.TaiKhoans.Add(newTaiKhoan);
                        await _context.SaveChangesAsync();
                    }

                    // 2. Tạo hồ sơ học viên
                    var newHocVien = new HocVien
                    {
                        MaTaiKhoan = newTaiKhoan?.MaTaiKhoan,
                        HoTen = model.HoTen.Trim(),
                        NgaySinh = model.NgaySinh,
                        GioiTinh = model.GioiTinh,
                        SoDienThoai = cleanPhone,
                        Email = cleanEmail,
                        DiaChi = model.DiaChi?.Trim(),
                        TrinhDo = model.TrinhDo?.Trim(),
                        TrangThai = model.TrangThai,
                        GhiChu = model.GhiChu?.Trim(),
                        NgayDangKy = DateTime.Now
                    };

                    _context.HocViens.Add(newHocVien);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Thêm mới học viên '{newHocVien.HoTen}' thành công!";
                    return RedirectToAction(nameof(Details), new { id = newHocVien.MaHocVien });
                }
            }

            return View(model);
        }

        // GET: /HocVien/Edit/5
        [HttpGet]
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var hocVien = await _context.HocViens
                .Include(h => h.TaiKhoan)
                .FirstOrDefaultAsync(h => h.MaHocVien == id.Value);

            if (hocVien == null) return NotFound();

            var model = new HocVienFormViewModel
            {
                MaHocVien = hocVien.MaHocVien,
                MaTaiKhoan = hocVien.MaTaiKhoan,
                HoTen = hocVien.HoTen,
                NgaySinh = hocVien.NgaySinh,
                GioiTinh = hocVien.GioiTinh ?? "Nam",
                SoDienThoai = hocVien.SoDienThoai,
                Email = hocVien.Email,
                DiaChi = hocVien.DiaChi,
                TrinhDo = hocVien.TrinhDo ?? "Đại học",
                TrangThai = hocVien.TrangThai,
                GhiChu = hocVien.GhiChu,
                TaoTaiKhoan = false,
                TenDangNhap = hocVien.TaiKhoan?.TenDangNhap
            };

            return View(model);
        }

        // POST: /HocVien/Edit/5
        [HttpPost]
        [AuthorizeRole("Admin", "NhanVien")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HocVienFormViewModel model)
        {
            if (id != model.MaHocVien) return NotFound();

            if (ModelState.IsValid)
            {
                var hocVien = await _context.HocViens
                    .Include(h => h.TaiKhoan)
                    .FirstOrDefaultAsync(h => h.MaHocVien == id);

                if (hocVien == null) return NotFound();

                // Kiểm tra trùng Email với học viên khác
                var cleanEmail = model.Email.Trim().ToLower();
                if (await _context.HocViens.AnyAsync(h => h.MaHocVien != id && h.Email.ToLower() == cleanEmail))
                {
                    ModelState.AddModelError("Email", "Địa chỉ Email này đã được sử dụng bởi học viên khác.");
                    return View(model);
                }

                // Kiểm tra trùng SĐT với học viên khác
                var cleanPhone = model.SoDienThoai.Trim();
                if (await _context.HocViens.AnyAsync(h => h.MaHocVien != id && h.SoDienThoai == cleanPhone))
                {
                    ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng bởi học viên khác.");
                    return View(model);
                }

                hocVien.HoTen = model.HoTen.Trim();
                hocVien.NgaySinh = model.NgaySinh;
                hocVien.GioiTinh = model.GioiTinh;
                hocVien.SoDienThoai = cleanPhone;
                hocVien.Email = cleanEmail;
                hocVien.DiaChi = model.DiaChi?.Trim();
                hocVien.TrinhDo = model.TrinhDo?.Trim();
                hocVien.TrangThai = model.TrangThai;
                hocVien.GhiChu = model.GhiChu?.Trim();

                // Đồng bộ sang tài khoản đăng nhập liên kết (nếu có)
                if (hocVien.TaiKhoan != null)
                {
                    hocVien.TaiKhoan.HoTen = hocVien.HoTen;
                    hocVien.TaiKhoan.Email = hocVien.Email;
                    hocVien.TaiKhoan.TrangThai = hocVien.TrangThai;
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Cập nhật hồ sơ học viên '{hocVien.HoTen}' thành công!";
                return RedirectToAction(nameof(Details), new { id = hocVien.MaHocVien });
            }

            return View(model);
        }

        // GET: /HocVien/Delete/5
        [HttpGet]
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var hocVien = await _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyKhoaHocs)
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.MaHocVien == id.Value);

            if (hocVien == null) return NotFound();

            return View(hocVien);
        }

        // POST: /HocVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [AuthorizeRole("Admin", "NhanVien")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hocVien = await _context.HocViens
                .Include(h => h.DangKyKhoaHocs)
                .Include(h => h.TaiKhoan)
                .FirstOrDefaultAsync(h => h.MaHocVien == id);

            if (hocVien == null) return NotFound();

            // KIỂM TRA RÀNG BUỘC TOÀN VẸN DỮ LIỆU THEO MỤC 6.2 & 10 ĐỀ 16:
            // "Không xóa đối tượng đã phát sinh lịch sử nếu làm mất tính toàn vẹn dữ liệu"
            if (hocVien.DangKyKhoaHocs.Any())
            {
                TempData["ErrorMessage"] = $"Không thể xóa học viên '{hocVien.HoTen}' vì đã có {hocVien.DangKyKhoaHocs.Count} lịch sử đăng ký khóa học. Để đảm bảo tính toàn vẹn dữ liệu, vui lòng chuyển trạng thái học viên sang 'Khóa/Nghỉ'.";
                return RedirectToAction(nameof(Details), new { id = hocVien.MaHocVien });
            }

            _context.HocViens.Remove(hocVien);
            if (hocVien.TaiKhoan != null)
            {
                _context.TaiKhoans.Remove(hocVien.TaiKhoan);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã xóa hồ sơ học viên '{hocVien.HoTen}' thành công.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /HocVien/ToggleTrangThai/5
        [HttpPost]
        [AuthorizeRole("Admin", "NhanVien")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleTrangThai(int id)
        {
            var hocVien = await _context.HocViens
                .Include(h => h.TaiKhoan)
                .FirstOrDefaultAsync(h => h.MaHocVien == id);

            if (hocVien == null) return NotFound();

            hocVien.TrangThai = !hocVien.TrangThai;
            if (hocVien.TaiKhoan != null)
            {
                hocVien.TaiKhoan.TrangThai = hocVien.TrangThai;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã {(hocVien.TrangThai ? "kích hoạt" : "tạm khóa")} hồ sơ học viên '{hocVien.HoTen}' thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /HocVien/HoSoCaNhan
        [HttpGet]
        [AuthorizeRole("HocVien", "Admin", "NhanVien")]
        public async Task<IActionResult> HoSoCaNhan()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!maTaiKhoan.HasValue)
            {
                return RedirectToAction("Login", "TaiKhoan", new { returnUrl = Url.Action(nameof(HoSoCaNhan)) });
            }

            // Tự động tìm hồ sơ học viên liên kết với tài khoản đang đăng nhập
            var hocVien = await _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyKhoaHocs)
                .FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan.Value);

            // Nếu tài khoản Học viên nhưng chưa có bản ghi HocVien, tự động khởi tạo
            if (hocVien == null)
            {
                var taiKhoan = await _context.TaiKhoans.FindAsync(maTaiKhoan.Value);
                if (taiKhoan == null) return RedirectToAction("Login", "TaiKhoan");

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

            var model = new HoSoCaNhanViewModel
            {
                MaHocVien = hocVien.MaHocVien,
                MaTaiKhoan = hocVien.MaTaiKhoan,
                TenDangNhap = hocVien.TaiKhoan?.TenDangNhap ?? HttpContext.Session.GetString("TenDangNhap") ?? "N/A",
                HoTen = hocVien.HoTen,
                NgaySinh = hocVien.NgaySinh,
                GioiTinh = hocVien.GioiTinh ?? "Nam",
                SoDienThoai = hocVien.SoDienThoai,
                Email = hocVien.Email,
                DiaChi = hocVien.DiaChi,
                TrinhDo = hocVien.TrinhDo ?? "Đại học",
                NgayDangKy = hocVien.NgayDangKy,
                TrangThai = hocVien.TrangThai,
                GhiChu = hocVien.GhiChu,
                TongKhoaHocDaDangKy = hocVien.DangKyKhoaHocs?.Count(d => d.TrangThai != "BiHuy") ?? 0,
                SoKhoaHocHoanThanh = hocVien.DangKyKhoaHocs?.Count(d => d.TrangThai == "HoanThanh") ?? 0,
                SoKhoaHocChoXuLy = hocVien.DangKyKhoaHocs?.Count(d => d.TrangThai == "ChoXuLy") ?? 0,
                TongHocPhiDaDong = hocVien.DangKyKhoaHocs?.Sum(d => d.SoTienDaDong) ?? 0,
                TongHocPhiConLai = hocVien.DangKyKhoaHocs?.Where(d => d.TrangThai != "BiHuy").Sum(d => d.SoTienConLai) ?? 0
            };

            return View(model);
        }

        // POST: /HocVien/CapNhatHoSo
        [HttpPost]
        [AuthorizeRole("HocVien", "Admin", "NhanVien")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatHoSo(HoSoCaNhanViewModel model)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!maTaiKhoan.HasValue)
            {
                return RedirectToAction("Login", "TaiKhoan");
            }

            // BẢO MẬT DỮ LIỆU: Tìm chính xác học viên thuộc tài khoản Session hiện tại
            var hocVien = await _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyKhoaHocs)
                .FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan.Value);

            if (hocVien == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ học viên của bạn để cập nhật.";
                return RedirectToAction(nameof(HoSoCaNhan));
            }

            // Kiểm tra ngày sinh hợp lệ (không lớn hơn ngày hiện tại)
            if (model.NgaySinh.HasValue && model.NgaySinh.Value.Date > DateTime.Today)
            {
                ModelState.AddModelError("NgaySinh", "Ngày sinh không được vượt quá ngày hiện tại.");
            }

            // Kiểm tra trùng Email với người khác
            var cleanEmail = model.Email.Trim().ToLower();
            if (await _context.HocViens.AnyAsync(h => h.MaHocVien != hocVien.MaHocVien && h.Email.ToLower() == cleanEmail))
            {
                ModelState.AddModelError("Email", "Địa chỉ Email này đã được sử dụng bởi người dùng khác.");
            }

            // Kiểm tra trùng SĐT với người khác
            var cleanPhone = model.SoDienThoai.Trim();
            if (await _context.HocViens.AnyAsync(h => h.MaHocVien != hocVien.MaHocVien && h.SoDienThoai == cleanPhone))
            {
                ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng bởi người dùng khác.");
            }

            if (ModelState.IsValid)
            {
                hocVien.HoTen = model.HoTen.Trim();
                hocVien.NgaySinh = model.NgaySinh;
                hocVien.GioiTinh = model.GioiTinh;
                hocVien.SoDienThoai = cleanPhone;
                hocVien.Email = cleanEmail;
                hocVien.DiaChi = model.DiaChi?.Trim();
                hocVien.TrinhDo = model.TrinhDo?.Trim();
                hocVien.GhiChu = model.GhiChu?.Trim();

                // Đồng bộ sang TaiKhoan
                if (hocVien.TaiKhoan != null)
                {
                    hocVien.TaiKhoan.HoTen = hocVien.HoTen;
                    hocVien.TaiKhoan.Email = hocVien.Email;
                }

                await _context.SaveChangesAsync();

                // Cập nhật lại Session
                HttpContext.Session.SetString("HoTen", hocVien.HoTen);

                TempData["SuccessMessage"] = "Hồ sơ cá nhân của bạn đã được cập nhật thành công!";
                return RedirectToAction(nameof(HoSoCaNhan));
            }

            // Tái nạp thông tin hiển thị nếu Model lỗi
            model.MaHocVien = hocVien.MaHocVien;
            model.MaTaiKhoan = hocVien.MaTaiKhoan;
            model.TenDangNhap = hocVien.TaiKhoan?.TenDangNhap ?? "N/A";
            model.NgayDangKy = hocVien.NgayDangKy;
            model.TrangThai = hocVien.TrangThai;
            model.TongKhoaHocDaDangKy = hocVien.DangKyKhoaHocs?.Count ?? 0;
            model.SoKhoaHocHoanThanh = hocVien.DangKyKhoaHocs?.Count(d => d.TrangThai == "HoanThanh") ?? 0;
            model.SoKhoaHocChoXuLy = hocVien.DangKyKhoaHocs?.Count(d => d.TrangThai == "ChoXuLy") ?? 0;
            model.TongHocPhiDaDong = hocVien.DangKyKhoaHocs?.Sum(d => d.SoTienDaDong) ?? 0;
            model.TongHocPhiConLai = hocVien.DangKyKhoaHocs?.Where(d => d.TrangThai != "BiHuy").Sum(d => d.SoTienConLai) ?? 0;

            return View("HoSoCaNhan", model);
        }
    }
}
