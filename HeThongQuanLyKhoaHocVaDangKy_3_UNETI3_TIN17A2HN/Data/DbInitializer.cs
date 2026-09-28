// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Nội dung thực hiện: Module 1 - Khởi tạo dữ liệu mẫu ban đầu (Seed Data)

using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Helpers;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            // 1. Kiểm tra và Nạp Dữ liệu Môn học mẫu (Tối thiểu 5 môn theo Mục 16 Đề 16)
            if (!context.MonHocs.Any())
            {
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
                    }
                };
                context.MonHocs.AddRange(monHocs);
                context.SaveChanges();
            }

            // 2. Kiểm tra và Nạp Dữ liệu Giảng viên mẫu (Dữ liệu nền tảng phục vụ mở lớp)
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

            // 3. Kiểm tra và Nạp Dữ liệu Tài khoản mẫu (Đủ 3 vai trò + Test tài khoản bị khóa)
            if (!context.TaiKhoans.Any())
            {
                var taiKhoans = new List<TaiKhoan>
                {
                    new TaiKhoan
                    {
                        TenDangNhap = "admin",
                        MatKhau = PasswordHelper.HashPassword("Admin@123"),
                        HoTen = "Quản trị viên Hệ thống",
                        Email = "admin@uneti.edu.vn",
                        VaiTro = "Admin",
                        TrangThai = true,
                        NgayTao = DateTime.Now
                    },
                    new TaiKhoan
                    {
                        TenDangNhap = "nv_daotao",
                        MatKhau = PasswordHelper.HashPassword("Nv@123"),
                        HoTen = "Trần Văn Đào Tạo",
                        Email = "daotao@uneti.edu.vn",
                        VaiTro = "NhanVien",
                        TrangThai = true,
                        NgayTao = DateTime.Now
                    },
                    new TaiKhoan
                    {
                        TenDangNhap = "nv_tuyensinh",
                        MatKhau = PasswordHelper.HashPassword("Nv@123"),
                        HoTen = "Lê Thị Tuyển Sinh",
                        Email = "tuyensinh@uneti.edu.vn",
                        VaiTro = "NhanVien",
                        TrangThai = true,
                        NgayTao = DateTime.Now
                    },
                    new TaiKhoan
                    {
                        TenDangNhap = "sv_nguyenvana",
                        MatKhau = PasswordHelper.HashPassword("Sv@123"),
                        HoTen = "Nguyễn Văn A",
                        Email = "nguyenvana@gmail.com",
                        VaiTro = "HocVien",
                        TrangThai = true,
                        NgayTao = DateTime.Now
                    },
                    new TaiKhoan
                    {
                        TenDangNhap = "sv_bitaikhoa",
                        MatKhau = PasswordHelper.HashPassword("Sv@123"),
                        HoTen = "Phạm Văn Bị Khóa",
                        Email = "khoatk@gmail.com",
                        VaiTro = "HocVien",
                        TrangThai = false, // Dùng để kiểm thử Test Case khóa tài khoản (TC-07)
                        NgayTao = DateTime.Now
                    }
                };

                context.TaiKhoans.AddRange(taiKhoans);
                context.SaveChanges();

                // Tạo kèm hồ sơ Học viên cho tài khoản sv_nguyenvana
                var tkSv = context.TaiKhoans.First(t => t.TenDangNhap == "sv_nguyenvana");
                context.HocViens.Add(new HocVien
                {
                    MaTaiKhoan = tkSv.MaTaiKhoan,
                    HoTen = tkSv.HoTen,
                    Email = tkSv.Email,
                    SoDienThoai = "0987654321",
                    DiaChi = "Hà Nội",
                    GioiTinh = "Nam",
                    NgaySinh = new DateTime(2003, 5, 15),
                    NgayDangKy = DateTime.Now,
                    TrangThai = true
                });
                context.SaveChanges();
            }
            else
            {
                // Đồng bộ và cập nhật mật khẩu chuẩn theo các nút 1-Click nếu DB cũ đã tồn tại
                var defaultAccounts = new (string Username, string Password)[]
                {
                    ("admin", "Admin@123"),
                    ("nv_daotao", "Nv@123"),
                    ("nv_tuyensinh", "Nv@123"),
                    ("sv_nguyenvana", "Sv@123"),
                    ("sv_bitaikhoa", "Sv@123")
                };

                bool hasUpdate = false;
                foreach (var (uName, uPass) in defaultAccounts)
                {
                    var tk = context.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == uName);
                    if (tk != null && !PasswordHelper.VerifyPassword(uPass, tk.MatKhau))
                    {
                        tk.MatKhau = PasswordHelper.HashPassword(uPass);
                        hasUpdate = true;
                    }
                }

                if (hasUpdate)
                {
                    context.SaveChanges();
                }
            }
        }
    }
}
