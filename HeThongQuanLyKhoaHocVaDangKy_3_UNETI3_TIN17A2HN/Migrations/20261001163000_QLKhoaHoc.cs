// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Lớp: TIN17A2HN
// Migration QLKhoaHoc: Khởi tạo dữ liệu nền tảng hệ thống Quản lý Khóa học UNETI
// Bao gồm: 12 Môn học, 02 Quản trị viên (Admin), 03 Nhân viên (NhanVien), 30 Học viên (HocVien)

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Migrations
{
    /// <inheritdoc />
    public partial class QLKhoaHoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ====================================================================================
            // PHẦN 1: SEED 12 MÔN HỌC THEO KHUNG CHƯƠNG TRÌNH ĐÀO TẠO CNTT UNETI
            // ====================================================================================
            migrationBuilder.Sql(@"
                -- 1. Lập trình C# và .NET Core
                IF NOT EXISTS (SELECT 1 FROM [MonHoc] WHERE [TenMonHoc] = N'Lập trình C# và .NET Core')
                    INSERT INTO [MonHoc] ([TenMonHoc], [SoTinChi], [HocPhi], [TrangThai], [MoTa])
                    VALUES (N'Lập trình C# và .NET Core', 3, 3500000.00, 1, N'Kiến thức C# 13, ASP.NET Core MVC, Entity Framework Core và lập trình Web hiện đại.');

                -- 2. Cơ sở Dữ liệu & SQL Server
                IF NOT EXISTS (SELECT 1 FROM [MonHoc] WHERE [TenMonHoc] = N'Cơ sở Dữ liệu & SQL Server')
                    INSERT INTO [MonHoc] ([TenMonHoc], [SoTinChi], [HocPhi], [TrangThai], [MoTa])
                    VALUES (N'Cơ sở Dữ liệu & SQL Server', 3, 3000000.00, 1, N'Thiết kế CSDL quan hệ, T-SQL, Trigger, Store Procedure và tối ưu truy vấn.');

                -- 3. Lập trình Web Frontend với React
                IF NOT EXISTS (SELECT 1 FROM [MonHoc] WHERE [TenMonHoc] = N'Lập trình Web Frontend với React')
                    INSERT INTO [MonHoc] ([TenMonHoc], [SoTinChi], [HocPhi], [TrangThai], [MoTa])
                    VALUES (N'Lập trình Web Frontend với React', 3, 3800000.00, 1, N'Xây dựng giao diện tương tác Single Page Application với ReactJS, Redux Toolkit và Tailwind.');

                -- 4. Lập trình Di động với Flutter
                IF NOT EXISTS (SELECT 1 FROM [MonHoc] WHERE [TenMonHoc] = N'Lập trình Di động với Flutter')
                    INSERT INTO [MonHoc] ([TenMonHoc], [SoTinChi], [HocPhi], [TrangThai], [MoTa])
                    VALUES (N'Lập trình Di động với Flutter', 4, 4200000.00, 0, N'Phát triển ứng dụng di động đa nền tảng iOS & Android với Dart và Flutter Framework.');

                -- 5. Phân tích Dữ liệu với Python
                IF NOT EXISTS (SELECT 1 FROM [MonHoc] WHERE [TenMonHoc] = N'Phân tích Dữ liệu với Python')
                    INSERT INTO [MonHoc] ([TenMonHoc], [SoTinChi], [HocPhi], [TrangThai], [MoTa])
                    VALUES (N'Phân tích Dữ liệu với Python', 3, 4000000.00, 0, N'Xử lý dữ liệu lớn với Pandas, NumPy, Matplotlib và Machine Learning cơ bản (Tạm dừng tuyển sinh).');

                -- 6. Lập trình hướng đối tượng (Mục 36 khung đào tạo)
                IF NOT EXISTS (SELECT 1 FROM [MonHoc] WHERE [TenMonHoc] = N'Lập trình hướng đối tượng')
                    INSERT INTO [MonHoc] ([TenMonHoc], [SoTinChi], [HocPhi], [TrangThai], [MoTa])
                    VALUES (N'Lập trình hướng đối tượng', 3, 3200000.00, 1, N'Phương pháp lập trình hướng đối tượng với C++/Java, 4 đặc tính đóng gói, kế thừa, đa hình, trừu tượng và Design Patterns.');

                -- 7. Cấu trúc dữ liệu và giải thuật (Mục 37 khung đào tạo)
                IF NOT EXISTS (SELECT 1 FROM [MonHoc] WHERE [TenMonHoc] = N'Cấu trúc dữ liệu và giải thuật')
                    INSERT INTO [MonHoc] ([TenMonHoc], [SoTinChi], [HocPhi], [TrangThai], [MoTa])
                    VALUES (N'Cấu trúc dữ liệu và giải thuật', 3, 3200000.00, 1, N'Cấu trúc dữ liệu tuyến tính và phi tuyến (Danh sách, Ngăn xếp, Hàng đợi, Cây, Đồ thị), các thuật toán sắp xếp và tìm kiếm nâng cao.');

                -- 8. Mạng máy tính (Mục 40 khung đào tạo)
                IF NOT EXISTS (SELECT 1 FROM [MonHoc] WHERE [TenMonHoc] = N'Mạng máy tính')
                    INSERT INTO [MonHoc] ([TenMonHoc], [SoTinChi], [HocPhi], [TrangThai], [MoTa])
                    VALUES (N'Mạng máy tính', 3, 3000000.00, 1, N'Kiến trúc mạng máy tính, mô hình OSI và TCP/IP, cấu hình định tuyến Router, Switch, địa chỉ IP và bảo mật mạng cục bộ LAN/WAN.');

                -- 9. Hệ điều hành (Mục 42 khung đào tạo)
                IF NOT EXISTS (SELECT 1 FROM [MonHoc] WHERE [TenMonHoc] = N'Hệ điều hành')
                    INSERT INTO [MonHoc] ([TenMonHoc], [SoTinChi], [HocPhi], [TrangThai], [MoTa])
                    VALUES (N'Hệ điều hành', 3, 3000000.00, 1, N'Nguyên lý hoạt động của hệ điều hành: Quản lý tiến trình (Process), lập lịch CPU, đồng bộ luồng, bộ nhớ ảo và tệp tin.');

                -- 10. An toàn thông tin (Mục 41 khung đào tạo)
                IF NOT EXISTS (SELECT 1 FROM [MonHoc] WHERE [TenMonHoc] = N'An toàn thông tin')
                    INSERT INTO [MonHoc] ([TenMonHoc], [SoTinChi], [HocPhi], [TrangThai], [MoTa])
                    VALUES (N'An toàn thông tin', 3, 3400000.00, 1, N'Nguyên lý an ninh thông tin, mật mã hóa đối xứng/bất đối xứng, chữ ký số, chứng chỉ số và phòng ngừa các nguy cơ tấn công mạng.');

                -- 11. Công nghệ Java (Mục 47 khung đào tạo)
                IF NOT EXISTS (SELECT 1 FROM [MonHoc] WHERE [TenMonHoc] = N'Công nghệ Java')
                    INSERT INTO [MonHoc] ([TenMonHoc], [SoTinChi], [HocPhi], [TrangThai], [MoTa])
                    VALUES (N'Công nghệ Java', 3, 3600000.00, 1, N'Phát triển ứng dụng doanh nghiệp với Java Core, lập trình đa luồng, Spring Boot Framework, Spring Data JPA và kiến trúc REST API.');

                -- 12. Phân tích và thiết kế các hệ thống thông tin (Mục 46 khung đào tạo)
                IF NOT EXISTS (SELECT 1 FROM [MonHoc] WHERE [TenMonHoc] = N'Phân tích và thiết kế các hệ thống thông tin')
                    INSERT INTO [MonHoc] ([TenMonHoc], [SoTinChi], [HocPhi], [TrangThai], [MoTa])
                    VALUES (N'Phân tích và thiết kế các hệ thống thông tin', 3, 3200000.00, 1, N'Khảo sát nghiệp vụ, phân tích yêu cầu phần mềm, mô hình hóa hệ thống thông tin theo chuẩn UML và thiết kế hệ thống dữ liệu.');
            ");

            // ====================================================================================
            // PHẦN 2: SEED 02 TÀI KHOẢN ADMIN (Mật khẩu mặc định: Admin@123)
            // Hash SHA256 kèm Salt: 2C9F6FCB68FC7631586536B5DCAD6A60B07AC5AC12DD30C863D12F4B1ACA6319
            // ====================================================================================
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'admin')
                BEGIN
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('admin', '2C9F6FCB68FC7631586536B5DCAD6A60B07AC5AC12DD30C863D12F4B1ACA6319', N'Quản trị viên Hệ thống', 'admin@uneti.edu.vn', 'Admin', 1, GETDATE());
                END;

                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'admin_tonghop')
                BEGIN
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('admin_tonghop', '2C9F6FCB68FC7631586536B5DCAD6A60B07AC5AC12DD30C863D12F4B1ACA6319', N'Nguyễn Quản Trị', 'quantri@uneti.edu.vn', 'Admin', 1, GETDATE());
                END;
            ");

            // ====================================================================================
            // PHẦN 3: SEED 03 TÀI KHOẢN NHÂN VIÊN (Mật khẩu mặc định: User@123)
            // Hash SHA256 kèm Salt: EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F
            // ====================================================================================
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'nv_daotao')
                BEGIN
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('nv_daotao', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Trần Văn Đào Tạo', 'daotao@uneti.edu.vn', 'NhanVien', 1, GETDATE());
                END;

                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'nv_tuyensinh')
                BEGIN
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('nv_tuyensinh', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Lê Thị Tuyển Sinh', 'tuyensinh@uneti.edu.vn', 'NhanVien', 1, GETDATE());
                END;

                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'nv_ketoan')
                BEGIN
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('nv_ketoan', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Phạm Thu Ngân', 'thungan@uneti.edu.vn', 'NhanVien', 1, GETDATE());
                END;
            ");

            // ====================================================================================
            // PHẦN 4: SEED 30 HỌC VIÊN KÈM TÀI KHOẢN VÀ HỒ SƠ (Mật khẩu mặc định: User@123)
            // ====================================================================================
            migrationBuilder.Sql(@"
                DECLARE @tkId int;

                -- 1. sv_nguyenvana
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_nguyenvana')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_nguyenvana', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Nguyễn Văn An', 'an.nv@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_nguyenvana';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Nguyễn Văn An', '2003-05-15', N'Nam', '0987654321', 'an.nv@gmail.com', N'Hà Nội', N'Đại học', GETDATE(), 1);

                -- 2. sv_tranthib
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_tranthib')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_tranthib', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Trần Thị Bích', 'bich.tt@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_tranthib';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Trần Thị Bích', '2004-08-20', N'Nữ', '0912345671', 'bich.tt@gmail.com', N'Nam Định', N'Đại học', GETDATE(), 1);

                -- 3. sv_leducchinh
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_leducchinh')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_leducchinh', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Lê Đức Chính', 'chinh.ld@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_leducchinh';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Lê Đức Chính', '2003-03-12', N'Nam', '0923456782', 'chinh.ld@gmail.com', N'Hải Phòng', N'Cử nhân', GETDATE(), 1);

                -- 4. sv_phamminhdang
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_phamminhdang')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_phamminhdang', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Phạm Minh Đăng', 'dang.pm@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_phamminhdang';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Phạm Minh Đăng', '2004-11-05', N'Nam', '0934567893', 'dang.pm@gmail.com', N'Bắc Ninh', N'Cao đẳng', GETDATE(), 1);

                -- 5. sv_hoangthuha
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_hoangthuha')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_hoangthuha', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Hoàng Thu Hà', 'ha.ht@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_hoangthuha';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Hoàng Thu Hà', '2004-09-18', N'Nữ', '0945678904', 'ha.ht@gmail.com', N'Hà Nam', N'Đại học', GETDATE(), 1);

                -- 6. sv_vuthikimlien
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_vuthikimlien')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_vuthikimlien', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Vũ Thị Kim Liên', 'lien.vtk@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_vuthikimlien';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Vũ Thị Kim Liên', '2003-02-22', N'Nữ', '0956789015', 'lien.vtk@gmail.com', N'Thái Bình', N'Đại học', GETDATE(), 1);

                -- 7. sv_dangquanghuy
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_dangquanghuy')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_dangquanghuy', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Đặng Quang Huy', 'huy.dq@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_dangquanghuy';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Đặng Quang Huy', '2004-07-14', N'Nam', '0967890126', 'huy.dq@gmail.com', N'Hưng Yên', N'Đại học', GETDATE(), 1);

                -- 8. sv_ngothimyhuyen
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_ngothimyhuyen')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_ngothimyhuyen', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Ngô Thị Mỹ Huyền', 'huyen.ntm@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_ngothimyhuyen';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Ngô Thị Mỹ Huyền', '2003-10-30', N'Nữ', '0978901237', 'huyen.ntm@gmail.com', N'Vĩnh Phúc', N'Cử nhân', GETDATE(), 1);

                -- 9. sv_buituankiet
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_buituankiet')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_buituankiet', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Bùi Tuấn Kiệt', 'kiet.bt@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_buituankiet';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Bùi Tuấn Kiệt', '2004-04-08', N'Nam', '0989012348', 'kiet.bt@gmail.com', N'Quảng Ninh', N'Đại học', GETDATE(), 1);

                -- 10. sv_doandinhkhoa
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_doandinhkhoa')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_doandinhkhoa', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Đoàn Đình Khoa', 'khoa.dd@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_doandinhkhoa';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Đoàn Đình Khoa', '2003-12-25', N'Nam', '0911223349', 'khoa.dd@gmail.com', N'Hà Nội', N'Kỹ sư', GETDATE(), 1);

                -- 11. sv_duongthanhlong
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_duongthanhlong')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_duongthanhlong', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Dương Thành Long', 'long.dt@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_duongthanhlong';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Dương Thành Long', '2004-06-19', N'Nam', '0922334450', 'long.dt@gmail.com', N'Hải Dương', N'Đại học', GETDATE(), 1);

                -- 12. sv_hoangbaongoc
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_hoangbaongoc')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_hoangbaongoc', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Hoàng Bảo Ngọc', 'ngoc.hb@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_hoangbaongoc';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Hoàng Bảo Ngọc', '2004-01-03', N'Nữ', '0933445561', 'ngoc.hb@gmail.com', N'Nam Định', N'Đại học', GETDATE(), 1);

                -- 13. sv_lyminhnhat
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_lyminhnhat')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_lyminhnhat', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Lý Minh Nhật', 'nhat.lm@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_lyminhnhat';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Lý Minh Nhật', '2003-08-11', N'Nam', '0944556672', 'nhat.lm@gmail.com', N'Bắc Giang', N'Đại học', GETDATE(), 1);

                -- 14. sv_maithiphuong
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_maithiphuong')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_maithiphuong', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Mai Thị Phương', 'phuong.mt@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_maithiphuong';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Mai Thị Phương', '2004-04-27', N'Nữ', '0955667783', 'phuong.mt@gmail.com', N'Ninh Bình', N'Cử nhân', GETDATE(), 1);

                -- 15. sv_nguyenthanhphuc
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_nguyenthanhphuc')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_nguyenthanhphuc', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Nguyễn Thành Phúc', 'phuc.nt@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_nguyenthanhphuc';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Nguyễn Thành Phúc', '2003-09-09', N'Nam', '0966778894', 'phuc.nt@gmail.com', N'Hà Nội', N'Đại học', GETDATE(), 1);

                -- 16. sv_phanhoangquan
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_phanhoangquan')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_phanhoangquan', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Phan Hoàng Quân', 'quan.ph@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_phanhoangquan';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Phan Hoàng Quân', '2004-01-16', N'Nam', '0977889905', 'quan.ph@gmail.com', N'Thanh Hóa', N'Đại học', GETDATE(), 1);

                -- 17. sv_taquocson
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_taquocson')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_taquocson', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Tạ Quốc Sơn', 'son.tq@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_taquocson';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Tạ Quốc Sơn', '2003-10-04', N'Nam', '0988990016', 'son.tq@gmail.com', N'Nghệ An', N'Kỹ sư', GETDATE(), 1);

                -- 18. sv_trinhngocthai
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_trinhngocthai')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_trinhngocthai', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Trịnh Ngọc Thái', 'thai.tn@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_trinhngocthai';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Trịnh Ngọc Thái', '2004-05-21', N'Nam', '0919876527', 'thai.tn@gmail.com', N'Phú Thọ', N'Đại học', GETDATE(), 1);

                -- 19. sv_dinhtrungkien
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_dinhtrungkien')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_dinhtrungkien', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Đinh Trung Kiên', 'kien.dt@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_dinhtrungkien';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Đinh Trung Kiên', '2003-11-13', N'Nam', '0928765438', 'kien.dt@gmail.com', N'Hà Nội', N'Đại học', GETDATE(), 1);

                -- 20. sv_nguyenphuongthao
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_nguyenphuongthao')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_nguyenphuongthao', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Nguyễn Phương Thảo', 'thao.np@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_nguyenphuongthao';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Nguyễn Phương Thảo', '2004-03-07', N'Nữ', '0937654349', 'thao.np@gmail.com', N'Thái Nguyên', N'Đại học', GETDATE(), 1);

                -- 21. sv_vovanvuong
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_vovanvuong')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_vovanvuong', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Võ Văn Vượng', 'vuong.vv@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_vovanvuong';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Võ Văn Vượng', '2003-08-29', N'Nam', '0946543250', 'vuong.vv@gmail.com', N'Hà Tĩnh', N'Đại học', GETDATE(), 1);

                -- 22. sv_chuquangvinh
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_chuquangvinh')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_chuquangvinh', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Chu Quang Vinh', 'vinh.cq@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_chuquangvinh';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Chu Quang Vinh', '2004-12-17', N'Nam', '0955432161', 'vinh.cq@gmail.com', N'Nam Định', N'Cử nhân', GETDATE(), 1);

                -- 23. sv_nguyenthiyen
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_nguyenthiyen')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_nguyenthiyen', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Nguyễn Thị Yến', 'yen.nt@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_nguyenthiyen';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Nguyễn Thị Yến', '2003-06-24', N'Nữ', '0964321072', 'yen.nt@gmail.com', N'Hà Nội', N'Đại học', GETDATE(), 1);

                -- 24. sv_phamvietanh
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_phamvietanh')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_phamvietanh', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Phạm Việt Anh', 'anh.pv@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_phamvietanh';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Phạm Việt Anh', '2004-02-02', N'Nam', '0973210983', 'anh.pv@gmail.com', N'Bắc Ninh', N'Kỹ sư', GETDATE(), 1);

                -- 25. sv_tranthubaotram
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_tranthubaotram')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_tranthubaotram', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Trần Thị Bảo Trâm', 'tram.ttb@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_tranthubaotram';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Trần Thị Bảo Trâm', '2004-10-10', N'Nữ', '0982109894', 'tram.ttb@gmail.com', N'Hải Dương', N'Đại học', GETDATE(), 1);

                -- 26. sv_letuanhung
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_letuanhung')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_letuanhung', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Lê Tuấn Hùng', 'hung.lt@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_letuanhung';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Lê Tuấn Hùng', '2003-07-06', N'Nam', '0910987605', 'hung.lt@gmail.com', N'Hà Nam', N'Đại học', GETDATE(), 1);

                -- 27. sv_nguyenkhanhlinh
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_nguyenkhanhlinh')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_nguyenkhanhlinh', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Nguyễn Khánh Linh', 'linh.nk@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_nguyenkhanhlinh';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Nguyễn Khánh Linh', '2004-09-15', N'Nữ', '0929876516', 'linh.nk@gmail.com', N'Hà Nội', N'Đại học', GETDATE(), 1);

                -- 28. sv_hoangtrongnam
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_hoangtrongnam')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_hoangtrongnam', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Hoàng Trọng Nam', 'nam.ht@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_hoangtrongnam';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Hoàng Trọng Nam', '2003-03-28', N'Nam', '0938765427', 'nam.ht@gmail.com', N'Hưng Yên', N'Đại học', GETDATE(), 1);

                -- 29. sv_quachanhhoa
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_quachanhhoa')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_quachanhhoa', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Quách Ánh Hoa', 'hoa.qa@gmail.com', 'HocVien', 1, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_quachanhhoa';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Quách Ánh Hoa', '2004-11-19', N'Nữ', '0947654338', 'hoa.qa@gmail.com', N'Hải Phòng', N'Đại học', GETDATE(), 1);

                -- 30. sv_dogiakhoa (Tài khoản bị khóa để kiểm thử Test Case TC-07 Mục 22.1 Đề 16)
                IF NOT EXISTS (SELECT 1 FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_dogiakhoa')
                    INSERT INTO [TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
                    VALUES ('sv_dogiakhoa', 'EB1DF53123AF85F3822D21BC0B30B07A7211D453601516FEAAC8AFD9CC1A288F', N'Đỗ Gia Khóa', 'khoa.dg@gmail.com', 'HocVien', 0, GETDATE());
                SELECT @tkId = [MaTaiKhoan] FROM [TaiKhoan] WHERE [TenDangNhap] = 'sv_dogiakhoa';
                IF NOT EXISTS (SELECT 1 FROM [HocVien] WHERE [MaTaiKhoan] = @tkId)
                    INSERT INTO [HocVien] ([MaTaiKhoan], [HoTen], [NgaySinh], [GioiTinh], [SoDienThoai], [Email], [DiaChi], [TrinhDo], [NgayDangKy], [TrangThai])
                    VALUES (@tkId, N'Đỗ Gia Khóa', '2003-01-01', N'Nam', '0956543249', 'khoa.dg@gmail.com', N'Hà Nội', N'Đại học', GETDATE(), 0);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM [HocVien] WHERE [Email] IN (
                    'an.nv@gmail.com', 'bich.tt@gmail.com', 'chinh.ld@gmail.com', 'dang.pm@gmail.com', 'ha.ht@gmail.com',
                    'lien.vtk@gmail.com', 'huy.dq@gmail.com', 'huyen.ntm@gmail.com', 'kiet.bt@gmail.com', 'khoa.dd@gmail.com',
                    'long.dt@gmail.com', 'ngoc.hb@gmail.com', 'nhat.lm@gmail.com', 'phuong.mt@gmail.com', 'phuc.nt@gmail.com',
                    'quan.ph@gmail.com', 'son.tq@gmail.com', 'thai.tn@gmail.com', 'kien.dt@gmail.com', 'thao.np@gmail.com',
                    'vuong.vv@gmail.com', 'vinh.cq@gmail.com', 'yen.nt@gmail.com', 'anh.pv@gmail.com', 'tram.ttb@gmail.com',
                    'hung.lt@gmail.com', 'linh.nk@gmail.com', 'nam.ht@gmail.com', 'hoa.qa@gmail.com', 'khoa.dg@gmail.com'
                );

                DELETE FROM [TaiKhoan] WHERE [TenDangNhap] IN (
                    'admin_tonghop', 'nv_ketoan',
                    'sv_nguyenvana', 'sv_tranthib', 'sv_leducchinh', 'sv_phamminhdang', 'sv_hoangthuha',
                    'sv_vuthikimlien', 'sv_dangquanghuy', 'sv_ngothimyhuyen', 'sv_buituankiet', 'sv_doandinhkhoa',
                    'sv_duongthanhlong', 'sv_hoangbaongoc', 'sv_lyminhnhat', 'sv_maithiphuong', 'sv_nguyenthanhphuc',
                    'sv_phanhoangquan', 'sv_taquocson', 'sv_trinhngocthai', 'sv_dinhtrungkien', 'sv_nguyenphuongthao',
                    'sv_vovanvuong', 'sv_chuquangvinh', 'sv_nguyenthiyen', 'sv_phamvietanh', 'sv_tranthubaotram',
                    'sv_letuanhung', 'sv_nguyenkhanhlinh', 'sv_hoangtrongnam', 'sv_quachanhhoa', 'sv_dogiakhoa'
                );

                DELETE FROM [MonHoc] WHERE [TenMonHoc] IN (
                    N'Lập trình hướng đối tượng',
                    N'Cấu trúc dữ liệu và giải thuật',
                    N'Mạng máy tính',
                    N'Hệ điều hành',
                    N'An toàn thông tin',
                    N'Công nghệ Java',
                    N'Phân tích và thiết kế các hệ thống thông tin'
                );
            ");
        }
    }
}
