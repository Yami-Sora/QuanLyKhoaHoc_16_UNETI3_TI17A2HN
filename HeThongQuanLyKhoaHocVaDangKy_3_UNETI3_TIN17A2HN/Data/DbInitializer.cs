// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Module 1: Khởi tạo dữ liệu mẫu (Chuẩn Mục 16 Đề 16: 05 môn học, 02 Admin, 03 Nhân viên, 30 Học viên)

using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Helpers;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            // 1. Nạp môn học mẫu (12 môn học theo khung đào tạo CNTT UNETI)
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

            // 2. Nạp giảng viên mẫu
            if (!context.GiangViens.Any())
            {
                var giangViens = new List<GiangVien>
                {
                    new GiangVien
                    {
                        HoTen = "TS. Nguyễn Hoàng Long",
                        Email = "nhlong@uneti.edu.vn",
                        SoDienThoai = "0912345678",
                        ChuyenMon = "Công nghệ phần mềm & Lập trình .NET",
                        HocVi = "Tiến sĩ",
                        TrangThai = true
                    },
                    new GiangVien
                    {
                        HoTen = "ThS. Trần Thị Mai Lan",
                        Email = "ttmlan@uneti.edu.vn",
                        SoDienThoai = "0923456789",
                        ChuyenMon = "Cơ sở dữ liệu & Hệ thống thông tin",
                        HocVi = "Thạc sĩ",
                        TrangThai = true
                    },
                    new GiangVien
                    {
                        HoTen = "ThS. Lê Tuấn Anh",
                        Email = "ltanh@uneti.edu.vn",
                        SoDienThoai = "0934567890",
                        ChuyenMon = "Lập trình Web & Di động (React, Flutter)",
                        HocVi = "Thạc sĩ",
                        TrangThai = true
                    },
                    new GiangVien
                    {
                        HoTen = "TS. Phạm Minh Đức",
                        Email = "pmduc@uneti.edu.vn",
                        SoDienThoai = "0945678901",
                        ChuyenMon = "Khoa học Dữ liệu & Trí tuệ Nhân tạo",
                        HocVi = "Tiến sĩ",
                        TrangThai = true
                    }
                };
                context.GiangViens.AddRange(giangViens);
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

            // 4. Nạp 03 tài khoản Nhân viên (Mục 16 Đề 16)
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

            // 5. Nạp 30 Học viên đầy đủ kèm tài khoản TaiKhoan (Mục 16 Đề 16)
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
                ("sv_dogiakhoa", "Đỗ Gia Khóa", "2003-01-01", "Nam", "0956543249", "khoa.dg@gmail.com", "Hà Nội", "Đại học", false)
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
        }
    }
}
