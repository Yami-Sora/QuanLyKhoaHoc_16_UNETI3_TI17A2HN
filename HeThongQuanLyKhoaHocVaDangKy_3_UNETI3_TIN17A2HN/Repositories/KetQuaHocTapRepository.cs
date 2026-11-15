// Họ và tên: Nguyễn Công Thành
// Mã sinh viên: 23103100102
// Nội dung thực hiện: Module 5 - Triển khai KetQuaHocTapRepository (Repository Pattern)

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.ViewModels;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories
{
    public class KetQuaHocTapRepository : IKetQuaHocTapRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<KetQuaHocTapRepository> _logger;

        public KetQuaHocTapRepository(ApplicationDbContext context, ILogger<KetQuaHocTapRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 1. Danh sách khóa học để nhập điểm (Dành cho Admin & Nhân viên)
        public async Task<(KetQuaFilterViewModel Filter, SelectList MonHocList)> GetKetQuaFilterAsync(KetQuaFilterViewModel filter)
        {
            if (filter.PageIndex < 1) filter.PageIndex = 1;
            if (filter.PageSize < 1) filter.PageSize = 10;

            // Dropdown môn học cho bộ lọc
            var monHocList = new SelectList(
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
                var dsCoKetQua = dsHoanThanh
                    .Where(d => d.KetQuaHocTap != null 
                             && d.KetQuaHocTap.DiemChuyenCan.HasValue 
                             && d.KetQuaHocTap.DiemGiuaKy.HasValue 
                             && d.KetQuaHocTap.DiemCuoiKy.HasValue 
                             && d.KetQuaHocTap.DiemTongKet.HasValue)
                    .ToList();
                var countCoKetQua = dsCoKetQua.Count;

                decimal? diemTB = dsCoKetQua.Any(d => d.KetQuaHocTap!.DiemTongKet.HasValue)
                    ? Math.Round(dsCoKetQua.Average(d => d.KetQuaHocTap!.DiemTongKet!.Value), 2)
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

            return (filter, monHocList);
        }

        // 2. Bảng điểm chi tiết & Form nhập điểm hàng loạt (Admin & Nhân viên)
        public async Task<BangDiemKhoaHocViewModel?> GetBangDiemAsync(int maKhoaHoc)
        {
            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.GiangVien)
                .FirstOrDefaultAsync(k => k.MaKhoaHoc == maKhoaHoc);

            if (khoaHoc == null)
            {
                return null;
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
                    DiemChuyenCan = kq?.DiemChuyenCan,
                    DiemGiuaKy = kq?.DiemGiuaKy,
                    DiemCuoiKy = kq?.DiemCuoiKy,
                    DiemTongKet = kq?.DiemTongKet,
                    XepLoai = kq?.XepLoai,
                    KetQua = kq?.KetQua
                };
            }).ToList();

            // Chỉ coi là đã có kết quả khi đã nhập đủ cả 3 đầu điểm
            var daCoKetQua = danhSachDiem.Where(d => d.DiemChuyenCan.HasValue && d.DiemGiuaKy.HasValue && d.DiemCuoiKy.HasValue && d.DaCoKetQua).ToList();
            var soDat = daCoKetQua.Count(d => d.KetQua == "Dat");
            var soKhongDat = daCoKetQua.Count(d => d.KetQua == "KhongDat");
            decimal? diemTB = daCoKetQua.Any(d => d.DiemTongKet.HasValue)
                ? Math.Round(daCoKetQua.Where(d => d.DiemTongKet.HasValue).Average(d => d.DiemTongKet!.Value), 2)
                : null;

            return new BangDiemKhoaHocViewModel
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
        }

        // 3. Lưu bảng điểm hàng loạt (POST)
        public async Task<(bool Success, int CountSaved, string? ErrorMessage)> LuuDiemAsync(BangDiemKhoaHocViewModel model)
        {
            if (model.DanhSachDiem == null || !model.DanhSachDiem.Any())
            {
                return (false, 0, "Không có dữ liệu học viên để lưu điểm.");
            }

            try
            {
                int countSaved = 0;

                foreach (var item in model.DanhSachDiem)
                {
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

                    // Kiểm tra tình trạng nhập điểm: có ít nhất 1 đầu điểm hay đã đủ cả 3
                    bool coDiemNao = item.DiemChuyenCan.HasValue || item.DiemGiuaKy.HasValue || item.DiemCuoiKy.HasValue;
                    bool hasAll3 = item.DiemChuyenCan.HasValue && item.DiemGiuaKy.HasValue && item.DiemCuoiKy.HasValue;

                    // Nếu cả 3 ô đều trống
                    if (!coDiemNao)
                    {
                        // Nếu trước đó đã có bản ghi trong DB mà giờ xóa bớt/để trống toàn bộ -> Xóa bản ghi điểm
                        if (ketQuaDb != null)
                        {
                            _context.KetQuaHocTaps.Remove(ketQuaDb);
                            countSaved++;
                        }
                        continue;
                    }

                    // Có ít nhất 1 đầu điểm: hỗ trợ lưu theo từng đợt
                    decimal? cc = item.DiemChuyenCan.HasValue ? NormalizeDiem(item.DiemChuyenCan.Value) : null;
                    decimal? gk = item.DiemGiuaKy.HasValue ? NormalizeDiem(item.DiemGiuaKy.Value) : null;
                    decimal? ck = item.DiemCuoiKy.HasValue ? NormalizeDiem(item.DiemCuoiKy.Value) : null;

                    decimal? tongKet = null;
                    string? xepLoai = null;
                    string? ketQua = null;

                    // Chỉ khi nhập ĐỦ CẢ 3 ĐẦU ĐIỂM mới tính điểm tổng kết & xếp loại
                    if (hasAll3)
                    {
                        tongKet = Math.Round(cc!.Value * 0.1m + gk!.Value * 0.4m + ck!.Value * 0.5m, 2);
                        xepLoai = tongKet switch
                        {
                            >= 9.0m => "XuatSac",
                            >= 8.0m => "Gioi",
                            >= 6.5m => "Kha",
                            >= 5.0m => "TrungBinh",
                            _ => "Yeu"
                        };
                        ketQua = tongKet >= 5.0m ? "Dat" : "KhongDat";
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
                return (true, countSaved, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lưu bảng điểm vào cơ sở dữ liệu");
                return (false, 0, $"Lỗi khi lưu bảng điểm vào cơ sở dữ liệu: {ex.Message}");
            }
        }

        // 4. Kết quả học tập của tôi (Dành riêng cho Học viên xem điểm cá nhân)
        public async Task<KetQuaCuaToiViewModel?> GetKetQuaCuaToiAsync(int maTaiKhoan, string? vaiTro)
        {
            // Tìm học viên tương ứng với tài khoản đang đăng nhập
            var hocVien = await _context.HocViens
                .FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);

            // Trường hợp tài khoản Admin/Nhân viên vào xem thử thì lấy học viên đầu tiên để demo
            if (hocVien == null)
            {
                if (vaiTro == "Admin" || vaiTro == "NhanVien")
                {
                    hocVien = await _context.HocViens.FirstOrDefaultAsync();
                }

                if (hocVien == null)
                {
                    return null;
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
                bool daDuDiem = kq != null 
                    && kq.DiemChuyenCan.HasValue 
                    && kq.DiemGiuaKy.HasValue 
                    && kq.DiemCuoiKy.HasValue 
                    && kq.DiemTongKet.HasValue;

                return new KetQuaCuaToiItem
                {
                    MaDangKy = d.MaDangKy,
                    MaKhoaHoc = d.MaKhoaHoc,
                    TenKhoaHoc = d.KhoaHoc?.TenKhoaHoc ?? $"Khóa #{d.MaKhoaHoc}",
                    TenMonHoc = d.KhoaHoc?.MonHoc?.TenMonHoc,
                    SoTinChi = d.KhoaHoc?.MonHoc?.SoTinChi ?? 0,
                    TenGiangVien = d.KhoaHoc?.GiangVien?.HoTen,
                    HinhThucHoc = d.KhoaHoc?.HinhThuc,
                    DaCoKetQua = daDuDiem,
                    DiemChuyenCan = kq?.DiemChuyenCan,
                    DiemGiuaKy = kq?.DiemGiuaKy,
                    DiemCuoiKy = kq?.DiemCuoiKy,
                    DiemTongKet = kq?.DiemTongKet,
                    XepLoai = kq?.XepLoai,
                    KetQua = kq?.KetQua,
                    NgayCapNhat = kq?.NgayCapNhat
                };
            }).ToList();

            var dsCoKetQua = dsItems.Where(i => i.DaCoKetQua && i.DiemTongKet.HasValue).ToList();
            var soDat = dsCoKetQua.Count(i => i.KetQua == "Dat");
            var soKhongDat = dsCoKetQua.Count(i => i.KetQua == "KhongDat");
            decimal? dtb = dsCoKetQua.Any()
                ? Math.Round(dsCoKetQua.Average(i => i.DiemTongKet!.Value), 2)
                : null;

            var tongTinChi = dsItems.Sum(i => i.SoTinChi);
            var tinChiDat = dsItems.Where(i => i.KetQua == "Dat").Sum(i => i.SoTinChi);

            return new KetQuaCuaToiViewModel
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
        }

        // Chuẩn hóa điểm: Nếu điểm bị lỗi culture parse làm mất dấu chấm (ví dụ 8.5 thành 85) thì tự chia 10
        private static decimal NormalizeDiem(decimal raw)
        {
            if (raw > 10m && raw <= 100m)
            {
                raw = raw / 10m;
            }
            return Math.Clamp(Math.Round(raw, 1), 0m, 10m);
        }
    }
}
