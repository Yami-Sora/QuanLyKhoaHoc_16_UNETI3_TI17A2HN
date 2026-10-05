// Họ và tên: Nguyễn Văn Mạnh
// Mã sinh viên: 23103100096
// Nội dung thực hiện: Module 2 - Quản lý Khóa học (CRUD, Tìm kiếm đa tiêu chí, Cập nhật trạng thái)

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;

using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    public class KhoaHocController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KhoaHocController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================================
        // 1. CỔNG SINH VIÊN & KHÁCH HÀNG (PUBLIC COURSE PORTAL)
        // 6.3 Tìm kiếm: Tên khóa học, Tên môn học, Tên giảng viên
        // 6.4 Lọc kết hợp: Môn học, Hình thức, Trạng thái, Khoảng học phí, Còn chỗ/Hết chỗ
        // 6.5 Sắp xếp: Tên A->Z, Z->A, Ngày BĐ tăng/giảm, Học phí tăng/giảm, Số lượng còn lại tăng/giảm
        // 6.6 Phân trang: EF Core Skip(), Take(), giữ nguyên điều kiện khi chuyển trang
        // 6.7 Hiển thị cho người dùng: Rõ ràng khả dụng / không khả dụng theo nghiệp vụ
        // =========================================================================

        // GET: /KhoaHoc or /KhoaHoc/Index
        [HttpGet]
        public async Task<IActionResult> Index(KhoaHocFilterViewModel filter)
        {
            // Dropdown môn học cho bộ lọc
            ViewBag.MonHocList = new SelectList(
                await _context.MonHocs
                    .Where(m => m.TrangThai)
                    .OrderBy(m => m.TenMonHoc)
                    .Select(m => new { m.MaMonHoc, Display = m.TenMonHoc + " (" + m.SoTinChi + " TC)" })
                    .ToListAsync(),
                "MaMonHoc", "Display", filter.MaMonHoc);

            // Thống kê tổng thể cho Stat Cards
            var allCoursesQuery = _context.KhoaHocs
                .Include(k => k.DangKyKhoaHocs)
                .AsNoTracking();

            filter.TongSoKhoaHoc = await allCoursesQuery.CountAsync();
            filter.SoKhoaDangMo = await allCoursesQuery.CountAsync(k => k.TrangThai == "DangMo");
            filter.SoKhoaConCho = await allCoursesQuery.CountAsync(k =>
                k.SoLuongToiDa > k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"));
            filter.SoKhoaKhaDung = await allCoursesQuery.CountAsync(k =>
                k.TrangThai == "DangMo" &&
                k.NgayBatDau >= DateTime.Today &&
                k.SoLuongToiDa > k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"));
            filter.HocPhiThapNhat = await allCoursesQuery.AnyAsync()
                ? await allCoursesQuery.MinAsync(k => k.HocPhi)
                : 0m;
            filter.HocPhiCaoNhat = await allCoursesQuery.AnyAsync()
                ? await allCoursesQuery.MaxAsync(k => k.HocPhi)
                : 0m;

            // Truy vấn cơ sở với Include
            var query = _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.GiangVien)
                .Include(k => k.DangKyKhoaHocs)
                .AsNoTracking()
                .AsQueryable();

            // -------------------------------------------------------------
            // BƯỚC 1: TÌM KIẾM ĐA TIÊU CHÍ (Mục 6.3)
            // Tên khóa học, Tên môn học, Tên giảng viên
            // -------------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(filter.SearchString))
            {
                var keyword = filter.SearchString.Trim().ToLower();
                query = query.Where(k =>
                    k.TenKhoaHoc.ToLower().Contains(keyword) ||
                    (k.MonHoc != null && k.MonHoc.TenMonHoc.ToLower().Contains(keyword)) ||
                    (k.GiangVien != null && k.GiangVien.HoTen.ToLower().Contains(keyword))
                );
            }

            // -------------------------------------------------------------
            // BƯỚC 2: BỘ LỌC KẾT HỢP (Mục 6.4)
            // • Môn học
            // • Hình thức
            // • Trạng thái
            // • Khoảng học phí
            // • Còn chỗ / Hết chỗ
            // -------------------------------------------------------------

            // Lọc theo Môn học
            if (filter.MaMonHoc.HasValue && filter.MaMonHoc.Value > 0)
            {
                query = query.Where(k => k.MaMonHoc == filter.MaMonHoc.Value);
            }

            // Lọc theo Hình thức (Trực tiếp, Trực tuyến, Kết hợp)
            if (!string.IsNullOrWhiteSpace(filter.HinhThuc))
            {
                query = query.Where(k => k.HinhThuc == filter.HinhThuc);
            }

            // Lọc theo Trạng thái (SapMo, DangMo, DangHoc, DaKetThuc, BiHuy)
            if (!string.IsNullOrWhiteSpace(filter.TrangThai))
            {
                query = query.Where(k => k.TrangThai == filter.TrangThai);
            }

            // Lọc theo Khoảng học phí
            if (!string.IsNullOrWhiteSpace(filter.KhoangHocPhi))
            {
                query = filter.KhoangHocPhi switch
                {
                    "duoi_2tr" => query.Where(k => k.HocPhi < 2000000),
                    "2tr_4tr" => query.Where(k => k.HocPhi >= 2000000 && k.HocPhi <= 4000000),
                    "4tr_6tr" => query.Where(k => k.HocPhi > 4000000 && k.HocPhi <= 6000000),
                    "tren_6tr" => query.Where(k => k.HocPhi > 6000000),
                    _ => query
                };
            }

            // Lọc theo Còn chỗ / Hết chỗ (Số lượng còn lại = SoLuongToiDa - SoLuongDaDangKy)
            if (!string.IsNullOrWhiteSpace(filter.TinhTrangCho))
            {
                if (filter.TinhTrangCho == "con_cho")
                {
                    query = query.Where(k => k.SoLuongToiDa > k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"));
                }
                else if (filter.TinhTrangCho == "het_cho")
                {
                    query = query.Where(k => k.SoLuongToiDa <= k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"));
                }
            }

            // -------------------------------------------------------------
            // BƯỚC 3: SẮP XẾP LINQ (Mục 6.5)
            // • Tên khóa học A -> Z
            // • Tên khóa học Z -> A
            // • Ngày bắt đầu tăng / giảm
            // • Học phí tăng / giảm
            // • Số lượng còn lại tăng / giảm
            // -------------------------------------------------------------
            query = filter.SortBy switch
            {
                "ten_asc" => query.OrderBy(k => k.TenKhoaHoc),
                "ten_desc" => query.OrderByDescending(k => k.TenKhoaHoc),
                "ngay_asc" => query.OrderBy(k => k.NgayBatDau).ThenBy(k => k.TenKhoaHoc),
                "ngay_desc" => query.OrderByDescending(k => k.NgayBatDau).ThenBy(k => k.TenKhoaHoc),
                "hocphi_asc" => query.OrderBy(k => k.HocPhi).ThenBy(k => k.TenKhoaHoc),
                "hocphi_desc" => query.OrderByDescending(k => k.HocPhi).ThenBy(k => k.TenKhoaHoc),
                "cho_asc" => query.OrderBy(k => (k.SoLuongToiDa - k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"))).ThenBy(k => k.TenKhoaHoc),
                "cho_desc" => query.OrderByDescending(k => (k.SoLuongToiDa - k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"))).ThenBy(k => k.TenKhoaHoc),
                _ => query.OrderByDescending(k => k.MaKhoaHoc)
            };

            // -------------------------------------------------------------
            // BƯỚC 4: PHÂN TRANG BẰNG EF CORE Skip() & Take() (Mục 6.6)
            // -------------------------------------------------------------
            filter.TotalItems = await query.CountAsync();

            if (filter.PageIndex < 1) filter.PageIndex = 1;
            if (filter.PageSize < 1) filter.PageSize = 5;

            var rawList = await query
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            // Ánh xạ sang ViewModel để tính toán nghiệp vụ khả dụng (Mục 6.7)
            filter.DanhSachKhoaHoc = rawList.Select(k => new KhoaHocItemViewModel
            {
                MaKhoaHoc = k.MaKhoaHoc,
                TenKhoaHoc = k.TenKhoaHoc,
                MaMonHoc = k.MaMonHoc,
                TenMonHoc = k.MonHoc?.TenMonHoc ?? "Chưa xác định",
                SoTinChi = k.MonHoc?.SoTinChi ?? 0,
                MaGiangVien = k.MaGiangVien,
                TenGiangVien = k.GiangVien?.HoTen ?? "Chưa phân công",
                HocVi = k.GiangVien?.HocVi,
                ChuyenMon = k.GiangVien?.ChuyenMon,
                NgayBatDau = k.NgayBatDau,
                NgayKetThuc = k.NgayKetThuc,
                SoLuongToiDa = k.SoLuongToiDa,
                SoLuongDaDangKy = k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"),
                HocPhi = k.HocPhi,
                HinhThuc = k.HinhThuc,
                TrangThai = k.TrangThai,
                MoTa = k.MoTa
            }).ToList();

            // Phản hồi AJAX nếu có yêu cầu partial
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_KhoaHocGrid", filter);
            }

            return View(filter);
        }

        // =========================================================================
        // 2. TRANG QUẢN TRỊ KHÓA HỌC (ADMIN & NHÂN VIÊN)
        // Hỗ trợ tìm kiếm, lọc, sắp xếp, phân trang Skip/Take
        // =========================================================================

        // GET: /KhoaHoc/QuanLy
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> QuanLy(
            string? searchString,
            int? maMonHoc,
            string? trangThai,
            string? hinhThuc,
            string? khoangHocPhi,
            string? tinhTrangCho,
            string? sortBy = "id_desc",
            int page = 1,
            int pageSize = 5)
        {
            // Dropdown môn học cho bộ lọc quản trị
            ViewBag.MonHocList = new SelectList(
                await _context.MonHocs
                    .Where(m => m.TrangThai)
                    .OrderBy(m => m.TenMonHoc)
                    .Select(m => new { m.MaMonHoc, Display = m.TenMonHoc + " (" + m.SoTinChi + " TC)" })
                    .ToListAsync(),
                "MaMonHoc", "Display", maMonHoc);

            // Thống kê số liệu tổng thể phục vụ 4 thẻ Stat Cards
            ViewBag.TotalCount = await _context.KhoaHocs.CountAsync();
            ViewBag.DangMoCount = await _context.KhoaHocs.CountAsync(k => k.TrangThai == "DangMo");
            ViewBag.DangHocCount = await _context.KhoaHocs.CountAsync(k => k.TrangThai == "DangHoc");
            ViewBag.DaKetThucCount = await _context.KhoaHocs.CountAsync(k => k.TrangThai == "DaKetThuc");
            ViewBag.AvgHocPhi = await _context.KhoaHocs.AnyAsync()
                ? (long)await _context.KhoaHocs.AverageAsync(k => (double)k.HocPhi)
                : 0;

            var query = _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.GiangVien)
                .Include(k => k.DangKyKhoaHocs)
                .AsNoTracking()
                .AsQueryable();

            // 6.3 Tìm kiếm đa tiêu chí: Tên khóa học, Tên môn học, Tên giảng viên
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var keyword = searchString.Trim().ToLower();
                query = query.Where(k =>
                    k.TenKhoaHoc.ToLower().Contains(keyword) ||
                    (k.MonHoc != null && k.MonHoc.TenMonHoc.ToLower().Contains(keyword)) ||
                    (k.GiangVien != null && k.GiangVien.HoTen.ToLower().Contains(keyword))
                );
                ViewBag.CurrentSearch = searchString;
            }

            // 6.4 Lọc môn học
            if (maMonHoc.HasValue && maMonHoc.Value > 0)
            {
                query = query.Where(k => k.MaMonHoc == maMonHoc.Value);
                ViewBag.CurrentMaMonHoc = maMonHoc.Value;
            }

            // 6.4 Lọc trạng thái
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(k => k.TrangThai == trangThai);
                ViewBag.CurrentTrangThai = trangThai;
            }

            // 6.4 Lọc hình thức đào tạo
            if (!string.IsNullOrWhiteSpace(hinhThuc))
            {
                query = query.Where(k => k.HinhThuc == hinhThuc);
                ViewBag.CurrentHinhThuc = hinhThuc;
            }

            // 6.4 Lọc khoảng học phí
            if (!string.IsNullOrWhiteSpace(khoangHocPhi))
            {
                query = khoangHocPhi switch
                {
                    "duoi_2tr" => query.Where(k => k.HocPhi < 2000000),
                    "2tr_4tr" => query.Where(k => k.HocPhi >= 2000000 && k.HocPhi <= 4000000),
                    "4tr_6tr" => query.Where(k => k.HocPhi > 4000000 && k.HocPhi <= 6000000),
                    "tren_6tr" => query.Where(k => k.HocPhi > 6000000),
                    _ => query
                };
                ViewBag.CurrentKhoangHocPhi = khoangHocPhi;
            }

            // 6.4 Lọc còn chỗ / hết chỗ
            if (!string.IsNullOrWhiteSpace(tinhTrangCho))
            {
                if (tinhTrangCho == "con_cho")
                {
                    query = query.Where(k => k.SoLuongToiDa > k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"));
                }
                else if (tinhTrangCho == "het_cho")
                {
                    query = query.Where(k => k.SoLuongToiDa <= k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"));
                }
                ViewBag.CurrentTinhTrangCho = tinhTrangCho;
            }

            // 6.5 Sắp xếp
            ViewBag.CurrentSortBy = sortBy;
            query = sortBy switch
            {
                "ten_asc" => query.OrderBy(k => k.TenKhoaHoc),
                "ten_desc" => query.OrderByDescending(k => k.TenKhoaHoc),
                "ngay_asc" => query.OrderBy(k => k.NgayBatDau).ThenBy(k => k.TenKhoaHoc),
                "ngay_desc" => query.OrderByDescending(k => k.NgayBatDau).ThenBy(k => k.TenKhoaHoc),
                "hocphi_asc" => query.OrderBy(k => k.HocPhi).ThenBy(k => k.TenKhoaHoc),
                "hocphi_desc" => query.OrderByDescending(k => k.HocPhi).ThenBy(k => k.TenKhoaHoc),
                "cho_asc" => query.OrderBy(k => (k.SoLuongToiDa - k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"))).ThenBy(k => k.TenKhoaHoc),
                "cho_desc" => query.OrderByDescending(k => (k.SoLuongToiDa - k.DangKyKhoaHocs.Count(dk => dk.TrangThai != "BiHuy"))).ThenBy(k => k.TenKhoaHoc),
                "id_asc" => query.OrderBy(k => k.MaKhoaHoc),
                _ => query.OrderByDescending(k => k.MaKhoaHoc)
            };

            // 6.6 Phân trang Skip(), Take()
            int totalFiltered = await query.CountAsync();
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;
            int totalPages = (int)Math.Ceiling(totalFiltered / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages) page = totalPages;

            ViewBag.PageIndex = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalFiltered;
            ViewBag.TotalPages = totalPages;

            var list = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return View(list);
        }

        // GET: /KhoaHoc/Details/5
        // Quyền: Admin và Nhân viên
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.GiangVien)
                .Include(k => k.DangKyKhoaHocs)
                .FirstOrDefaultAsync(k => k.MaKhoaHoc == id);

            if (khoaHoc == null) return NotFound();

            return View(khoaHoc);
        }

        // GET: /KhoaHoc/Create
        // Quyền: Admin và Nhân viên
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> Create()
        {
            await LoadDropdownDataAsync();
            return View(new KhoaHoc
            {
                NgayBatDau = DateTime.Today,
                NgayKetThuc = DateTime.Today.AddMonths(3),
                SoLuongToiDa = 30,
                HocPhi = 3000000,
                HinhThuc = "Trực tiếp",
                TrangThai = "DangMo"
            });
        }

        // POST: /KhoaHoc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> Create(KhoaHoc khoaHoc)
        {
            // Kiểm tra nghiệp vụ: Ngày kết thúc phải sau ngày bắt đầu
            if (khoaHoc.NgayKetThuc <= khoaHoc.NgayBatDau)
            {
                ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải sau ngày bắt đầu khóa học.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(khoaHoc);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã thêm mới khóa học '{khoaHoc.TenKhoaHoc}' thành công!";
                    return RedirectToAction(nameof(QuanLy));
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Không thể lưu khóa học do lỗi kết nối CSDL. Vui lòng thử lại sau.");
                }
            }

            await LoadDropdownDataAsync(khoaHoc.MaMonHoc, khoaHoc.MaGiangVien);
            return View(khoaHoc);
        }

        // GET: /KhoaHoc/Edit/5
        // Quyền: Admin và Nhân viên
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var khoaHoc = await _context.KhoaHocs.FindAsync(id);
            if (khoaHoc == null) return NotFound();

            await LoadDropdownDataAsync(khoaHoc.MaMonHoc, khoaHoc.MaGiangVien);
            return View(khoaHoc);
        }

        // POST: /KhoaHoc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> Edit(int id, KhoaHoc khoaHoc)
        {
            if (id != khoaHoc.MaKhoaHoc) return NotFound();

            // Kiểm tra nghiệp vụ: Ngày kết thúc phải sau ngày bắt đầu
            if (khoaHoc.NgayKetThuc <= khoaHoc.NgayBatDau)
            {
                ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải sau ngày bắt đầu khóa học.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khoaHoc);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Cập nhật khóa học '{khoaHoc.TenKhoaHoc}' thành công!";
                    return RedirectToAction(nameof(QuanLy));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.KhoaHocs.AnyAsync(e => e.MaKhoaHoc == khoaHoc.MaKhoaHoc))
                    {
                        return NotFound();
                    }
                    throw;
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Không thể cập nhật khóa học do sự cố CSDL. Vui lòng thử lại.");
                }
            }

            await LoadDropdownDataAsync(khoaHoc.MaMonHoc, khoaHoc.MaGiangVien);
            return View(khoaHoc);
        }

        // GET: /KhoaHoc/Delete/5
        // Quyền: Chỉ Admin
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.GiangVien)
                .Include(k => k.DangKyKhoaHocs)
                .FirstOrDefaultAsync(k => k.MaKhoaHoc == id);

            if (khoaHoc == null) return NotFound();

            // Kiểm tra xem khóa học đã có đăng ký học viên chưa
            ViewBag.HasDangKy = khoaHoc.DangKyKhoaHocs.Any();
            return View(khoaHoc);
        }

        // POST: /KhoaHoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // BẢO VỆ TOÀN VẸN DỮ LIỆU BẮT BUỘC: Kiểm tra trước khi xóa
                bool daCoDangKy = await _context.DangKyKhoaHocs.AnyAsync(dk => dk.MaKhoaHoc == id);

                if (daCoDangKy)
                {
                    TempData["ErrorMessage"] = "Không thể xóa khóa học này vì đã có học viên đăng ký. Bạn có thể chuyển trạng thái sang 'Đã hủy'!";
                    return RedirectToAction(nameof(QuanLy));
                }

                var khoaHoc = await _context.KhoaHocs.FindAsync(id);
                if (khoaHoc != null)
                {
                    _context.KhoaHocs.Remove(khoaHoc);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã xóa khóa học '{khoaHoc.TenKhoaHoc}' thành công.";
                }
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Không thể xóa khóa học này vì có dữ liệu liên kết khác trong hệ thống.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Đã xảy ra sự cố khi xóa khóa học. Vui lòng thử lại sau.";
            }

            return RedirectToAction(nameof(QuanLy));
        }

        // POST: /KhoaHoc/UpdateStatus/5
        // Cập nhật trạng thái khóa học: SapMo -> DangMo -> DangHoc -> DaKetThuc / BiHuy
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin", "NhanVien")]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            try
            {
                var khoaHoc = await _context.KhoaHocs.FindAsync(id);
                if (khoaHoc != null)
                {
                    var validStatuses = new[] { "SapMo", "DangMo", "DangHoc", "DaKetThuc", "BiHuy" };
                    if (!validStatuses.Contains(newStatus))
                    {
                        TempData["ErrorMessage"] = "Trạng thái không hợp lệ.";
                        return RedirectToAction(nameof(QuanLy));
                    }

                    khoaHoc.TrangThai = newStatus;
                    await _context.SaveChangesAsync();

                    var statusLabel = GetStatusLabel(newStatus);
                    TempData["SuccessMessage"] = $"Đã chuyển trạng thái khóa học '{khoaHoc.TenKhoaHoc}' sang '{statusLabel}'.";
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Không thể cập nhật trạng thái khóa học do sự cố CSDL.";
            }
            return RedirectToAction(nameof(QuanLy));
        }

        // =========================================================================
        // HELPER METHODS
        // =========================================================================

        /// <summary>
        /// Nạp danh sách Môn học và Giảng viên vào SelectList cho dropdown
        /// Khóa ngoại phải sử dụng select lấy dữ liệu từ Database
        /// </summary>
        private async Task LoadDropdownDataAsync(int? selectedMonHoc = null, int? selectedGiangVien = null)
        {
            // Chỉ lấy Môn học đang mở giảng dạy
            ViewBag.MaMonHoc = new SelectList(
                await _context.MonHocs
                    .Where(m => m.TrangThai)
                    .OrderBy(m => m.TenMonHoc)
                    .Select(m => new { m.MaMonHoc, Display = m.TenMonHoc + " (" + m.SoTinChi + " TC)" })
                    .ToListAsync(),
                "MaMonHoc", "Display", selectedMonHoc);

            // Chỉ lấy Giảng viên đang công tác
            ViewBag.MaGiangVien = new SelectList(
                await _context.GiangViens
                    .Where(g => g.TrangThai)
                    .OrderBy(g => g.HoTen)
                    .Select(g => new { g.MaGiangVien, Display = g.HoTen + " - " + (g.ChuyenMon ?? "N/A") })
                    .ToListAsync(),
                "MaGiangVien", "Display", selectedGiangVien);
        }

        /// <summary>
        /// Lấy nhãn hiển thị cho trạng thái khóa học
        /// </summary>
        public static string GetStatusLabel(string trangThai)
        {
            return trangThai switch
            {
                "SapMo" => "Sắp mở",
                "DangMo" => "Đang mở đăng ký",
                "DangHoc" => "Đang giảng dạy",
                "DaKetThuc" => "Đã kết thúc",
                "BiHuy" => "Đã hủy",
                _ => trangThai
            };
        }
    }
}
