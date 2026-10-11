// Họ và tên: Nguyễn Công Thành
// Mã sinh viên: 23103100102
// Nội dung thực hiện: Module 5 - Quản lý kết quả học tập, bảng điểm và nhập điểm hàng loạt

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Filters;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Controllers
{
    public class KetQuaHocTapController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KetQuaHocTapController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================================
        // 1. DANH SÁCH KHÓA HỌC ĐỂ NHẬP ĐIỂM (Dành cho Admin & Nhân viên)
        // =========================================================================
        [AuthorizeRole("Admin", "NhanVien")]
        [HttpGet]
        public async Task<IActionResult> Index(KetQuaFilterViewModel filter)
        {
            if (filter.PageIndex < 1) filter.PageIndex = 1;
            if (filter.PageSize < 1) filter.PageSize = 10;

            // Dropdown môn học cho bộ lọc
            ViewBag.MonHocList = new SelectList(
                await _context.MonHocs
                    .OrderBy(m => m.TenMonHoc)
                    .Select(m => new { m.MaMonHoc, Display = m.TenMonHoc + " (" + m.SoTinChi + " TC)" })
                    .ToListAsync(),
                "MaMonHoc", "Display", filter.MaMonHoc);

            // Query cơ sở khóa học kèm dữ liệu liên quan
            var query = _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.GiangVien)
                .Include(k => k.DangKyKhoaHocs)
                    .ThenInclude(d => d.KetQuaHocTap)
                .AsQueryable();

            // 1. Lọc theo từ khóa tìm kiếm (tên khóa, môn học, giảng viên)
            if (!string.IsNullOrWhiteSpace(filter.SearchString))
            {
                var keyword = filter.SearchString.Trim();
                query = query.Where(k =>
                    k.TenKhoaHoc.Contains(keyword) ||
                    (k.MonHoc != null && k.MonHoc.TenMonHoc.Contains(keyword)) ||
                    (k.GiangVien != null && k.GiangVien.HoTen.Contains(keyword)));
            }

            // 2. Lọc theo môn học
            if (filter.MaMonHoc.HasValue)
            {
                query = query.Where(k => k.MaMonHoc == filter.MaMonHoc.Value);
            }

            // 3. Lọc theo trạng thái khóa học
            if (!string.IsNullOrWhiteSpace(filter.TrangThai) && filter.TrangThai != "TatCa")
            {
                query = query.Where(k => k.TrangThai == filter.TrangThai);
            }

            // Lấy danh sách entity để tính toán tiến độ điểm chính xác trong bộ nhớ
            var allFiltered = await query.ToListAsync();

            var mappedList = allFiltered.Select(k =>
            {
                var dsHoanThanh = k.DangKyKhoaHocs.Where(d => d.TrangThai == "HoanThanh").ToList();
                var countHoanThanh = dsHoanThanh.Count;
                var dsCoKetQua = dsHoanThanh.Where(d => d.KetQuaHocTap != null).ToList();
                var countCoKetQua = dsCoKetQua.Count;

                decimal? diemTB = dsCoKetQua.Any()
                    ? Math.Round(dsCoKetQua.Average(d => d.KetQuaHocTap!.DiemTongKet), 2)
                    : null;

                return new KhoaHocKetQuaViewModel
                {
                    MaKhoaHoc = k.MaKhoaHoc,
                    TenKhoaHoc = k.TenKhoaHoc,
                    TenMonHoc = k.MonHoc?.TenMonHoc,
                    TenGiangVien = k.GiangVien?.HoTen,
                    TrangThai = k.TrangThai,
                    HinhThuc = k.HinhThuc,
                    NgayBatDau = k.NgayBatDau,
                    NgayKetThuc = k.NgayKetThuc,
                    SoDangKyHoanThanh = countHoanThanh,
                    SoDaCoKetQua = countCoKetQua,
                    DiemTBLop = diemTB
                };
            }).AsQueryable();

            // 4. Lọc theo tình trạng điểm
            if (!string.IsNullOrWhiteSpace(filter.TinhTrangDiem) && filter.TinhTrangDiem != "TatCa")
            {
                mappedList = filter.TinhTrangDiem switch
                {
                    "DaNhapDiem" => mappedList.Where(k => k.SoDangKyHoanThanh > 0 && k.SoDaCoKetQua == k.SoDangKyHoanThanh),
                    "ChuaNhapDiem" => mappedList.Where(k => k.SoDangKyHoanThanh > 0 && k.SoDaCoKetQua == 0),
                    "NhapMotPhan" => mappedList.Where(k => k.SoDangKyHoanThanh > 0 && k.SoDaCoKetQua > 0 && k.SoDaCoKetQua < k.SoDangKyHoanThanh),
                    "ChuaCoHocVien" => mappedList.Where(k => k.SoDangKyHoanThanh == 0),
                    _ => mappedList
                };
            }

            // 5. Sắp xếp (SortBy)
            mappedList = (filter.SortBy ?? "ten_asc") switch
            {
                "ten_desc" => mappedList.OrderByDescending(k => k.TenKhoaHoc),
                "ngay_desc" => mappedList.OrderByDescending(k => k.NgayBatDau),
                "ngay_asc" => mappedList.OrderBy(k => k.NgayBatDau),
                "tiendo_desc" => mappedList.OrderByDescending(k => k.TienDoNhapDiem),
                "diem_desc" => mappedList.OrderByDescending(k => k.DiemTBLop ?? -1),
                _ => mappedList.OrderBy(k => k.TenKhoaHoc)
            };

            var finalResults = mappedList.ToList();

            // Thống kê Stat Cards tổng quan
            filter.TotalItems = finalResults.Count;
            filter.TongSoKhoaHoc = await _context.KhoaHocs.CountAsync();
            filter.SoKhoaHocDaNhapDiem = finalResults.Count(k => k.SoDangKyHoanThanh > 0 && k.SoDaCoKetQua == k.SoDangKyHoanThanh);
            filter.SoKhoaHocChuaNhapDiem = finalResults.Count(k => k.SoDangKyHoanThanh > 0 && k.SoDaCoKetQua == 0);
            filter.TongSoHocVienCanNhapDiem = await _context.DangKyKhoaHocs
                .CountAsync(d => d.TrangThai == "HoanThanh" && d.KetQuaHocTap == null);

            // Phân trang
            filter.DanhSachKhoaHoc = finalResults
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            return View(filter);
        }

        // =========================================================================
        // 2. BẢNG ĐIỂM CHI TIẾT & FORM NHẬP ĐIỂM HÀNG LOẠT (Admin & Nhân viên)
        // =========================================================================
        [AuthorizeRole("Admin", "NhanVien")]
        [HttpGet]
        public async Task<IActionResult> BangDiem(int maKhoaHoc)
        {
            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.GiangVien)
                .FirstOrDefaultAsync(k => k.MaKhoaHoc == maKhoaHoc);

            if (khoaHoc == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin khóa học yêu cầu.";
                return RedirectToAction(nameof(Index));
            }

            // Chỉ lấy các học viên đã hoàn thành thủ tục đăng ký học (TrangThai == "HoanThanh")
            var dsDangKy = await _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .Include(d => d.KetQuaHocTap)
                .Where(d => d.MaKhoaHoc == maKhoaHoc && d.TrangThai == "HoanThanh")
                .OrderBy(d => d.HocVien != null ? d.HocVien.HoTen : string.Empty)
                .ToListAsync();

            var danhSachDiem = dsDangKy.Select(d =>
            {
                var kq = d.KetQuaHocTap;
                return new NhapDiemHocVienItem
                {
                    MaDangKy = d.MaDangKy,
                    MaHocVien = d.MaHocVien,
                    HoTenHocVien = d.HocVien?.HoTen ?? $"HV #{d.MaHocVien}",
                    EmailHocVien = d.HocVien?.Email,
                    MaKetQua = kq?.MaKetQua,
                    DiemChuyenCan = kq?.DiemChuyenCan ?? 0,
                    DiemGiuaKy = kq?.DiemGiuaKy ?? 0,
                    DiemCuoiKy = kq?.DiemCuoiKy ?? 0,
                    DiemTongKet = kq?.DiemTongKet ?? 0,
                    XepLoai = kq?.XepLoai ?? "Chưa xếp loại",
                    KetQua = kq?.KetQua ?? "Chưa đánh giá"
                };
            }).ToList();

            var daCoKetQua = danhSachDiem.Where(d => d.DaCoKetQua).ToList();
            var soDat = daCoKetQua.Count(d => d.KetQua == "Dat");
            var soKhongDat = daCoKetQua.Count(d => d.KetQua == "KhongDat");
            decimal? diemTB = daCoKetQua.Any()
                ? Math.Round(daCoKetQua.Average(d => d.DiemTongKet), 2)
                : null;

            var viewModel = new BangDiemKhoaHocViewModel
            {
                MaKhoaHoc = khoaHoc.MaKhoaHoc,
                TenKhoaHoc = khoaHoc.TenKhoaHoc,
                TenMonHoc = khoaHoc.MonHoc?.TenMonHoc,
                SoTinChi = khoaHoc.MonHoc?.SoTinChi ?? 0,
                TenGiangVien = khoaHoc.GiangVien?.HoTen,
                TrangThaiKhoaHoc = khoaHoc.TrangThai,
                TongSoDangKyHoanThanh = dsDangKy.Count,
                SoDaCoKetQua = daCoKetQua.Count,
                SoChuaCoKetQua = dsDangKy.Count - daCoKetQua.Count,
                DiemTBLop = diemTB,
                SoDat = soDat,
                SoKhongDat = soKhongDat,
                DanhSachDiem = danhSachDiem
            };

            return View(viewModel);
        }

        // =========================================================================
        // 3. LƯU BẢNG ĐIỂM HÀNG LOẠT (POST)
        // =========================================================================
        [AuthorizeRole("Admin", "NhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LuuDiem(BangDiemKhoaHocViewModel model)
        {
            if (model.DanhSachDiem == null || !model.DanhSachDiem.Any())
            {
                TempData["ErrorMessage"] = "Không có dữ liệu học viên để lưu điểm.";
                return RedirectToAction(nameof(BangDiem), new { maKhoaHoc = model.MaKhoaHoc });
            }

            try
            {
                int countSaved = 0;

                foreach (var item in model.DanhSachDiem)
                {
                    // Chuẩn hóa điểm trong khoảng [0, 10]
                    decimal cc = Math.Clamp(Math.Round(item.DiemChuyenCan, 1), 0m, 10m);
                    decimal gk = Math.Clamp(Math.Round(item.DiemGiuaKy, 1), 0m, 10m);
                    decimal ck = Math.Clamp(Math.Round(item.DiemCuoiKy, 1), 0m, 10m);

                    // Công thức chuẩn: CC * 0.1 + GK * 0.4 + CK * 0.5
                    decimal tongKet = Math.Round(cc * 0.1m + gk * 0.4m + ck * 0.5m, 2);

                    // Xếp loại học lực theo thang 10
                    string xepLoai = tongKet switch
                    {
                        >= 9.0m => "XuatSac",
                        >= 8.0m => "Gioi",
                        >= 6.5m => "Kha",
                        >= 5.0m => "TrungBinh",
                        _ => "Yeu"
                    };

                    // Kết quả: Đạt nếu ĐTK >= 5.0
                    string ketQua = tongKet >= 5.0m ? "Dat" : "KhongDat";

                    // Tìm bản ghi điểm hiện có
                    KetQuaHocTap? ketQuaDb = null;
                    if (item.MaKetQua.HasValue && item.MaKetQua.Value > 0)
                    {
                        ketQuaDb = await _context.KetQuaHocTaps.FindAsync(item.MaKetQua.Value);
                    }
                    else
                    {
                        ketQuaDb = await _context.KetQuaHocTaps.FirstOrDefaultAsync(k => k.MaDangKy == item.MaDangKy);
                    }

                    if (ketQuaDb != null)
                    {
                        // Cập nhật kết quả hiện có
                        ketQuaDb.DiemChuyenCan = cc;
                        ketQuaDb.DiemGiuaKy = gk;
                        ketQuaDb.DiemCuoiKy = ck;
                        ketQuaDb.DiemTongKet = tongKet;
                        ketQuaDb.XepLoai = xepLoai;
                        ketQuaDb.KetQua = ketQua;
                        ketQuaDb.NgayCapNhat = DateTime.Now;
                        _context.Update(ketQuaDb);
                    }
                    else
                    {
                        // Tạo mới kết quả học tập gắn với đơn đăng ký
                        var newKq = new KetQuaHocTap
                        {
                            MaDangKy = item.MaDangKy,
                            DiemChuyenCan = cc,
                            DiemGiuaKy = gk,
                            DiemCuoiKy = ck,
                            DiemTongKet = tongKet,
                            XepLoai = xepLoai,
                            KetQua = ketQua,
                            NgayCapNhat = DateTime.Now
                        };
                        _context.KetQuaHocTaps.Add(newKq);
                    }

                    countSaved++;
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Lưu bảng điểm thành công cho toàn bộ {countSaved} học viên!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi lưu bảng điểm vào cơ sở dữ liệu: {ex.Message}";
            }

            return RedirectToAction(nameof(BangDiem), new { maKhoaHoc = model.MaKhoaHoc });
        }

        // =========================================================================
        // 4. KẾT QUẢ HỌC TẬP CỦA TÔI (Dành riêng cho Học viên xem điểm cá nhân)
        // =========================================================================
        [AuthorizeRole("HocVien", "Admin", "NhanVien")]
        [HttpGet]
        public async Task<IActionResult> KetQuaCuaToi()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (!maTaiKhoan.HasValue)
            {
                return RedirectToAction("Login", "TaiKhoan");
            }

            // Tìm học viên tương ứng với tài khoản đang đăng nhập
            var hocVien = await _context.HocViens
                .FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan.Value);

            // Trường hợp tài khoản Admin/Nhân viên vào xem thử thì lấy học viên đầu tiên để demo
            if (hocVien == null)
            {
                var vaiTro = HttpContext.Session.GetString("VaiTro");
                if (vaiTro == "Admin" || vaiTro == "NhanVien")
                {
                    hocVien = await _context.HocViens.FirstOrDefaultAsync();
                }

                if (hocVien == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy hồ sơ học viên tương ứng với tài khoản này.";
                    return RedirectToAction("Index", "Home");
                }
            }

            // Lấy toàn bộ đơn đăng ký đã hoàn thành của học viên
            var dsDangKy = await _context.DangKyKhoaHocs
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.MonHoc)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.GiangVien)
                .Include(d => d.KetQuaHocTap)
                .Where(d => d.MaHocVien == hocVien.MaHocVien && d.TrangThai == "HoanThanh")
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();

            var dsItems = dsDangKy.Select(d =>
            {
                var kq = d.KetQuaHocTap;
                return new KetQuaCuaToiItem
                {
                    MaDangKy = d.MaDangKy,
                    MaKhoaHoc = d.MaKhoaHoc,
                    TenKhoaHoc = d.KhoaHoc?.TenKhoaHoc ?? $"Khóa #{d.MaKhoaHoc}",
                    TenMonHoc = d.KhoaHoc?.MonHoc?.TenMonHoc,
                    SoTinChi = d.KhoaHoc?.MonHoc?.SoTinChi ?? 0,
                    TenGiangVien = d.KhoaHoc?.GiangVien?.HoTen,
                    HinhThucHoc = d.KhoaHoc?.HinhThuc,
                    DaCoKetQua = kq != null,
                    DiemChuyenCan = kq?.DiemChuyenCan ?? 0,
                    DiemGiuaKy = kq?.DiemGiuaKy ?? 0,
                    DiemCuoiKy = kq?.DiemCuoiKy ?? 0,
                    DiemTongKet = kq?.DiemTongKet ?? 0,
                    XepLoai = kq?.XepLoai,
                    KetQua = kq?.KetQua,
                    NgayCapNhat = kq?.NgayCapNhat
                };
            }).ToList();

            var dsCoKetQua = dsItems.Where(i => i.DaCoKetQua).ToList();
            var soDat = dsCoKetQua.Count(i => i.KetQua == "Dat");
            var soKhongDat = dsCoKetQua.Count(i => i.KetQua == "KhongDat");
            decimal? dtb = dsCoKetQua.Any()
                ? Math.Round(dsCoKetQua.Average(i => i.DiemTongKet), 2)
                : null;

            var tongTinChi = dsItems.Sum(i => i.SoTinChi);
            var tinChiDat = dsItems.Where(i => i.KetQua == "Dat").Sum(i => i.SoTinChi);

            var viewModel = new KetQuaCuaToiViewModel
            {
                MaHocVien = hocVien.MaHocVien,
                HoTenHocVien = hocVien.HoTen,
                EmailHocVien = hocVien.Email,
                DanhSachKetQua = dsItems,
                DiemTBToanBo = dtb,
                TongSoKhoaDaHoc = dsItems.Count,
                SoKhoaDat = soDat,
                SoKhoaKhongDat = soKhongDat,
                SoKhoaChuaCoKetQua = dsItems.Count - dsCoKetQua.Count,
                TongTinChi = tongTinChi,
                TinChiDat = tinChiDat
            };

            return View(viewModel);
        }
    }
}
