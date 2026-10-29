<div align="center">

# 🎓 Hệ Thống Quản Lý Khóa Học & Đăng Ký Học

**Quản lý đào tạo trực tuyến** — môn học, giảng viên, khóa học, đăng ký học và kết quả học tập, tất cả trong một hệ thống.

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-Azure-CC292B?logo=microsoftsqlserver&logoColor=white)
![Azure](https://img.shields.io/badge/Azure-App%20Service-0078D4?logo=microsoftazure&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?logo=bootstrap&logoColor=white)

### 🌐 Demo trực tuyến: [quanlykhoahoc.id.vn](https://quanlykhoahoc.id.vn)

</div>

---

## 📖 Giới thiệu

Website hỗ trợ trung tâm đào tạo quản lý toàn bộ quy trình: từ mở khóa học, phân công giảng viên, tiếp nhận học viên đăng ký, thu học phí cho đến nhập kết quả học tập và xem báo cáo thống kê. Học viên đăng ký khóa học trực tuyến và theo dõi đăng ký, kết quả của mình ngay trên web.

## ✨ Tính năng

- 🔐 Đăng nhập, phân quyền theo người dùng (quản trị viên · nhân viên đào tạo · học viên)
- 📚 Quản lý môn học, giảng viên và tài khoản hệ thống
- 🗓️ Quản lý khóa học: mở lớp, phân công giảng viên, sĩ số, trạng thái lớp
- 📝 Học viên đăng ký / hủy đăng ký khóa học trực tuyến
- ✅ Duyệt đăng ký, ghi danh tại quầy, theo dõi tình trạng học phí
- 🏆 Nhập kết quả học tập, học viên tra cứu điểm của mình
- 📊 Dashboard tổng quan và thống kê doanh thu, công nợ, lượt đăng ký

## 🛠️ Công nghệ

| Thành phần | Công nghệ |
|---|---|
| Backend | ASP.NET Core MVC (.NET 10), Entity Framework Core 10 |
| Cơ sở dữ liệu | SQL Server · Azure SQL Database |
| Giao diện | Razor View, Bootstrap 5, jQuery |
| Triển khai | Azure App Service · GitHub Actions CI/CD (tự deploy khi push lên `main`) |

## 🔑 Tài khoản demo

| Tên đăng nhập | Mật khẩu |
|---|---|
| `admin` | `Admin@123` |
| `nv_daotao` | `User@123` |
| `sv_nguyenvana` | `User@123` |

## 🚀 Chạy tại máy (Local)

1. Clone repo, mở file `.sln` bằng Visual Studio (yêu cầu .NET SDK 10).
2. Kiểm tra chuỗi kết nối `DefaultConnection` trong `appsettings.json` (mặc định LocalDB).
3. Chạy project (F5) — app tự tạo database (Migration) và nạp dữ liệu mẫu (Seed) ở lần chạy đầu.

---
