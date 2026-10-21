// Họ và tên: Trần Văn Thành - MSV: 23103100076 (Module 1: Khởi tạo dữ liệu mẫu nền)
// Họ và tên: Nguyễn Công Thành - MSV: 23103100102 (Module 5: Dữ liệu mẫu giao dịch đăng ký & Kết quả học tập)

using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Helpers;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            // 1. Nạp môn học mẫu
            var monHocs = new List<MonHoc>
            {
                new MonHoc
                {
                    TenMonHoc = "Lập trình C# và .NET Core",
                    SoTinChi = 3,
                    HocPhi = 3500000,
                    TrangThai = true,
                    MoTa = "Kiến thức C# 13, ASP.NET Core MVC, Entity Framework Core và lập trình Web hiện đại."
                },
                new MonHoc
                {
                    TenMonHoc = "Cơ sở Dữ liệu & SQL Server",
                    SoTinChi = 3,
                    HocPhi = 3000000,
                    TrangThai = true,
                    MoTa = "Thiết kế CSDL quan hệ, T-SQL, Trigger, Store Procedure và tối ưu truy vấn."
                },
                new MonHoc
                {
                    TenMonHoc = "Lập trình Web Frontend với React",
                    SoTinChi = 3,
                    HocPhi = 3800000,
                    TrangThai = true,
                    MoTa = "Xây dựng giao diện tương tác Single Page Application với ReactJS, Redux Toolkit và Tailwind."
                },
                new MonHoc
                {
                    TenMonHoc = "Lập trình Di động với Flutter",
                    SoTinChi = 4,
                    HocPhi = 4200000,
                    TrangThai = true,
                    MoTa = "Phát triển ứng dụng di động đa nền tảng iOS & Android với Dart và Flutter Framework."
                },
                new MonHoc
                {
                    TenMonHoc = "Phân tích Dữ liệu với Python",
                    SoTinChi = 3,
                    HocPhi = 4000000,
                    TrangThai = false,
                    MoTa = "Xử lý dữ liệu lớn với Pandas, NumPy, Matplotlib và Machine Learning cơ bản (Tạm dừng tuyển sinh)."
                },
                new MonHoc
                {
                    TenMonHoc = "Lập trình hướng đối tượng",
                    SoTinChi = 3,
                    HocPhi = 3200000,
                    TrangThai = true,
                    MoTa = "Phương pháp lập trình hướng đối tượng với C++/Java, 4 đặc tính đóng gói, kế thừa, đa hình, trừu tượng và Design Patterns."
                },
                new MonHoc
                {
                    TenMonHoc = "Cấu trúc dữ liệu và giải thuật",
                    SoTinChi = 3,
                    HocPhi = 3200000,
                    TrangThai = true,
                    MoTa = "Cấu trúc dữ liệu tuyến tính và phi tuyến (Danh sách, Ngăn xếp, Hàng đợi, Cây, Đồ thị), các thuật toán sắp xếp và tìm kiếm nâng cao."
                },
                new MonHoc
                {
                    TenMonHoc = "Mạng máy tính",
                    SoTinChi = 3,
                    HocPhi = 3000000,
                    TrangThai = true,
                    MoTa = "Kiến trúc mạng máy tính, mô hình OSI và TCP/IP, cấu hình định tuyến Router, Switch, địa chỉ IP và bảo mật mạng cục bộ LAN/WAN."
                },
                new MonHoc
                {
                    TenMonHoc = "Hệ điều hành",
                    SoTinChi = 3,
                    HocPhi = 3000000,
                    TrangThai = true,
                    MoTa = "Nguyên lý hoạt động của hệ điều hành: Quản lý tiến trình (Process), lập lịch CPU, đồng bộ luồng, bộ nhớ ảo và tệp tin."
                },
                new MonHoc
                {
                    TenMonHoc = "An toàn thông tin",
                    SoTinChi = 3,
                    HocPhi = 3400000,
                    TrangThai = true,
                    MoTa = "Nguyên lý an ninh thông tin, mật mã hóa đối xứng/bất đối xứng, chữ ký số, chứng chỉ số và phòng ngừa các nguy cơ tấn công mạng."
                },
                new MonHoc
                {
                    TenMonHoc = "Công nghệ Java",
                    SoTinChi = 3,
                    HocPhi = 3600000,
                    TrangThai = true,
                    MoTa = "Phát triển ứng dụng doanh nghiệp với Java Core, lập trình đa luồng, Spring Boot Framework, Spring Data JPA và kiến trúc REST API."
                },
                new MonHoc
                {
                    TenMonHoc = "Phân tích và thiết kế các hệ thống thông tin",
                    SoTinChi = 3,
                    HocPhi = 3200000,
                    TrangThai = true,
                    MoTa = "Khảo sát nghiệp vụ, phân tích yêu cầu phần mềm, mô hình hóa hệ thống thông tin theo chuẩn UML và thiết kế hệ thống dữ liệu."
                }
            };

            if (!context.MonHocs.Any())
            {
                context.MonHocs.AddRange(monHocs);
                context.SaveChanges();
            }
            else
            {
                bool hasNew = false;
                foreach (var mh in monHocs)
                {
                    if (!context.MonHocs.Any(m => m.TenMonHoc == mh.TenMonHoc))
                    {
                        context.MonHocs.Add(mh);
                        hasNew = true;
                    }
                }
                if (hasNew)
                {
                    context.SaveChanges();
                }
            }

            // 2. Nạp giảng viên mẫu (nạp bổ sung theo Email: CSDL đã có giảng viên cũ
            //    vẫn tự thêm được giảng viên mới khi chạy lại app, không cần xóa dữ liệu)
            var giangViens = new List<GiangVien>
            {
                new GiangVien
                {
                    HoTen = "TS. Nguyễn Hoàng Long",
                    Email = "nhlong@uneti.edu.vn",
                    SoDienThoai = "0912345678",
                    ChuyenMon = "Công nghệ phần mềm & Lập trình .NET",
                    HocVi = "Tiến sĩ",
                    GioiTinh = "Nam",
                    TrangThai = true
                },
                new GiangVien
                {
                    HoTen = "ThS. Trần Thị Mai Lan",
                    Email = "ttmlan@uneti.edu.vn",
                    SoDienThoai = "0923456789",
                    ChuyenMon = "Cơ sở dữ liệu & Hệ thống thông tin",
                    HocVi = "Thạc sĩ",
                    GioiTinh = "Nữ",
                    TrangThai = true
                },
                new GiangVien
                {
                    HoTen = "ThS. Lê Tuấn Anh",
                    Email = "ltanh@uneti.edu.vn",
                    SoDienThoai = "0934567890",
                    ChuyenMon = "Lập trình Web & Di động (React, Flutter)",
                    HocVi = "Thạc sĩ",
                    GioiTinh = "Nam",
                    TrangThai = true
                },
                new GiangVien
                {
                    HoTen = "TS. Phạm Minh Đức",
                    Email = "pmduc@uneti.edu.vn",
                    SoDienThoai = "0945678901",
                    ChuyenMon = "Khoa học Dữ liệu & Trí tuệ Nhân tạo",
                    HocVi = "Tiến sĩ",
                    GioiTinh = "Nam",
                    TrangThai = true
                },
                new GiangVien
                {
                    HoTen = "TS. Vũ Thị Thanh Hằng",
                    Email = "vtthang@uneti.edu.vn",
                    SoDienThoai = "0956789012",
                    ChuyenMon = "An toàn thông tin & Bảo mật hệ thống",
                    HocVi = "Tiến sĩ",
                    GioiTinh = "Nữ",
                    TrangThai = true
                },
                new GiangVien
                {
                    HoTen = "ThS. Đỗ Thu Trang",
                    Email = "dttrang@uneti.edu.vn",
                    SoDienThoai = "0967890123",
                    ChuyenMon = "Phân tích thiết kế hệ thống & Kiểm thử phần mềm",
                    HocVi = "Thạc sĩ",
                    GioiTinh = "Nữ",
                    TrangThai = true
                },
                new GiangVien
                {
                    HoTen = "ThS. Bùi Diệu Linh",
                    Email = "bdlinh@uneti.edu.vn",
                    SoDienThoai = "0978901234",
                    ChuyenMon = "Mạng máy tính & Quản trị hệ thống mạng",
                    HocVi = "Thạc sĩ",
                    GioiTinh = "Nữ",
                    TrangThai = true
                },
                new GiangVien
                {
                    HoTen = "TS. Ngô Văn Thắng",
                    Email = "nvthang@uneti.edu.vn",
                    SoDienThoai = "0989012345",
                    ChuyenMon = "Hệ điều hành & Lập trình hệ thống",
                    HocVi = "Tiến sĩ",
                    GioiTinh = "Nam",
                    TrangThai = true
                },
                new GiangVien
                {
                    HoTen = "ThS. Trịnh Quốc Bảo",
                    Email = "tqbao@uneti.edu.vn",
                    SoDienThoai = "0990123456",
                    ChuyenMon = "Lập trình Java & Kiến trúc phần mềm doanh nghiệp",
                    HocVi = "Thạc sĩ",
                    GioiTinh = "Nam",
                    TrangThai = true
                }
            };

            bool hasNewGiangVien = false;
            foreach (var gv in giangViens)
            {
                if (!context.GiangViens.Any(g => g.Email == gv.Email))
                {
                    context.GiangViens.Add(gv);
                    hasNewGiangVien = true;
                }
            }
            if (hasNewGiangVien)
            {
                context.SaveChanges();
            }

            // 2b. Điền giới tính cho giảng viên cũ chưa có (suy từ họ tên) để avatar hiển thị đúng Nam/Nữ
            bool hasGenderBackfill = false;
            foreach (var gv in context.GiangViens.Where(g => string.IsNullOrEmpty(g.GioiTinh)).ToList())
            {
                var gioiTinh = GiangVienAvatarHelper.SuyDoanTuTen(gv.HoTen);
                if (gioiTinh != null)
                {
                    gv.GioiTinh = gioiTinh;
                    hasGenderBackfill = true;
                }
            }
            if (hasGenderBackfill)
            {
                context.SaveChanges();
            }

            // 3. Nạp 02 tài khoản Admin (Mục 16 Đề 16)
            if (!context.TaiKhoans.Any(t => t.TenDangNhap == "admin"))
            {
                context.TaiKhoans.Add(new TaiKhoan
                {
                    TenDangNhap = "admin",
                    MatKhau = PasswordHelper.HashPassword("Admin@123"),
                    HoTen = "Quản trị viên Hệ thống",
                    Email = "admin@uneti.edu.vn",
                    VaiTro = VaiTro.Admin,
                    TrangThai = true,
                    NgayTao = DateTime.Now
                });
            }

            if (!context.TaiKhoans.Any(t => t.TenDangNhap == "admin_tonghop"))
            {
                context.TaiKhoans.Add(new TaiKhoan
                {
                    TenDangNhap = "admin_tonghop",
                    MatKhau = PasswordHelper.HashPassword("Admin@123"),
                    HoTen = "Nguyễn Quản Trị",
                    Email = "quantri@uneti.edu.vn",
                    VaiTro = VaiTro.Admin,
                    TrangThai = true,
                    NgayTao = DateTime.Now
                });
            }

            // 4. Nạp 03 tài khoản Nhân viên
            if (!context.TaiKhoans.Any(t => t.TenDangNhap == "nv_daotao"))
            {
                context.TaiKhoans.Add(new TaiKhoan
                {
                    TenDangNhap = "nv_daotao",
                    MatKhau = PasswordHelper.HashPassword("User@123"),
                    HoTen = "Trần Văn Đào Tạo",
                    Email = "daotao@uneti.edu.vn",
                    VaiTro = VaiTro.NhanVien,
                    TrangThai = true,
                    NgayTao = DateTime.Now
                });
            }

            if (!context.TaiKhoans.Any(t => t.TenDangNhap == "nv_tuyensinh"))
            {
                context.TaiKhoans.Add(new TaiKhoan
                {
                    TenDangNhap = "nv_tuyensinh",
                    MatKhau = PasswordHelper.HashPassword("User@123"),
                    HoTen = "Lê Thị Tuyển Sinh",
                    Email = "tuyensinh@uneti.edu.vn",
                    VaiTro = VaiTro.NhanVien,
                    TrangThai = true,
                    NgayTao = DateTime.Now
                });
            }

            if (!context.TaiKhoans.Any(t => t.TenDangNhap == "nv_ketoan"))
            {
                context.TaiKhoans.Add(new TaiKhoan
                {
                    TenDangNhap = "nv_ketoan",
                    MatKhau = PasswordHelper.HashPassword("User@123"),
                    HoTen = "Phạm Thu Ngân",
                    Email = "thungan@uneti.edu.vn",
                    VaiTro = VaiTro.NhanVien,
                    TrangThai = true,
                    NgayTao = DateTime.Now
                });
            }
            context.SaveChanges();

            // 5. Nạp 30 Học viên đầy đủ kèm tài khoản TaiKhoan
            var dsHocVienMau = new List<(string Username, string HoTen, string NgaySinh, string GioiTinh, string Sdt, string Email, string DiaChi, string TrinhDo, bool TrangThai)>
            {
                ("sv_nguyenvana", "Nguyễn Văn An", "2003-05-15", "Nam", "0987654321", "an.nv@gmail.com", "Hà Nội", "Đại học", true),
                ("sv_tranthib", "Trần Thị Bích", "2004-08-20", "Nữ", "0912345671", "bich.tt@gmail.com", "Nam Định", "Đại học", true),
                ("sv_leducchinh", "Lê Đức Chính", "2003-03-12", "Nam", "0923456782", "chinh.ld@gmail.com", "Hải Phòng", "Cử nhân", true),
                ("sv_phamminhdang", "Phạm Minh Đăng", "2004-11-05", "Nam", "0934567893", "dang.pm@gmail.com", "Bắc Ninh", "Cao đẳng", true),
                ("sv_hoangthuha", "Hoàng Thu Hà", "2004-09-18", "Nữ", "0945678904", "ha.ht@gmail.com", "Hà Nam", "Đại học", true),
                ("sv_vuthikimlien", "Vũ Thị Kim Liên", "2003-02-22", "Nữ", "0956789015", "lien.vtk@gmail.com", "Thái Bình", "Đại học", true),
                ("sv_dangquanghuy", "Đặng Quang Huy", "2004-07-14", "Nam", "0967890126", "huy.dq@gmail.com", "Hưng Yên", "Đại học", true),
                ("sv_ngothimyhuyen", "Ngô Thị Mỹ Huyền", "2003-10-30", "Nữ", "0978901237", "huyen.ntm@gmail.com", "Vĩnh Phúc", "Cử nhân", true),
                ("sv_buituankiet", "Bùi Tuấn Kiệt", "2004-04-08", "Nam", "0989012348", "kiet.bt@gmail.com", "Quảng Ninh", "Đại học", true),
                ("sv_doandinhkhoa", "Đoàn Đình Khoa", "2003-12-25", "Nam", "0911223349", "khoa.dd@gmail.com", "Hà Nội", "Kỹ sư", true),
                ("sv_duongthanhlong", "Dương Thành Long", "2004-06-19", "Nam", "0922334450", "long.dt@gmail.com", "Hải Dương", "Đại học", true),
                ("sv_hoangbaongoc", "Hoàng Bảo Ngọc", "2004-01-03", "Nữ", "0933445561", "ngoc.hb@gmail.com", "Nam Định", "Đại học", true),
                ("sv_lyminhnhat", "Lý Minh Nhật", "2003-08-11", "Nam", "0944556672", "nhat.lm@gmail.com", "Bắc Giang", "Đại học", true),
                ("sv_maithiphuong", "Mai Thị Phương", "2004-04-27", "Nữ", "0955667783", "phuong.mt@gmail.com", "Ninh Bình", "Cử nhân", true),
                ("sv_nguyenthanhphuc", "Nguyễn Thành Phúc", "2003-09-09", "Nam", "0966778894", "phuc.nt@gmail.com", "Hà Nội", "Đại học", true),
                ("sv_phanhoangquan", "Phan Hoàng Quân", "2004-01-16", "Nam", "0977889905", "quan.ph@gmail.com", "Thanh Hóa", "Đại học", true),
                ("sv_taquocson", "Tạ Quốc Sơn", "2003-10-04", "Nam", "0988990016", "son.tq@gmail.com", "Nghệ An", "Kỹ sư", true),
                ("sv_trinhngocthai", "Trịnh Ngọc Thái", "2004-05-21", "Nam", "0919876527", "thai.tn@gmail.com", "Phú Thọ", "Đại học", true),
                ("sv_dinhtrungkien", "Đinh Trung Kiên", "2003-11-13", "Nam", "0928765438", "kien.dt@gmail.com", "Hà Nội", "Đại học", true),
                ("sv_nguyenphuongthao", "Nguyễn Phương Thảo", "2004-03-07", "Nữ", "0937654349", "thao.np@gmail.com", "Thái Nguyên", "Đại học", true),
                ("sv_vovanvuong", "Võ Văn Vượng", "2003-08-29", "Nam", "0946543250", "vuong.vv@gmail.com", "Hà Tĩnh", "Đại học", true),
                ("sv_chuquangvinh", "Chu Quang Vinh", "2004-12-17", "Nam", "0955432161", "vinh.cq@gmail.com", "Nam Định", "Cử nhân", true),
                ("sv_nguyenthiyen", "Nguyễn Thị Yến", "2003-06-24", "Nữ", "0964321072", "yen.nt@gmail.com", "Hà Nội", "Đại học", true),
                ("sv_phamvietanh", "Phạm Việt Anh", "2004-02-02", "Nam", "0973210983", "anh.pv@gmail.com", "Bắc Ninh", "Kỹ sư", true),
                ("sv_tranthubaotram", "Trần Thị Bảo Trâm", "2004-10-10", "Nữ", "0982109894", "tram.ttb@gmail.com", "Hải Dương", "Đại học", true),
                ("sv_letuanhung", "Lê Tuấn Hùng", "2003-07-06", "Nam", "0910987605", "hung.lt@gmail.com", "Hà Nam", "Đại học", true),
                ("sv_nguyenkhanhlinh", "Nguyễn Khánh Linh", "2004-09-15", "Nữ", "0929876516", "linh.nk@gmail.com", "Hà Nội", "Đại học", true),
                ("sv_hoangtrongnam", "Hoàng Trọng Nam", "2003-03-28", "Nam", "0938765427", "nam.ht@gmail.com", "Hưng Yên", "Đại học", true),
                ("sv_quachanhhoa", "Quách Ánh Hoa", "2004-11-19", "Nữ", "0947654338", "hoa.qa@gmail.com", "Hải Phòng", "Đại học", true),
                ("sv_dogiakhoa", "Đỗ Gia Khóa", "2003-01-01", "Nam", "0956543249", "khoa.dg@gmail.com", "Hà Nội", "Đại học", false),
                ("sv_bitaikhoa", "Sinh Viên Bị Khóa", "2003-01-01", "Nam", "0999999999", "bitaikhoa@sv.uneti.edu.vn", "Hà Nội", "Đại học", false)
            };

            foreach (var item in dsHocVienMau)
            {
                var taiKhoan = context.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == item.Username);
                if (taiKhoan == null)
                {
                    taiKhoan = new TaiKhoan
                    {
                        TenDangNhap = item.Username,
                        MatKhau = PasswordHelper.HashPassword("User@123"),
                        HoTen = item.HoTen,
                        Email = item.Email,
                        VaiTro = VaiTro.HocVien,
                        TrangThai = item.TrangThai,
                        NgayTao = DateTime.Now
                    };
                    context.TaiKhoans.Add(taiKhoan);
                    context.SaveChanges();
                }

                if (!context.HocViens.Any(h => h.MaTaiKhoan == taiKhoan.MaTaiKhoan || h.Email == item.Email))
                {
                    context.HocViens.Add(new HocVien
                    {
                        MaTaiKhoan = taiKhoan.MaTaiKhoan,
                        HoTen = item.HoTen,
                        NgaySinh = DateTime.Parse(item.NgaySinh),
                        GioiTinh = item.GioiTinh,
                        SoDienThoai = item.Sdt,
                        Email = item.Email,
                        DiaChi = item.DiaChi,
                        TrinhDo = item.TrinhDo,
                        NgayDangKy = DateTime.Now,
                        TrangThai = item.TrangThai
                    });
                }
            }
            context.SaveChanges();

            // 6. Nạp Khóa học mẫu (Module 2) nếu chưa có dữ liệu
            if (!context.KhoaHocs.Any())
            {
                var allMonHocs = context.MonHocs.ToList();
                var allGiangViens = context.GiangViens.ToList();

                if (allMonHocs.Any() && allGiangViens.Any())
                {
                    var m1 = allMonHocs.ElementAtOrDefault(0) ?? allMonHocs.First();
                    var m2 = allMonHocs.ElementAtOrDefault(1) ?? allMonHocs.First();
                    var m3 = allMonHocs.ElementAtOrDefault(2) ?? allMonHocs.First();
                    var m4 = allMonHocs.ElementAtOrDefault(3) ?? allMonHocs.First();
                    var m5 = allMonHocs.ElementAtOrDefault(5) ?? allMonHocs.First();
                    var m6 = allMonHocs.ElementAtOrDefault(6) ?? allMonHocs.First();

                    var g1 = allGiangViens.ElementAtOrDefault(0) ?? allGiangViens.First();
                    var g2 = allGiangViens.ElementAtOrDefault(1) ?? allGiangViens.First();
                    var g3 = allGiangViens.ElementAtOrDefault(2) ?? allGiangViens.First();
                    var g4 = allGiangViens.ElementAtOrDefault(3) ?? allGiangViens.First();

                    var dsKhoaHocMau = new List<KhoaHoc>
                    {
                        new KhoaHoc
                        {
                            TenKhoaHoc = "Lập trình C# và .NET Core MVC - Lớp K17A",
                            MaMonHoc = m1.MaMonHoc,
                            MaGiangVien = g1.MaGiangVien,
                            NgayBatDau = DateTime.Today.AddDays(7),
                            NgayKetThuc = DateTime.Today.AddMonths(3),
                            SoLuongToiDa = 30,
                            HocPhi = 3500000,
                            HinhThuc = "Trực tiếp",
                            TrangThai = "DangMo",
                            MoTa = "Khóa đào tạo chuyên sâu kiến trúc ASP.NET Core MVC 10, Entity Framework Core và lập trình Web doanh nghiệp."
                        },
                        new KhoaHoc
                        {
                            TenKhoaHoc = "Cơ sở Dữ liệu & SQL Server Nâng cao",
                            MaMonHoc = m2.MaMonHoc,
                            MaGiangVien = g2.MaGiangVien,
                            NgayBatDau = DateTime.Today.AddDays(10),
                            NgayKetThuc = DateTime.Today.AddMonths(3),
                            SoLuongToiDa = 25,
                            HocPhi = 3000000,
                            HinhThuc = "Kết hợp",
                            TrangThai = "DangMo",
                            MoTa = "Thiết kế CSDL quan hệ, T-SQL, Stored Procedure, Trigger, Transaction và kỹ thuật tối ưu hóa truy vấn Indexing."
                        },
                        new KhoaHoc
                        {
                            TenKhoaHoc = "Lập trình Web Frontend với ReactJS & Redux",
                            MaMonHoc = m3.MaMonHoc,
                            MaGiangVien = g3.MaGiangVien,
                            NgayBatDau = DateTime.Today.AddDays(14),
                            NgayKetThuc = DateTime.Today.AddMonths(3),
                            SoLuongToiDa = 35,
                            HocPhi = 3800000,
                            HinhThuc = "Trực tuyến",
                            TrangThai = "DangMo",
                            MoTa = "Xây dựng Single Page Application hiện đại với React, Redux Toolkit, Tailwind CSS và kết nối RESTful API."
                        },
                        new KhoaHoc
                        {
                            TenKhoaHoc = "Lập trình Di động Đa nền tảng Flutter & Dart",
                            MaMonHoc = m4.MaMonHoc,
                            MaGiangVien = g1.MaGiangVien,
                            NgayBatDau = DateTime.Today.AddDays(20),
                            NgayKetThuc = DateTime.Today.AddMonths(4),
                            SoLuongToiDa = 25,
                            HocPhi = 4200000,
                            HinhThuc = "Trực tiếp",
                            TrangThai = "DangMo",
                            MoTa = "Phát triển ứng dụng di động cho cả iOS và Android trên cùng một codebase, quản lý state và tích hợp Firebase."
                        },
                        new KhoaHoc
                        {
                            TenKhoaHoc = "Lập trình Hướng đối tượng & Mẫu thiết kế",
                            MaMonHoc = m5.MaMonHoc,
                            MaGiangVien = g2.MaGiangVien,
                            NgayBatDau = DateTime.Today.AddDays(5),
                            NgayKetThuc = DateTime.Today.AddMonths(2),
                            SoLuongToiDa = 40,
                            HocPhi = 3200000,
                            HinhThuc = "Trực tiếp",
                            TrangThai = "DangMo",
                            MoTa = "Tư duy thiết kế phần mềm hướng đối tượng theo nguyên lý SOLID, áp dụng các mẫu thiết kế Creational, Structural và Behavioral."
                        },
                        new KhoaHoc
                        {
                            TenKhoaHoc = "Cấu trúc Dữ liệu & Giải thuật Ứng dụng",
                            MaMonHoc = m6.MaMonHoc,
                            MaGiangVien = g4.MaGiangVien,
                            NgayBatDau = DateTime.Today.AddDays(8),
                            NgayKetThuc = DateTime.Today.AddMonths(3),
                            SoLuongToiDa = 30,
                            HocPhi = 3200000,
                            HinhThuc = "Trực tuyến",
                            TrangThai = "DangMo",
                            MoTa = "Cấu trúc dữ liệu tuyến tính và phi tuyến, các thuật toán sắp xếp và tìm kiếm nâng cao phục vụ phỏng vấn lập trình viên."
                        }
                    };

                    context.KhoaHocs.AddRange(dsKhoaHocMau);
                    context.SaveChanges();
                }
            }

            // 6b. Bổ sung khóa học mẫu cho đủ 15 khóa, đa trạng thái (Mục 16 Đề 16)
            //     Nạp cộng dồn theo tên khóa: CSDL đã có 6 khóa cũ vẫn tự thêm 9 khóa mới khi chạy lại app.
            //     Phân công theo đúng chuyên môn: 5 giảng viên mới mỗi người 1 khóa, 4 giảng viên cũ thêm lớp K17B.
            {
                var gvTheoEmail = context.GiangViens.GroupBy(g => g.Email).ToDictionary(gr => gr.Key, gr => gr.First().MaGiangVien);
                var mhTheoTen = context.MonHocs.GroupBy(m => m.TenMonHoc).ToDictionary(gr => gr.Key, gr => gr.First().MaMonHoc);
                var dsBoSung = new List<KhoaHoc>();

                void ThemKhoa(string tenKhoa, string tenMonHoc, string emailGV, DateTime batDau, DateTime ketThuc,
                              int siSo, decimal hocPhi, string hinhThuc, string trangThai, string moTa)
                {
                    if (!mhTheoTen.TryGetValue(tenMonHoc, out var maMH)) return;
                    if (!gvTheoEmail.TryGetValue(emailGV, out var maGV)) return;
                    dsBoSung.Add(new KhoaHoc
                    {
                        TenKhoaHoc = tenKhoa,
                        MaMonHoc = maMH,
                        MaGiangVien = maGV,
                        NgayBatDau = batDau,
                        NgayKetThuc = ketThuc,
                        SoLuongToiDa = siSo,
                        HocPhi = hocPhi,
                        HinhThuc = hinhThuc,
                        TrangThai = trangThai,
                        MoTa = moTa
                    });
                }

                ThemKhoa("An toàn thông tin & Mật mã ứng dụng", "An toàn thông tin", "vtthang@uneti.edu.vn",
                    DateTime.Today.AddDays(12), DateTime.Today.AddMonths(3), 30, 3400000, "Trực tiếp", "DangMo",
                    "Nguyên lý mật mã, chữ ký số, bảo mật ứng dụng Web và quy trình ứng phó sự cố an toàn thông tin.");
                ThemKhoa("Mạng máy tính & Quản trị mạng doanh nghiệp", "Mạng máy tính", "bdlinh@uneti.edu.vn",
                    DateTime.Today.AddDays(-20), DateTime.Today.AddMonths(2), 30, 3000000, "Kết hợp", "DangHoc",
                    "Cấu hình Router, Switch, VLAN, định tuyến và quản trị hệ thống mạng doanh nghiệp trên thiết bị thật.");
                ThemKhoa("Hệ điều hành Linux & Quản trị hệ thống", "Hệ điều hành", "nvthang@uneti.edu.vn",
                    DateTime.Today.AddDays(15), DateTime.Today.AddMonths(3), 35, 3000000, "Trực tuyến", "DangMo",
                    "Quản trị Linux Server, lập trình Shell, quản lý tiến trình, bộ nhớ và bảo mật hệ thống.");
                ThemKhoa("Công nghệ Java & Spring Boot cho doanh nghiệp", "Công nghệ Java", "tqbao@uneti.edu.vn",
                    DateTime.Today.AddDays(10), DateTime.Today.AddMonths(3), 30, 3600000, "Trực tiếp", "DangMo",
                    "Java Core, Spring Boot, Spring Data JPA và xây dựng REST API cho ứng dụng doanh nghiệp.");
                ThemKhoa("Phân tích & Thiết kế HTTT với UML", "Phân tích và thiết kế các hệ thống thông tin", "dttrang@uneti.edu.vn",
                    DateTime.Today.AddDays(35), DateTime.Today.AddMonths(4), 40, 3200000, "Trực tuyến", "SapMo",
                    "Khảo sát nghiệp vụ, mô hình hóa UML, thiết kế kiến trúc và đặc tả yêu cầu hệ thống thông tin.");
                ThemKhoa("Lập trình C# và .NET Core MVC - Lớp K17B", "Lập trình C# và .NET Core", "nhlong@uneti.edu.vn",
                    DateTime.Today.AddDays(-30), DateTime.Today.AddMonths(2), 30, 3500000, "Kết hợp", "DangHoc",
                    "Lớp K17B: ASP.NET Core MVC, Entity Framework Core và triển khai ứng dụng Web doanh nghiệp.");
                ThemKhoa("Cơ sở Dữ liệu & SQL Server - Lớp K17B", "Cơ sở Dữ liệu & SQL Server", "ttmlan@uneti.edu.vn",
                    DateTime.Today.AddDays(18), DateTime.Today.AddMonths(3), 25, 3000000, "Trực tuyến", "DangMo",
                    "Lớp K17B học trực tuyến: T-SQL, Stored Procedure, Trigger và tối ưu truy vấn SQL Server.");
                ThemKhoa("Lập trình Di động Flutter & Dart - Lớp K17B", "Lập trình Di động với Flutter", "ltanh@uneti.edu.vn",
                    DateTime.Today.AddDays(40), DateTime.Today.AddMonths(4), 25, 4200000, "Trực tiếp", "SapMo",
                    "Lớp K17B: Phát triển ứng dụng iOS & Android với Dart, Flutter và tích hợp Firebase.");
                ThemKhoa("Cấu trúc Dữ liệu & Giải thuật - Lớp K17B", "Cấu trúc dữ liệu và giải thuật", "pmduc@uneti.edu.vn",
                    DateTime.Today.AddMonths(-5), DateTime.Today.AddMonths(-2), 30, 3200000, "Trực tuyến", "DaKetThuc",
                    "Lớp K17B đã hoàn thành: cấu trúc dữ liệu, đồ thị và các thuật toán sắp xếp, tìm kiếm nâng cao.");

                bool hasNewKhoa = false;
                foreach (var kh in dsBoSung)
                {
                    if (!context.KhoaHocs.Any(k => k.TenKhoaHoc == kh.TenKhoaHoc))
                    {
                        context.KhoaHocs.Add(kh);
                        hasNewKhoa = true;
                    }
                }
                if (hasNewKhoa)
                {
                    context.SaveChanges();
                }
            }

            // 7. Nạp Giao dịch Đăng ký khóa học mẫu (40-50 giao dịch) & Kết quả học tập (15-20 bản ghi)
            if (!context.DangKyKhoaHocs.Any())
            {
                var dsHocVien = context.HocViens.ToList();
                var dsKhoaHoc = context.KhoaHocs.ToList();

                if (dsHocVien.Any() && dsKhoaHoc.Any())
                {
                    var random = new Random(16); // Seed cố định để dữ liệu nhất quán
                    var dsDangKyMau = new List<DangKyKhoaHoc>();
                    var registeredPairs = new HashSet<string>();

                    var trangThaiPool = new[]
                    {
                        "HoanThanh", "HoanThanh", "HoanThanh", "HoanThanh", // Trọng số cao cho Hoàn thành để có dữ liệu chấm điểm
                        "DangXuLy", "DangXuLy",
                        "ChoXuLy", "ChoXuLy",
                        "BiHuy"
                    };

                    int totalOrders = Math.Min(45, dsHocVien.Count * dsKhoaHoc.Count);
                    int orderIndex = 0;

                    // Phân bổ đều giữa các học viên và khóa học
                    for (int h = 0; h < dsHocVien.Count && dsDangKyMau.Count < totalOrders; h++)
                    {
                        var hv = dsHocVien[h];
                        // Mỗi học viên đăng ký 1 - 2 khóa
                        int numCourses = (h % 3 == 0) ? 2 : 1;

                        for (int c = 0; c < numCourses && dsDangKyMau.Count < totalOrders; c++)
                        {
                            var kh = dsKhoaHoc[(h + c) % dsKhoaHoc.Count];
                            var pairKey = $"{hv.MaHocVien}_{kh.MaKhoaHoc}";
                            if (registeredPairs.Contains(pairKey)) continue;
                            registeredPairs.Add(pairKey);

                            var trangThai = trangThaiPool[orderIndex % trangThaiPool.Length];
                            orderIndex++;

                            var daysAgo = random.Next(5, 120);
                            var ngayDk = DateTime.Now.AddDays(-daysAgo);

                            decimal daDong = 0;
                            decimal conLai = kh.HocPhi;
                            DateTime? ngayXacNhan = null;
                            string? ghiChu = null;

                            if (trangThai == "HoanThanh")
                            {
                                daDong = kh.HocPhi;
                                conLai = 0;
                                ngayXacNhan = ngayDk.AddDays(random.Next(1, 3));
                                ghiChu = "Đã thu đủ 100% học phí và hoàn tất xếp lớp.";
                            }
                            else if (trangThai == "DangXuLy")
                            {
                                daDong = (orderIndex % 2 == 0) ? (kh.HocPhi / 2) : 0;
                                conLai = Math.Max(0, kh.HocPhi - daDong);
                                ngayXacNhan = ngayDk.AddHours(random.Next(2, 24));
                                ghiChu = daDong > 0 ? "Đã đặt cọc 50% học phí." : "Đã tiếp nhận hồ sơ sơ bộ.";
                            }
                            else if (trangThai == "BiHuy")
                            {
                                daDong = 0;
                                conLai = kh.HocPhi;
                                ghiChu = "Học viên bận lịch công tác nên xin hủy đơn.";
                            }

                            dsDangKyMau.Add(new DangKyKhoaHoc
                            {
                                MaHocVien = hv.MaHocVien,
                                MaKhoaHoc = kh.MaKhoaHoc,
                                NgayDangKy = ngayDk,
                                TrangThai = trangThai,
                                SoTienDaDong = daDong,
                                SoTienConLai = conLai,
                                NgayXacNhan = ngayXacNhan,
                                GhiChu = ghiChu
                            });
                        }
                    }

                    context.DangKyKhoaHocs.AddRange(dsDangKyMau);
                    context.SaveChanges();

                    // 8. Nạp 18 bản ghi Kết quả học tập mẫu (Mục 16 Đề 16)
                    var dsHoanThanh = context.DangKyKhoaHocs
                        .Where(d => d.TrangThai == "HoanThanh")
                        .Take(18)
                        .ToList();

                    var sampleScores = new (decimal CC, decimal GK, decimal CK)[]
                    {
                        (9.5m, 9.0m, 9.5m), // 9.30 -> XuatSac
                        (9.0m, 8.5m, 9.0m), // 8.80 -> Gioi
                        (8.5m, 8.0m, 8.5m), // 8.30 -> Gioi
                        (10.0m, 9.5m, 9.0m), // 9.30 -> XuatSac
                        (8.0m, 7.5m, 7.0m), // 7.30 -> Kha
                        (7.5m, 7.0m, 7.5m), // 7.30 -> Kha
                        (8.0m, 8.0m, 8.0m), // 8.00 -> Gioi
                        (7.0m, 6.5m, 6.5m), // 6.55 -> Kha
                        (6.5m, 6.0m, 6.0m), // 6.05 -> TrungBinh
                        (6.0m, 5.5m, 5.5m), // 5.55 -> TrungBinh
                        (7.0m, 5.5m, 5.0m), // 5.40 -> TrungBinh
                        (8.5m, 8.5m, 8.0m), // 8.25 -> Gioi
                        (9.0m, 8.0m, 8.5m), // 8.35 -> Gioi
                        (5.0m, 4.5m, 4.0m), // 4.30 -> Yeu (KhongDat để test tỷ lệ)
                        (6.0m, 4.0m, 4.0m), // 4.20 -> Yeu (KhongDat để test tỷ lệ)
                        (8.0m, 7.0m, 7.5m), // 7.35 -> Kha
                        (9.5m, 9.5m, 9.0m), // 9.25 -> XuatSac
                        (7.5m, 7.5m, 7.0m)  // 7.25 -> Kha
                    };

                    var dsKetQuaMau = new List<KetQuaHocTap>();
                    for (int i = 0; i < dsHoanThanh.Count; i++)
                    {
                        var score = sampleScores[i % sampleScores.Length];
                        var cc = score.CC;
                        var gk = score.GK;
                        var ck = score.CK;
                        var tk = Math.Round(cc * 0.1m + gk * 0.4m + ck * 0.5m, 2);

                        string xl = tk switch
                        {
                            >= 9.0m => "XuatSac",
                            >= 8.0m => "Gioi",
                            >= 6.5m => "Kha",
                            >= 5.0m => "TrungBinh",
                            _ => "Yeu"
                        };

                        string kq = tk >= 5.0m ? "Dat" : "KhongDat";

                        dsKetQuaMau.Add(new KetQuaHocTap
                        {
                            MaDangKy = dsHoanThanh[i].MaDangKy,
                            DiemChuyenCan = cc,
                            DiemGiuaKy = gk,
                            DiemCuoiKy = ck,
                            DiemTongKet = tk,
                            XepLoai = xl,
                            KetQua = kq,
                            NgayCapNhat = DateTime.Now.AddDays(-random.Next(1, 30))
                        });
                    }

                    context.KetQuaHocTaps.AddRange(dsKetQuaMau);
                    context.SaveChanges();
                }
            }
        }
    }
}
