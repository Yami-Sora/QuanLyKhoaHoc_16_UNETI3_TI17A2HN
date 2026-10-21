// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Module 1: Hệ thống hồ sơ nhận diện và nội dung chuyên ngành động cho Môn học

using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Helpers
{
    public class MonHocProfile
    {
        public string CategoryName { get; set; } = "Công nghệ thông tin";
        public string ThemePrimaryColor { get; set; } = "#1d4ed8";
        public string ThemeBadgeBg { get; set; } = "#eff6ff";
        public string ThemeGradient { get; set; } = "linear-gradient(135deg, #e0f2fe 0%, #edf8ff 45%, #f0fdf4 100%)";
        public string ThemeBorderColor { get; set; } = "#bae6fd";
        public string BrandBoxBg { get; set; } = "linear-gradient(135deg, #7c3aed 0%, #68217a 100%)";
        public string BrandBoxShadow { get; set; } = "rgba(124, 58, 237, 0.35)";

        // Gradient 2 tông màu và màu nhấn đồng bộ (theo ảnh mẫu)
        public string ColorC1 { get; set; } = "#3b82f6";
        public string ColorC2 { get; set; } = "#8b5cf6";
        public string ColorAccent { get; set; } = "#2563eb";
        public string IconWhiteSvg { get; set; } = "";
        // Banner nền riêng theo chủ đề nhóm môn (để trống = xoay vòng theo mã môn học)
        public string BannerFile { get; set; } = "";

        // SVG Biểu tượng môn học (Hình vuông bo góc)
        public string BrandLogoSvg { get; set; } = "";

        // SVG Tranh minh họa công nghệ bên phải banner
        public string HeroIllustrationSvg { get; set; } = "";

        // 3 Gạch đầu dòng mục tiêu chuẩn đầu ra
        public List<string> Objectives { get; set; } = new();

        // 3 Module nội dung chính
        public List<ModuleItem> Modules { get; set; } = new();

        // Danh sách công nghệ thực hành (Tech Chips)
        public List<TechChip> TechStack { get; set; } = new();

        // Giảng viên chuyên ngành phụ trách mặc định nếu chưa gán
        public string LecturerName { get; set; } = "ThS. Nguyễn Văn An";
        public string LecturerHocVi { get; set; } = "ThS.";
        public string LecturerDept { get; set; } = "Khoa Công nghệ thông tin";
        public string LecturerEmail { get; set; } = "nguyenvanan@uneti.edu.vn";
        public string LecturerPhone { get; set; } = "0987 654 321";
        public string LecturerQuote { get; set; } = "Kiến thức hôm nay – Thành công ngày mai";
        public string LecturerBio { get; set; } = "Giảng viên có nhiều năm kinh nghiệm giảng dạy và nghiên cứu chuyên sâu.";

        // Tài liệu tham khảo
        public List<DocItem> ReferenceDocs { get; set; } = new();
    }

    public class ModuleItem
    {
        public string Number { get; set; } = "01";
        public string BadgeColor { get; set; } = "#a855f7";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public List<string> Bullets { get; set; } = new();
    }

    public class TechChip
    {
        public string Name { get; set; } = "";
        public string IconClass { get; set; } = "bi bi-check2";
        public string ColorClass { get; set; } = "text-primary";
    }

    public class DocItem
    {
        public string Title { get; set; } = "";
        public string Meta { get; set; } = "";
        public string IconClass { get; set; } = "bi bi-file-earmark-pdf-fill";
        public string BadgeColor { get; set; } = "bg-danger-subtle text-danger";
    }

    public static class MonHocProfileHelper
    {
        public static MonHocProfile GetProfile(MonHoc monHoc)
        {
            var p = new MonHocProfile();
            string name = monHoc.TenMonHoc ?? "";

            // 1. NHÓM LẬP TRÌNH C# & .NET CORE
            if (name.Contains("C#", StringComparison.OrdinalIgnoreCase) || name.Contains(".NET", StringComparison.OrdinalIgnoreCase))
            {
                p.CategoryName = "Lập trình Web & Phần mềm .NET";
                p.ThemePrimaryColor = "#7c3aed";
                p.ThemeGradient = "linear-gradient(135deg, #e0f2fe 0%, #edf8ff 45%, #f0fdf4 100%)";
                p.ThemeBorderColor = "#bae6fd";
                p.BrandBoxBg = "linear-gradient(135deg, #7c3aed 0%, #68217a 100%)";
                p.BrandBoxShadow = "rgba(124, 58, 237, 0.35)";

                p.ColorC1 = "#3b82f6";
                p.ColorC2 = "#8b5cf6";
                p.ColorAccent = "#2563eb";
                p.IconWhiteSvg = @"<svg width=""56"" height=""56"" viewBox=""0 0 24 24"" fill=""white""><text x=""12"" y=""15.8"" font-family=""Arial, Helvetica, sans-serif"" font-size=""9.5"" font-weight=""800"" text-anchor=""middle"" fill=""white"">C#</text></svg>";
                p.BannerFile = "detail-banner-0.webp";

                p.BrandLogoSvg = @"
                    <svg width=""48"" height=""48"" viewBox=""0 0 128 128"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                        <path d=""M116.5 64c0 29-23.5 52.5-52.5 52.5S11.5 93 11.5 64 35 11.5 64 11.5 116.5 35 116.5 64z"" fill=""#68217A""/>
                        <path d=""M64 18.5c25.1 0 45.5 20.4 45.5 45.5S89.1 109.5 64 109.5 18.5 89.1 18.5 64 38.9 18.5 64 18.5z"" stroke=""#FFFFFF"" stroke-width=""6""/>
                        <path d=""M68 45c-4.2-3.8-9.8-6-16-6-13.8 0-25 11.2-25 25s11.2 25 25 25c6.2 0 11.8-2.2 16-6l-6.5-7.8c-2.5 2.3-5.8 3.8-9.5 3.8-7.7 0-14-6.3-14-14s6.3-14 14-14c3.7 0 7 1.5 9.5 3.8L68 45z"" fill=""#FFFFFF""/>
                        <path d=""M78 48h4v8h8v4h-8v12h8v4h-8v8h-4v-8h-10v8h-4v-8h-4v-4h4V60h-4v-4h4v-8h4v8h10v-8zm-10 12v12h10V60H68z"" fill=""#FFFFFF""/>
                    </svg>";

                p.HeroIllustrationSvg = "/images/hero-csharp-art.svg";

                p.Objectives = new List<string>
                {
                    "Trang bị kiến thức cơ bản và nâng cao về lập trình C# 13 và nền tảng .NET Core hiện đại.",
                    "Rèn luyện kỹ năng xây dựng ứng dụng web, ứng dụng doanh nghiệp và thao tác chuyên sâu với cơ sở dữ liệu qua Entity Framework Core.",
                    "Giúp sinh viên có khả năng phát triển và triển khai các ứng dụng thực tế theo chuẩn kiến trúc MVC và RESTful Web API."
                };

                p.Modules = new List<ModuleItem>
                {
                    new ModuleItem { Number = "01", BadgeColor = "#a855f7", Title = "Lập trình C# cơ bản & OOP", Description = "Ngôn ngữ C#, cấu trúc chương trình, hướng đối tượng và LINQ to Objects.", Bullets = new() { "Cú pháp C# 13, cấu trúc dữ liệu và xử lý ngoại lệ.", "Lập trình OOP, Interfaces và Generic Collections.", "Biểu thức Lambda, Delegates, Events và LINQ." } },
                    new ModuleItem { Number = "02", BadgeColor = "#0284c7", Title = "ASP.NET Core MVC", Description = "Xây dựng ứng dụng web theo mô hình MVC, Dependency Injection và bảo mật Session.", Bullets = new() { "Routing, Controllers, Razor Views và Tag Helpers.", "Dependency Injection và Middleware Pipeline.", "Session, Cookie Authentication và Authorization." } },
                    new ModuleItem { Number = "03", BadgeColor = "#f59e0b", Title = "Entity Framework Core", Description = "Làm việc với cơ sở dữ liệu Code-First, Migrations và quản lý dữ liệu giao dịch.", Bullets = new() { "Tiếp cận Code-First và cấu hình DbContext.", "EF Core Migrations và tự động Seed dữ liệu.", "Truy vấn LINQ to Entities và tối ưu hiệu năng." } }
                };

                p.TechStack = new List<TechChip>
                {
                    new TechChip { Name = "C# 13", IconClass = "bi bi-filetype-cs", ColorClass = "text-primary" },
                    new TechChip { Name = "ASP.NET Core MVC", IconClass = "bi bi-boxes", ColorClass = "text-info" },
                    new TechChip { Name = "SQL Server", IconClass = "bi bi-database", ColorClass = "text-warning" },
                    new TechChip { Name = "Entity Framework Core", IconClass = "bi bi-layers", ColorClass = "text-primary" },
                    new TechChip { Name = "Visual Studio 2022", IconClass = "bi bi-terminal", ColorClass = "text-secondary" },
                    new TechChip { Name = "Git / GitHub", IconClass = "bi bi-git", ColorClass = "text-danger" }
                };

                p.LecturerName = "Nguyễn Văn An";
                p.LecturerHocVi = "ThS.";
                p.LecturerDept = "Khoa Công nghệ thông tin";
                p.LecturerEmail = "nguyenvanan@uneti.edu.vn";
                p.LecturerPhone = "0987 654 321";
            }
            // 2. NHÓM LẬP TRÌNH HƯỚNG ĐỐI TƯỢNG (OOP)
            else if (name.Contains("Hướng đối tượng", StringComparison.OrdinalIgnoreCase) || name.Contains("OOP", StringComparison.OrdinalIgnoreCase))
            {
                p.CategoryName = "Lập trình cốt lõi & Cấu trúc phần mềm";
                p.ThemePrimaryColor = "#ea580c";
                p.ThemeGradient = "linear-gradient(135deg, #fff7ed 0%, #ffedd5 45%, #f0fdf4 100%)";
                p.ThemeBorderColor = "#fed7aa";
                p.BrandBoxBg = "linear-gradient(135deg, #f97316 0%, #c2410c 100%)";
                p.BrandBoxShadow = "rgba(249, 115, 22, 0.35)";

                p.ColorC1 = "#f472b6";
                p.ColorC2 = "#a855f7";
                p.ColorAccent = "#be185d";
                p.IconWhiteSvg = @"<svg width=""56"" height=""56"" viewBox=""0 0 24 24"" fill=""white""><rect x=""3"" y=""3"" width=""7.5"" height=""7.5"" rx=""2""/><rect x=""13.5"" y=""3"" width=""7.5"" height=""7.5"" rx=""2""/><rect x=""3"" y=""13.5"" width=""7.5"" height=""7.5"" rx=""2""/><rect x=""13.5"" y=""13.5"" width=""7.5"" height=""7.5"" rx=""2""/></svg>";

                p.BrandLogoSvg = @"
                    <svg width=""48"" height=""48"" viewBox=""0 0 128 128"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                        <rect x=""14"" y=""14"" width=""100"" height=""100"" rx=""24"" fill=""#EA580C""/>
                        <!-- 3D Box isometric symbol representing Objects & Classes -->
                        <path d=""M64 28 L98 48 L64 68 L30 48 Z"" fill=""#FDBA74""/>
                        <path d=""M30 48 L64 68 L64 104 L30 84 Z"" fill=""#FFFFFF""/>
                        <path d=""M98 48 L64 68 L64 104 L98 84 Z"" fill=""#FED7AA""/>
                        <circle cx=""64"" cy=""66"" r=""6"" fill=""#C2410C""/>
                    </svg>";

                p.Objectives = new List<string>
                {
                    "Làm chủ 4 trụ cột cơ bản của lập trình hướng đối tượng: Đóng gói (Encapsulation), Kế thừa (Inheritance), Đa hình (Polymorphism) và Trừu tượng (Abstraction).",
                    "Rèn luyện tư duy phân tích thiết kế phần mềm theo mô hình đối tượng, quản lý bộ nhớ, con trỏ và tái sử dụng mã nguồn hiệu quả.",
                    "Ứng dụng các nguyên lý SOLID và Design Patterns kinh điển để phát triển các ứng dụng phần mềm module hóa cao và dễ mở rộng."
                };

                p.Modules = new List<ModuleItem>
                {
                    new ModuleItem { Number = "01", BadgeColor = "#ea580c", Title = "Lớp, Đối tượng & Đóng gói", Description = "Tư duy thiết kế Class, Object, Constructors, Destructors và phạm vi truy cập dữ liệu.", Bullets = new() { "Khái niệm Class, Object và Instance trong bộ nhớ.", "Hàm khởi tạo (Constructor) và Hàm hủy (Destructor).", "Đóng gói dữ liệu và thuộc tính Get/Set an toàn." } },
                    new ModuleItem { Number = "02", BadgeColor = "#0284c7", Title = "Kế thừa & Tính Đa hình", Description = "Kế thừa đơn/đa, hàm ảo (Virtual Functions), Override và trừu tượng hóa qua Interface.", Bullets = new() { "Cơ chế kế thừa và tái sử dụng mã nguồn.", "Hàm ảo, Bảng phương thức ảo (vtable) và Overriding.", "Lớp trừu tượng (Abstract Class) và Pure Virtual/Interface." } },
                    new ModuleItem { Number = "03", BadgeColor = "#10b981", Title = "Nguyên lý SOLID & Design Patterns", Description = "Áp dụng 5 nguyên tắc SOLID và các mẫu thiết kế Creational, Structural, Behavioral.", Bullets = new() { "5 Nguyên tắc SOLID trong phát triển phần mềm.", "Design Patterns phổ biến: Singleton, Factory, Observer.", "Refactoring mã nguồn và thiết kế sơ đồ lớp UML." } }
                };

                p.TechStack = new List<TechChip>
                {
                    new TechChip { Name = "C++ 20", IconClass = "bi bi-filetype-cpp", ColorClass = "text-danger" },
                    new TechChip { Name = "Java Core", IconClass = "bi bi-filetype-java", ColorClass = "text-warning" },
                    new TechChip { Name = "OOP Principles", IconClass = "bi bi-boxes", ColorClass = "text-primary" },
                    new TechChip { Name = "Design Patterns", IconClass = "bi bi-diagram-3", ColorClass = "text-success" },
                    new TechChip { Name = "UML Designer", IconClass = "bi bi-bounding-box", ColorClass = "text-info" },
                    new TechChip { Name = "Git / GitHub", IconClass = "bi bi-git", ColorClass = "text-secondary" }
                };

                p.LecturerName = "Nguyễn Hoàng Long";
                p.LecturerHocVi = "TS.";
                p.LecturerDept = "Khoa Công nghệ thông tin";
                p.LecturerEmail = "nhlong@uneti.edu.vn";
                p.LecturerPhone = "0912 345 678";
            }
            // 3. NHÓM CƠ SỞ DỮ LIỆU & SQL
            else if (name.Contains("Cơ sở dữ liệu", StringComparison.OrdinalIgnoreCase) || name.Contains("SQL", StringComparison.OrdinalIgnoreCase) || name.Contains("Database", StringComparison.OrdinalIgnoreCase))
            {
                p.CategoryName = "Cơ sở dữ liệu & Quản trị Hệ thống";
                p.ThemePrimaryColor = "#1d4ed8";
                p.ThemeGradient = "linear-gradient(135deg, #e0f2fe 0%, #eff6ff 45%, #ecfeff 100%)";
                p.ThemeBorderColor = "#bfdbfe";
                p.BrandBoxBg = "linear-gradient(135deg, #1d4ed8 0%, #1e40af 100%)";
                p.BrandBoxShadow = "rgba(29, 78, 216, 0.35)";

                p.ColorC1 = "#a78bfa";
                p.ColorC2 = "#ec4899";
                p.ColorAccent = "#7c3aed";
                p.IconWhiteSvg = @"<svg width=""56"" height=""56"" viewBox=""0 0 24 24"" fill=""none"" stroke=""white"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round""><ellipse cx=""12"" cy=""5"" rx=""9"" ry=""3""/><path d=""M3 5v14c0 1.66 4 3 9 3s9-1.34 9-3V5""/><path d=""M3 12c0 1.66 4 3 9 3s9-1.34 9-3""/></svg>";
                p.BannerFile = "detail-banner-1.webp";

                p.BrandLogoSvg = @"
                    <svg width=""48"" height=""48"" viewBox=""0 0 128 128"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                        <rect x=""14"" y=""14"" width=""100"" height=""100"" rx=""24"" fill=""#1D4ED8""/>
                        <!-- Database Cylinders -->
                        <ellipse cx=""64"" cy=""38"" rx=""32"" ry=""12"" fill=""#E0F2FE""/>
                        <path d=""M32 38 V60 C32 67, 96 67, 96 60 V38 Z"" fill=""#BAE6FD""/>
                        <path d=""M32 60 V82 C32 89, 96 89, 96 82 V60 Z"" fill=""#7DD3FC""/>
                        <path d=""M32 82 V96 C32 103, 96 103, 96 96 V82 Z"" fill=""#38BDF8""/>
                    </svg>";

                p.Objectives = new List<string>
                {
                    "Nắm vững lý thuyết mô hình CSDL quan hệ, thiết kế sơ đồ thực thể liên kết (ERD) và các dạng chuẩn hóa 1NF - 3NF/BCNF.",
                    "Thành thạo lập trình truy vấn T-SQL nâng cao, Stored Procedure, Function, Trigger và quản trị giao dịch ACID an toàn.",
                    "Làm chủ kỹ thuật phân tích chỉ mục Indexing, tối ưu hóa câu lệnh truy vấn và sao lưu phục hồi dữ liệu trong doanh nghiệp."
                };

                p.Modules = new List<ModuleItem>
                {
                    new ModuleItem { Number = "01", BadgeColor = "#1d4ed8", Title = "Mô hình Dữ liệu & Chuẩn hóa", Description = "Thiết kế ERD, lược đồ bảng, khóa chính, khóa ngoại và các dạng chuẩn 1NF - 3NF.", Bullets = new() { "Phân tích yêu cầu và thiết kế lược đồ ERD.", "Chuyển đổi sang mô hình CSDL quan hệ.", "Quy chuẩn hóa 1NF, 2NF, 3NF và BCNF loại bỏ dư thừa." } },
                    new ModuleItem { Number = "02", BadgeColor = "#0d9488", Title = "Truy vấn T-SQL Nâng cao", Description = "Truy vấn phức tạp với JOIN, GROUP BY, Subqueries, Stored Procedures và Triggers.", Bullets = new() { "Kỹ thuật viết câu lệnh SELECT phức tạp đa bảng.", "Lập trình Stored Procedure, Function và Views.", "Trigger tự động kiểm tra toàn vẹn dữ liệu." } },
                    new ModuleItem { Number = "03", BadgeColor = "#f59e0b", Title = "Quản trị & Tối ưu hóa CSDL", Description = "Chỉ mục Indexing, phân tích Execution Plan, sao lưu, phục hồi và giao dịch Transaction.", Bullets = new() { "Giao dịch Transaction và tính chất ACID.", "Chỉ mục Clustered/Non-Clustered tối ưu tốc độ.", "Kế hoạch Backup và Disaster Recovery." } }
                };

                p.TechStack = new List<TechChip>
                {
                    new TechChip { Name = "SQL Server 2022", IconClass = "bi bi-database", ColorClass = "text-primary" },
                    new TechChip { Name = "T-SQL", IconClass = "bi bi-code-slash", ColorClass = "text-info" },
                    new TechChip { Name = "SSMS Studio", IconClass = "bi bi-terminal", ColorClass = "text-secondary" },
                    new TechChip { Name = "ERD Modeling", IconClass = "bi bi-diagram-3", ColorClass = "text-warning" },
                    new TechChip { Name = "Indexing & Tuning", IconClass = "bi bi-speedometer2", ColorClass = "text-success" },
                    new TechChip { Name = "Git", IconClass = "bi bi-git", ColorClass = "text-danger" }
                };

                p.LecturerName = "Trần Thị Mai Lan";
                p.LecturerHocVi = "ThS.";
                p.LecturerDept = "Khoa Công nghệ thông tin";
                p.LecturerEmail = "ttmlan@uneti.edu.vn";
                p.LecturerPhone = "0923 456 789";
            }
            // 4. NHÓM WEB FRONTEND & REACT
            else if (name.Contains("React", StringComparison.OrdinalIgnoreCase) || name.Contains("Frontend", StringComparison.OrdinalIgnoreCase) || name.Contains("Web", StringComparison.OrdinalIgnoreCase))
            {
                p.CategoryName = "Phát triển Web Hiện đại & Frontend";
                p.ThemePrimaryColor = "#0e7490";
                p.ThemeGradient = "linear-gradient(135deg, #ecfeff 0%, #f0fdfa 45%, #eff6ff 100%)";
                p.ThemeBorderColor = "#a5f3fc";
                p.BrandBoxBg = "linear-gradient(135deg, #0e7490 0%, #155e75 100%)";
                p.BrandBoxShadow = "rgba(14, 116, 144, 0.35)";

                p.ColorC1 = "#f59e0b";
                p.ColorC2 = "#fb923c";
                p.ColorAccent = "#c2410c";
                p.IconWhiteSvg = @"<svg width=""56"" height=""56"" viewBox=""0 0 24 24"" fill=""none"" stroke=""white"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round""><rect x=""3"" y=""3"" width=""18"" height=""18"" rx=""2""/><path d=""M3 9h18""/><circle cx=""6"" cy=""6"" r=""0.8"" fill=""white""/><circle cx=""9"" cy=""6"" r=""0.8"" fill=""white""/><circle cx=""8"" cy=""15"" r=""2""/><path d=""M14 13h4""/><path d=""M14 17h3""/></svg>";

                p.BrandLogoSvg = @"
                    <svg width=""48"" height=""48"" viewBox=""0 0 128 128"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                        <rect x=""14"" y=""14"" width=""100"" height=""100"" rx=""24"" fill=""#0891B2""/>
                        <!-- React Atom -->
                        <ellipse cx=""64"" cy=""64"" rx=""34"" ry=""14"" stroke=""#FFFFFF"" stroke-width=""5"" transform=""rotate(0 64 64)""/>
                        <ellipse cx=""64"" cy=""64"" rx=""34"" ry=""14"" stroke=""#FFFFFF"" stroke-width=""5"" transform=""rotate(60 64 64)""/>
                        <ellipse cx=""64"" cy=""64"" rx=""34"" ry=""14"" stroke=""#FFFFFF"" stroke-width=""5"" transform=""rotate(120 64 64)""/>
                        <circle cx=""64"" cy=""64"" r=""7"" fill=""#FFFFFF""/>
                    </svg>";

                p.Objectives = new List<string>
                {
                    "Làm chủ cú pháp JavaScript ES6+, cơ chế Virtual DOM, JSX và kiến trúc Single Page Application (SPA).",
                    "Sử dụng thành thạo React Hooks (useState, useEffect, Custom Hooks) và quản trị State toàn cục với Redux Toolkit.",
                    "Thiết kế giao diện Responsive chuẩn UI/UX với Tailwind CSS và tích hợp trơn tru với các dịch vụ RESTful API."
                };

                p.Modules = new List<ModuleItem>
                {
                    new ModuleItem { Number = "01", BadgeColor = "#0891b2", Title = "ES6+ & Nền tảng ReactJS", Description = "Cú pháp ES6, JSX, Component Lifecycle, Virtual DOM và kiến trúc SPA.", Bullets = new() { "JavaScript hiện đại: Destructuring, Spread, Promise/Async.", "Tư duy chia nhỏ Component và luồng Props.", "Cơ chế Render và Reconciliation của React." } },
                    new ModuleItem { Number = "02", BadgeColor = "#3b82f6", Title = "React Hooks & Quản lý State", Description = "Làm chủ useState, useEffect, useRef, Context API và Redux Toolkit toàn cục.", Bullets = new() { "Hook căn bản: useState, useEffect, useCallback, useMemo.", "Quản lý trạng thái chia sẻ với React Context.", "Redux Toolkit: Slices, Thunks và Async Actions." } },
                    new ModuleItem { Number = "03", BadgeColor = "#10b981", Title = "Tích hợp REST API & Deployment", Description = "Kết nối Backend qua Axios, React Router điều hướng và tối ưu hóa đóng gói Vite.", Bullets = new() { "Điều hướng nhiều trang với React Router v6.", "Gọi API, xử lý Loading, Error và Token Authentication.", "Tối ưu hóa Bundle Size và triển khai lên Production." } }
                };

                p.TechStack = new List<TechChip>
                {
                    new TechChip { Name = "React 19", IconClass = "bi bi-filetype-jsx", ColorClass = "text-info" },
                    new TechChip { Name = "Redux Toolkit", IconClass = "bi bi-boxes", ColorClass = "text-primary" },
                    new TechChip { Name = "Tailwind CSS", IconClass = "bi bi-palette", ColorClass = "text-cyan" },
                    new TechChip { Name = "TypeScript", IconClass = "bi bi-filetype-tsx", ColorClass = "text-primary" },
                    new TechChip { Name = "Vite / VS Code", IconClass = "bi bi-terminal", ColorClass = "text-secondary" },
                    new TechChip { Name = "Git / GitHub", IconClass = "bi bi-git", ColorClass = "text-danger" }
                };

                p.LecturerName = "Lê Tuấn Anh";
                p.LecturerHocVi = "ThS.";
                p.LecturerDept = "Khoa Công nghệ thông tin";
                p.LecturerEmail = "ltanh@uneti.edu.vn";
                p.LecturerPhone = "0934 567 890";
            }
            // 5. NHÓM DI ĐỘNG & FLUTTER
            else if (name.Contains("Flutter", StringComparison.OrdinalIgnoreCase) || name.Contains("Di động", StringComparison.OrdinalIgnoreCase) || name.Contains("Mobile", StringComparison.OrdinalIgnoreCase))
            {
                p.CategoryName = "Lập trình Ứng dụng Di động Đa nền tảng";
                p.ThemePrimaryColor = "#4f46e5";
                p.ThemeGradient = "linear-gradient(135deg, #e0e7ff 0%, #eef2ff 45%, #eff6ff 100%)";
                p.ThemeBorderColor = "#c7d2fe";
                p.BrandBoxBg = "linear-gradient(135deg, #4f46e5 0%, #3730a3 100%)";
                p.BrandBoxShadow = "rgba(79, 70, 229, 0.35)";

                p.ColorC1 = "#2dd4bf";
                p.ColorC2 = "#38bdf8";
                p.ColorAccent = "#0f766e";
                p.IconWhiteSvg = @"<svg width=""56"" height=""56"" viewBox=""0 0 24 24"" fill=""none"" stroke=""white"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round""><rect width=""14"" height=""20"" x=""5"" y=""2"" rx=""3"" ry=""3""/><path d=""M12 18h.01""/></svg>";
                p.BannerFile = "detail-banner-4.webp";

                p.BrandLogoSvg = @"
                    <svg width=""48"" height=""48"" viewBox=""0 0 128 128"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                        <rect x=""14"" y=""14"" width=""100"" height=""100"" rx=""24"" fill=""#4F46E5""/>
                        <!-- Mobile Flutter Icon -->
                        <path d=""M76 24 L34 66 L48 80 L90 38 Z"" fill=""#FFFFFF""/>
                        <path d=""M62 80 L48 94 L62 108 L76 94 Z"" fill=""#A5B4FC""/>
                        <path d=""M76 66 L62 80 L76 94 L90 80 Z"" fill=""#E0E7FF""/>
                    </svg>";

                p.Objectives = new List<string>
                {
                    "Làm chủ ngôn ngữ lập trình Dart hướng đối tượng, cây Widget Flutter (Stateless & Stateful) và cơ chế Hot Reload.",
                    "Thiết kế giao diện ứng dụng chuẩn Material 3 & Cupertino mượt mà, quản lý trạng thái với Provider/Bloc Pattern.",
                    "Lập trình tương tác Backend qua REST API, lưu trữ dữ liệu cục bộ SQLite và đóng gói xuất bản ứng dụng lên App Store/Google Play."
                };

                p.Modules = new List<ModuleItem>
                {
                    new ModuleItem { Number = "01", BadgeColor = "#4f46e5", Title = "Ngôn ngữ Dart & Cây Widget", Description = "Cú pháp Dart, tư duy Widget trong Flutter, Layouts và Custom Animation.", Bullets = new() { "Ngôn ngữ Dart OOP, Null Safety và Async/Await.", "StatelessWidget, StatefulWidget và Vòng đời Widget.", "Hệ thống bố cục Column, Row, Stack và ListView." } },
                    new ModuleItem { Number = "02", BadgeColor = "#3b82f6", Title = "Quản lý State & Dữ liệu", Description = "Điều hướng Navigation 2.0, Provider, Bloc Pattern và lưu trữ dữ liệu SQLite.", Bullets = new() { "Quản lý State cục bộ và State toàn cục.", "Kiến trúc BLoC (Business Logic Component).", "Lưu trữ cấu hình SharedPreferences và CSDL SQLite." } },
                    new ModuleItem { Number = "03", BadgeColor = "#10b981", Title = "Tích hợp API & Triển khai App", Description = "Gọi dịch vụ REST API, tích hợp Firebase Auth/Push Notification và build APK/AAB.", Bullets = new() { "Kết nối HTTP/RESTful Web Service và xử lý JSON.", "Tích hợp dịch vụ Firebase Authentication và Cloud Database.", "Tối ưu hiệu năng Render và đóng gói ứng dụng phát hành." } }
                };

                p.TechStack = new List<TechChip>
                {
                    new TechChip { Name = "Flutter 3", IconClass = "bi bi-phone", ColorClass = "text-primary" },
                    new TechChip { Name = "Dart", IconClass = "bi bi-code-slash", ColorClass = "text-info" },
                    new TechChip { Name = "Android Studio", IconClass = "bi bi-terminal", ColorClass = "text-success" },
                    new TechChip { Name = "Bloc Pattern", IconClass = "bi bi-boxes", ColorClass = "text-warning" },
                    new TechChip { Name = "Firebase", IconClass = "bi bi-cloud-arrow-up", ColorClass = "text-danger" },
                    new TechChip { Name = "Git", IconClass = "bi bi-git", ColorClass = "text-secondary" }
                };

                p.LecturerName = "Lê Tuấn Anh";
                p.LecturerHocVi = "ThS.";
                p.LecturerDept = "Khoa Công nghệ thông tin";
                p.LecturerEmail = "ltanh@uneti.edu.vn";
                p.LecturerPhone = "0934 567 890";
            }
            // 6. NHÓM PHÂN TÍCH DỮ LIỆU & PYTHON / AI
            // Lưu ý: KHÔNG bắt theo từ "Dữ liệu" hay "AI" đứng một mình — môn "Cấu trúc dữ liệu..."
            // sẽ bị nhận nhầm vào nhóm này (nó thuộc nhóm 7 bên dưới), và "AI" còn xuất hiện trong
            // rất nhiều từ tiếng Việt khác (khai, mai, tương lai...). Chỉ bắt các cụm từ đặc trưng.
            else if (name.Contains("Python", StringComparison.OrdinalIgnoreCase) || name.Contains("Phân tích dữ liệu", StringComparison.OrdinalIgnoreCase) || name.Contains("Khoa học dữ liệu", StringComparison.OrdinalIgnoreCase) || name.Contains("Khai phá dữ liệu", StringComparison.OrdinalIgnoreCase) || name.Contains("Dữ liệu lớn", StringComparison.OrdinalIgnoreCase) || name.Contains("Big Data", StringComparison.OrdinalIgnoreCase) || name.Contains("Học máy", StringComparison.OrdinalIgnoreCase) || name.Contains("Machine Learning", StringComparison.OrdinalIgnoreCase) || name.Contains("Deep Learning", StringComparison.OrdinalIgnoreCase) || name.Contains("Trí tuệ nhân tạo", StringComparison.OrdinalIgnoreCase) || name.Contains(" AI ", StringComparison.OrdinalIgnoreCase) || name.StartsWith("AI ", StringComparison.OrdinalIgnoreCase) || name.EndsWith(" AI", StringComparison.OrdinalIgnoreCase))
            {
                p.CategoryName = "Khoa học Dữ liệu & Trí tuệ Nhân tạo";
                p.ThemePrimaryColor = "#b45309";
                p.ThemeGradient = "linear-gradient(135deg, #fef3c7 0%, #fffbeb 45%, #eff6ff 100%)";
                p.ThemeBorderColor = "#fde68a";
                p.BrandBoxBg = "linear-gradient(135deg, #b45309 0%, #92400e 100%)";
                p.BrandBoxShadow = "rgba(180, 83, 9, 0.35)";

                p.ColorC1 = "#10b981";
                p.ColorC2 = "#14b8a6";
                p.ColorAccent = "#047857";
                p.IconWhiteSvg = @"<svg width=""56"" height=""56"" viewBox=""0 0 24 24"" fill=""white""><path d=""M11.9 2c-3.1 0-4.9 1.4-4.9 3.2v2.4h5v.8H5.9C3.7 8.4 2 10.3 2 12.8s1.8 4.4 4.1 4.4h1.7v-2.3c0-1.8 1.6-3.3 3.5-3.3h5.1c1.5 0 2.8-1.2 2.8-2.8V5.2c0-1.8-1.8-3.2-4.9-3.2h-2.4zm-1.8 1.9c.5 0 .9.4.9.9s-.4.9-.9.9-.9-.4-.9-.9.4-.9.9-.9z""/><path d=""M12.1 22c3.1 0 4.9-1.4 4.9-3.2v-2.4h-5v-.8h6.1c2.2 0 3.9-1.9 3.9-4.4s-1.8-4.4-4.1-4.4h-1.7v2.3c0 1.8-1.6 3.3-3.5 3.3H7.6c-1.5 0-2.8 1.2-2.8 2.8v3.6c0 1.8 1.8 3.2 4.9 3.2h2.4zm1.8-1.9c-.5 0-.9-.4-.9-.9s.4-.9.9-.9.9.4.9.9-.4.9-.9.9z""/></svg>";
                p.BannerFile = "detail-banner-5.webp";

                p.BrandLogoSvg = @"
                    <svg width=""48"" height=""48"" viewBox=""0 0 128 128"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                        <rect x=""14"" y=""14"" width=""100"" height=""100"" rx=""24"" fill=""#D97706""/>
                        <!-- Python / Data Icon -->
                        <path d=""M64 30 C50 30 42 36 42 46 V54 H64 V58 H32 C22 58 18 68 18 78 C18 88 24 94 36 94 H44 V82 C44 72 52 66 64 66 H74 V54 C74 42 66 30 64 30 Z"" fill=""#FEF3C7""/>
                        <circle cx=""50"" cy=""40"" r=""4"" fill=""#B45309""/>
                        <path d=""M64 98 C78 98 86 92 86 82 V74 H64 V70 H96 C106 70 110 60 110 50 C110 40 104 34 92 34 H84 V46 C84 56 76 62 64 62 H54 V74 C54 86 62 98 64 98 Z"" fill=""#FFFFFF""/>
                        <circle cx=""78"" cy=""88"" r=""4"" fill=""#D97706""/>
                    </svg>";

                p.Objectives = new List<string>
                {
                    "Làm chủ cú pháp Python nâng cao, cấu trúc dữ liệu và xử lý tính toán số học ma trận với thư viện NumPy.",
                    "Thành thạo tiền xử lý, làm sạch và khai phá dữ liệu bảng (DataFrames) với thư viện Pandas và trực quan hóa qua Matplotlib/Seaborn.",
                    "Hiểu và áp dụng các mô hình Học máy cơ bản (Hồi quy tuyến tính, Cây quyết định, K-Means) vào phân tích dự báo thực tế."
                };

                p.Modules = new List<ModuleItem>
                {
                    new ModuleItem { Number = "01", BadgeColor = "#d97706", Title = "Python & Tính toán NumPy", Description = "Cú pháp Python nâng cao, mảng đa chiều NumPy và đại số tuyến tính cơ bản.", Bullets = new() { "Python Data Structures: Lists, Tuples, Sets, Dictionaries.", "NumPy Arrays, Broadcasting và tính toán ma trận.", "Khai báo hàm, List Comprehensions và Generator." } },
                    new ModuleItem { Number = "02", BadgeColor = "#0284c7", Title = "Khai phá Dữ liệu với Pandas", Description = "Thao tác DataFrames, lọc, nhóm, xử lý Missing Data và trực quan hóa biểu đồ.", Bullets = new() { "Đọc/Ghi dữ liệu CSV, Excel, SQL vào Pandas.", "Làm sạch dữ liệu, xử lý giá trị Null và biến đổi kiểu.", "Vẽ biểu đồ phân tích xu hướng với Matplotlib và Seaborn." } },
                    new ModuleItem { Number = "03", BadgeColor = "#10b981", Title = "Nhập môn Học máy (ML)", Description = "Xây dựng pipeline dữ liệu và huấn luyện mô hình Scikit-Learn cơ bản.", Bullets = new() { "Quy trình huấn luyện và đánh giá mô hình Train/Test Split.", "Mô hình Hồi quy (Linear Regression) và Phân loại (Decision Tree).", "Đo lường độ chính xác Accuracy, Precision, Recall và F1-Score." } }
                };

                p.TechStack = new List<TechChip>
                {
                    new TechChip { Name = "Python 3.12", IconClass = "bi bi-filetype-py", ColorClass = "text-warning" },
                    new TechChip { Name = "Pandas", IconClass = "bi bi-table", ColorClass = "text-primary" },
                    new TechChip { Name = "NumPy", IconClass = "bi bi-calculator", ColorClass = "text-info" },
                    new TechChip { Name = "Matplotlib / Seaborn", IconClass = "bi bi-graph-up", ColorClass = "text-danger" },
                    new TechChip { Name = "Scikit-Learn", IconClass = "bi bi-cpu", ColorClass = "text-success" },
                    new TechChip { Name = "Jupyter Notebook", IconClass = "bi bi-journal-code", ColorClass = "text-secondary" }
                };

                p.LecturerName = "Phạm Minh Đức";
                p.LecturerHocVi = "TS.";
                p.LecturerDept = "Khoa Công nghệ thông tin";
                p.LecturerEmail = "pmduc@uneti.edu.vn";
                p.LecturerPhone = "0945 678 901";
            }
            // 7. CẤU TRÚC DỮ LIỆU VÀ GIẢI THUẬT
            else if (name.Contains("Cấu trúc dữ liệu", StringComparison.OrdinalIgnoreCase) || name.Contains("Giải thuật", StringComparison.OrdinalIgnoreCase))
            {
                p.CategoryName = "Giải thuật & Tư duy Lập trình Cốt lõi";
                p.ThemePrimaryColor = "#059669";
                p.ThemeGradient = "linear-gradient(135deg, #ecfdf5 0%, #f0fdf4 45%, #eff6ff 100%)";
                p.ThemeBorderColor = "#a7f3d0";
                p.BrandBoxBg = "linear-gradient(135deg, #059669 0%, #047857 100%)";
                p.BrandBoxShadow = "rgba(5, 150, 105, 0.35)";

                p.ColorC1 = "#06b6d4";
                p.ColorC2 = "#3b82f6";
                p.ColorAccent = "#0284c7";
                p.IconWhiteSvg = @"<svg width=""56"" height=""56"" viewBox=""0 0 24 24"" fill=""none"" stroke=""white"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round""><circle cx=""12"" cy=""5"" r=""3""/><circle cx=""6"" cy=""18"" r=""3""/><circle cx=""18"" cy=""18"" r=""3""/><path d=""M12 8v3""/><path d=""M12 11l-4.5 4.5""/><path d=""M12 11l4.5 4.5""/></svg>";

                p.BrandLogoSvg = @"
                    <svg width=""48"" height=""48"" viewBox=""0 0 128 128"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                        <rect x=""14"" y=""14"" width=""100"" height=""100"" rx=""24"" fill=""#059669""/>
                        <!-- Binary Tree Graph -->
                        <circle cx=""64"" cy=""36"" r=""10"" fill=""#FFFFFF""/>
                        <circle cx=""40"" cy=""66"" r=""9"" fill=""#A7F3D0""/>
                        <circle cx=""88"" cy=""66"" r=""9"" fill=""#A7F3D0""/>
                        <circle cx=""26"" cy=""94"" r=""8"" fill=""#ECFDF5""/>
                        <circle cx=""54"" cy=""94"" r=""8"" fill=""#ECFDF5""/>
                        <line x1=""64"" y1=""46"" x2=""40"" y2=""57"" stroke=""#FFFFFF"" stroke-width=""3""/>
                        <line x1=""64"" y1=""46"" x2=""88"" y2=""57"" stroke=""#FFFFFF"" stroke-width=""3""/>
                        <line x1=""40"" y1=""75"" x2=""26"" y2=""86"" stroke=""#A7F3D0"" stroke-width=""2.5""/>
                        <line x1=""40"" y1=""75"" x2=""54"" y2=""86"" stroke=""#A7F3D0"" stroke-width=""2.5""/>
                    </svg>";

                p.Objectives = new List<string>
                {
                    "Hiểu sâu cách phân tích độ phức tạp thời gian và không gian của thuật toán theo chuẩn ký hiệu Big-O.",
                    "Cài đặt và làm chủ các cấu trúc dữ liệu tuyến tính (Danh sách liên kết, Ngăn xếp, Hàng đợi) và phi tuyến (Cây nhị phân, Bảng băm, Đồ thị).",
                    "Thành thạo các thuật toán sắp xếp (QuickSort, MergeSort), tìm kiếm nhị phân và giải thuật đồ thị (BFS, DFS, Dijkstra)."
                };

                p.Modules = new List<ModuleItem>
                {
                    new ModuleItem { Number = "01", BadgeColor = "#059669", Title = "Phân tích Big-O & CTDL Tuyến tính", Description = "Độ phức tạp thuật toán, mảng động, Danh sách liên kết, Stack và Queue.", Bullets = new() { "Độ phức tạp thời gian/bộ nhớ và quy tắc Big-O.", "Singly/Doubly Linked List và thao tác chèn/xóa.", "Ứng dụng Stack trong đệ quy và Queue trong lập lịch." } },
                    new ModuleItem { Number = "02", BadgeColor = "#0284c7", Title = "Cây & Thuật toán Sắp xếp", Description = "Cây nhị phân tìm kiếm BST, Heap, bảng băm (Hash Table) và sắp xếp nâng cao.", Bullets = new() { "Binary Search Tree: Duyệt cây Pre/In/Post Order.", "Cơ chế Hash Function và xử lý xung đột băm.", "Thuật toán QuickSort, MergeSort và HeapSort (O(N log N))." } },
                    new ModuleItem { Number = "03", BadgeColor = "#ea580c", Title = "Đồ thị & Quy hoạch động", Description = "Biểu diễn đồ thị, duyệt BFS/DFS, thuật toán đường đi ngắn nhất và quy hoạch động.", Bullets = new() { "Ma trận kề, danh sách kề và duyệt đồ thị BFS/DFS.", "Thuật toán tìm đường đi ngắn nhất Dijkstra.", "Tư duy chia để trị và nhập môn Quy hoạch động (Dynamic Programming)." } }
                };

                p.TechStack = new List<TechChip>
                {
                    new TechChip { Name = "C++ / Java", IconClass = "bi bi-filetype-cpp", ColorClass = "text-success" },
                    new TechChip { Name = "Big-O Notation", IconClass = "bi bi-speedometer", ColorClass = "text-danger" },
                    new TechChip { Name = "Binary Trees", IconClass = "bi bi-diagram-3", ColorClass = "text-primary" },
                    new TechChip { Name = "Graph Algorithms", IconClass = "bi bi-bezier2", ColorClass = "text-info" },
                    new TechChip { Name = "Dynamic Programming", IconClass = "bi bi-lightning-charge", ColorClass = "text-warning" },
                    new TechChip { Name = "Git", IconClass = "bi bi-git", ColorClass = "text-secondary" }
                };

                p.LecturerName = "Phạm Minh Đức";
                p.LecturerHocVi = "TS.";
                p.LecturerDept = "Khoa Công nghệ thông tin";
                p.LecturerEmail = "pmduc@uneti.edu.vn";
                p.LecturerPhone = "0945 678 901";
            }
            // 8. NHÓM MẠNG MÁY TÍNH
            else if (name.Contains("Mạng máy tính", StringComparison.OrdinalIgnoreCase) || name.Contains("Network", StringComparison.OrdinalIgnoreCase))
            {
                p.CategoryName = "Mạng máy tính & Truyền thông dữ liệu";
                p.ThemePrimaryColor = "#0d9488";
                p.ThemeGradient = "linear-gradient(135deg, #ccfbf1 0%, #f0fdfa 45%, #eff6ff 100%)";
                p.ThemeBorderColor = "#99f6e4";
                p.BrandBoxBg = "linear-gradient(135deg, #0d9488 0%, #0f766e 100%)";
                p.BrandBoxShadow = "rgba(13, 148, 136, 0.35)";

                p.ColorC1 = "#14b8a6";
                p.ColorC2 = "#06b6d4";
                p.ColorAccent = "#0f766e";
                p.IconWhiteSvg = @"<svg width=""56"" height=""56"" viewBox=""0 0 24 24"" fill=""none"" stroke=""white"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round""><rect x=""2"" y=""3"" width=""20"" height=""14"" rx=""2""/><path d=""M6 21h12""/><path d=""M12 17v4""/></svg>";
                p.BannerFile = "detail-banner-3.webp";

                p.BrandLogoSvg = @"
                    <svg width=""48"" height=""48"" viewBox=""0 0 128 128"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                        <rect x=""14"" y=""14"" width=""100"" height=""100"" rx=""24"" fill=""#0D9488""/>
                        <line x1=""64"" y1=""42"" x2=""40"" y2=""84"" stroke=""#99F6E4"" stroke-width=""5""/>
                        <line x1=""64"" y1=""42"" x2=""88"" y2=""84"" stroke=""#99F6E4"" stroke-width=""5""/>
                        <line x1=""40"" y1=""84"" x2=""88"" y2=""84"" stroke=""#99F6E4"" stroke-width=""5""/>
                        <circle cx=""64"" cy=""38"" r=""12"" fill=""#FFFFFF""/>
                        <circle cx=""38"" cy=""88"" r=""12"" fill=""#CCFBF1""/>
                        <circle cx=""90"" cy=""88"" r=""12"" fill=""#CCFBF1""/>
                    </svg>";

                p.Objectives = new List<string>
                {
                    "Nắm vững mô hình tham chiếu OSI 7 tầng, bộ giao thức TCP/IP và cơ chế địa chỉ IP, subnetting trong mạng hiện đại.",
                    "Cấu hình và quản trị thiết bị mạng Cisco (Router, Switch), triển khai VLAN, định tuyến tĩnh/động và dịch vụ DNS/DHCP.",
                    "Hiểu nguyên lý an ninh mạng cơ bản, tường lửa và kỹ thuật giám sát, phân tích lưu lượng với Wireshark."
                };

                p.Modules = new List<ModuleItem>
                {
                    new ModuleItem { Number = "01", BadgeColor = "#0d9488", Title = "Mô hình OSI & Giao thức TCP/IP", Description = "Kiến trúc phân tầng mạng, địa chỉ IPv4/IPv6, chia subnet và đóng gói dữ liệu.", Bullets = new() { "Mô hình OSI 7 tầng và TCP/IP 4 tầng.", "Địa chỉ IP, Subnet Mask và kỹ thuật chia subnet.", "Giao thức ARP, ICMP, TCP/UDP và bắt tay 3 bước." } },
                    new ModuleItem { Number = "02", BadgeColor = "#0284c7", Title = "Thiết bị mạng & Định tuyến", Description = "Cấu hình Switch, Router, VLAN, định tuyến tĩnh và các giao thức RIP/OSPF.", Bullets = new() { "Cấu hình cơ bản thiết bị Cisco qua CLI.", "VLAN, Trunking và định tuyến giữa các VLAN.", "Định tuyến tĩnh và giao thức định tuyến động OSPF." } },
                    new ModuleItem { Number = "03", BadgeColor = "#f59e0b", Title = "Dịch vụ mạng & An ninh cơ bản", Description = "Triển khai DNS, DHCP, NAT và các biện pháp bảo vệ mạng nội bộ.", Bullets = new() { "Cài đặt và quản trị dịch vụ DNS/DHCP.", "Kỹ thuật NAT/PAT và chuyển tiếp cổng.", "Access Control List (ACL) và tường lửa cơ bản." } }
                };

                p.TechStack = new List<TechChip>
                {
                    new TechChip { Name = "Cisco Packet Tracer", IconClass = "bi bi-hdd-network", ColorClass = "text-success" },
                    new TechChip { Name = "TCP/IP", IconClass = "bi bi-diagram-3", ColorClass = "text-primary" },
                    new TechChip { Name = "DNS / DHCP", IconClass = "bi bi-globe", ColorClass = "text-info" },
                    new TechChip { Name = "Wireshark", IconClass = "bi bi-activity", ColorClass = "text-warning" },
                    new TechChip { Name = "Linux Networking", IconClass = "bi bi-terminal", ColorClass = "text-secondary" },
                    new TechChip { Name = "Git", IconClass = "bi bi-git", ColorClass = "text-danger" }
                };

                p.LecturerName = "Trần Thị Mai Lan";
                p.LecturerHocVi = "ThS.";
                p.LecturerDept = "Khoa Công nghệ thông tin";
                p.LecturerEmail = "ttmlan@uneti.edu.vn";
                p.LecturerPhone = "0923 456 789";
            }
            // 9. NHÓM HỆ ĐIỀU HÀNH
            else if (name.Contains("Hệ điều hành", StringComparison.OrdinalIgnoreCase) || name.Contains("Operating System", StringComparison.OrdinalIgnoreCase))
            {
                p.CategoryName = "Hệ điều hành & Quản trị hệ thống";
                p.ThemePrimaryColor = "#475569";
                p.ThemeGradient = "linear-gradient(135deg, #f1f5f9 0%, #f8fafc 45%, #eff6ff 100%)";
                p.ThemeBorderColor = "#cbd5e1";
                p.BrandBoxBg = "linear-gradient(135deg, #475569 0%, #1e293b 100%)";
                p.BrandBoxShadow = "rgba(71, 85, 105, 0.35)";

                p.ColorC1 = "#6366f1";
                p.ColorC2 = "#8b5cf6";
                p.ColorAccent = "#4f46e5";
                p.IconWhiteSvg = @"<svg width=""56"" height=""56"" viewBox=""0 0 24 24"" fill=""none"" stroke=""white"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round""><polyline points=""4 17 10 11 4 5""/><line x1=""12"" y1=""19"" x2=""20"" y2=""19""/></svg>";

                p.BrandLogoSvg = @"
                    <svg width=""48"" height=""48"" viewBox=""0 0 128 128"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                        <rect x=""14"" y=""14"" width=""100"" height=""100"" rx=""24"" fill=""#475569""/>
                        <rect x=""30"" y=""38"" width=""68"" height=""52"" rx=""10"" fill=""#1E293B""/>
                        <circle cx=""40"" cy=""50"" r=""4"" fill=""#F87171""/>
                        <circle cx=""52"" cy=""50"" r=""4"" fill=""#FBBF24""/>
                        <circle cx=""64"" cy=""50"" r=""4"" fill=""#34D399""/>
                        <path d=""M40 68 L52 76 L40 84"" stroke=""#38BDF8"" stroke-width=""5"" stroke-linecap=""round"" stroke-linejoin=""round""/>
                        <line x1=""58"" y1=""84"" x2=""78"" y2=""84"" stroke=""#E2E8F0"" stroke-width=""5"" stroke-linecap=""round""/>
                    </svg>";

                p.Objectives = new List<string>
                {
                    "Hiểu sâu kiến trúc hệ điều hành: quản lý tiến trình, lập lịch CPU, đồng bộ hóa và xử lý deadlock.",
                    "Nắm vững cơ chế quản lý bộ nhớ (phân trang, bộ nhớ ảo), hệ thống file và quản lý thiết bị vào/ra.",
                    "Thành thạo quản trị hệ thống Linux/Windows Server, viết shell script tự động hóa tác vụ."
                };

                p.Modules = new List<ModuleItem>
                {
                    new ModuleItem { Number = "01", BadgeColor = "#475569", Title = "Tiến trình & Lập lịch CPU", Description = "Khái niệm tiến trình/luồng, các thuật toán lập lịch và đồng bộ hóa đa luồng.", Bullets = new() { "Trạng thái tiến trình và chuyển ngữ cảnh (Context Switch).", "Thuật toán lập lịch: FCFS, SJF, Round Robin, Priority.", "Semaphore, Mutex và bài toán deadlock." } },
                    new ModuleItem { Number = "02", BadgeColor = "#0284c7", Title = "Bộ nhớ & Hệ thống file", Description = "Quản lý bộ nhớ vật lý/ảo, phân trang và tổ chức hệ thống tập tin.", Bullets = new() { "Phân đoạn, phân trang và bộ nhớ ảo (Virtual Memory).", "Thuật toán thay thế trang: FIFO, LRU, Optimal.", "Cấu trúc hệ thống file FAT32, NTFS, ext4." } },
                    new ModuleItem { Number = "03", BadgeColor = "#10b981", Title = "Quản trị Linux & Shell Script", Description = "Sử dụng thành thạo dòng lệnh Linux, phân quyền và tự động hóa bằng script.", Bullets = new() { "Lệnh Linux cơ bản và quản lý tiến trình.", "Phân quyền file, người dùng và nhóm.", "Viết Bash script tự động hóa sao lưu hệ thống." } }
                };

                p.TechStack = new List<TechChip>
                {
                    new TechChip { Name = "Linux / Ubuntu", IconClass = "bi bi-terminal", ColorClass = "text-secondary" },
                    new TechChip { Name = "Bash Script", IconClass = "bi bi-code-slash", ColorClass = "text-success" },
                    new TechChip { Name = "Windows Server", IconClass = "bi bi-window", ColorClass = "text-primary" },
                    new TechChip { Name = "VirtualBox", IconClass = "bi bi-boxes", ColorClass = "text-info" },
                    new TechChip { Name = "Docker", IconClass = "bi bi-box", ColorClass = "text-primary" },
                    new TechChip { Name = "Git", IconClass = "bi bi-git", ColorClass = "text-danger" }
                };

                p.LecturerName = "Nguyễn Hoàng Long";
                p.LecturerHocVi = "TS.";
                p.LecturerDept = "Khoa Công nghệ thông tin";
                p.LecturerEmail = "nhlong@uneti.edu.vn";
                p.LecturerPhone = "0912 345 678";
            }
            // 10. NHÓM AN TOÀN THÔNG TIN
            else if (name.Contains("An toàn", StringComparison.OrdinalIgnoreCase) || name.Contains("Bảo mật", StringComparison.OrdinalIgnoreCase) || name.Contains("Security", StringComparison.OrdinalIgnoreCase))
            {
                p.CategoryName = "An toàn & Bảo mật thông tin";
                p.ThemePrimaryColor = "#e11d48";
                p.ThemeGradient = "linear-gradient(135deg, #ffe4e6 0%, #fff1f2 45%, #eff6ff 100%)";
                p.ThemeBorderColor = "#fecdd3";
                p.BrandBoxBg = "linear-gradient(135deg, #e11d48 0%, #9f1239 100%)";
                p.BrandBoxShadow = "rgba(225, 29, 72, 0.35)";

                p.ColorC1 = "#f43f5e";
                p.ColorC2 = "#ec4899";
                p.ColorAccent = "#be123c";
                p.IconWhiteSvg = @"<svg width=""56"" height=""56"" viewBox=""0 0 24 24"" fill=""none"" stroke=""white"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round""><path d=""M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z""/></svg>";
                p.BannerFile = "detail-banner-3.webp";

                p.BrandLogoSvg = @"
                    <svg width=""48"" height=""48"" viewBox=""0 0 128 128"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                        <rect x=""14"" y=""14"" width=""100"" height=""100"" rx=""24"" fill=""#E11D48""/>
                        <path d=""M64 28 L92 40 V66 C92 86 78 98 64 104 C50 98 36 86 36 66 V40 Z"" fill=""#FFFFFF""/>
                        <path d=""M54 64 L61 71 L76 56"" stroke=""#E11D48"" stroke-width=""6"" stroke-linecap=""round"" stroke-linejoin=""round""/>
                    </svg>";

                p.Objectives = new List<string>
                {
                    "Nắm vững các nguyên lý mã hóa đối xứng/bất đối xứng, hàm băm và chữ ký số trong bảo vệ dữ liệu.",
                    "Hiểu các hình thức tấn công mạng phổ biến (SQL Injection, XSS, DDoS) và biện pháp phòng chống.",
                    "Xây dựng chính sách an toàn thông tin, kiểm thử xâm nhập cơ bản và quy trình ứng cứu sự cố."
                };

                p.Modules = new List<ModuleItem>
                {
                    new ModuleItem { Number = "01", BadgeColor = "#e11d48", Title = "Mã hóa & Chữ ký số", Description = "Thuật toán mã hóa AES, RSA, hàm băm SHA và hạ tầng khóa công khai PKI.", Bullets = new() { "Mã hóa đối xứng AES và bất đối xứng RSA.", "Hàm băm SHA-256 và kiểm tra toàn vẹn.", "Chữ ký số và chứng chỉ số SSL/TLS." } },
                    new ModuleItem { Number = "02", BadgeColor = "#f59e0b", Title = "An ninh mạng & Tấn công", Description = "Các lỗ hổng OWASP Top 10, tường lửa, IDS/IPS và kỹ thuật tấn công phổ biến.", Bullets = new() { "Lỗ hổng Injection, XSS, CSRF theo chuẩn OWASP.", "Tường lửa và hệ thống phát hiện xâm nhập.", "Kỹ thuật Social Engineering và phòng tránh." } },
                    new ModuleItem { Number = "03", BadgeColor = "#0284c7", Title = "Kiểm thử & Ứng cứu sự cố", Description = "Quy trình pentest cơ bản, phân tích mã độc và lập kế hoạch ứng phó.", Bullets = new() { "Phương pháp kiểm thử xâm nhập với Kali Linux.", "Phân tích log và truy vết nguồn tấn công.", "Xây dựng kế hoạch ứng cứu và khôi phục hệ thống." } }
                };

                p.TechStack = new List<TechChip>
                {
                    new TechChip { Name = "Cryptography", IconClass = "bi bi-key-fill", ColorClass = "text-danger" },
                    new TechChip { Name = "Kali Linux", IconClass = "bi bi-terminal", ColorClass = "text-secondary" },
                    new TechChip { Name = "Wireshark", IconClass = "bi bi-activity", ColorClass = "text-info" },
                    new TechChip { Name = "Burp Suite", IconClass = "bi bi-shield-check", ColorClass = "text-warning" },
                    new TechChip { Name = "OWASP ZAP", IconClass = "bi bi-bug", ColorClass = "text-primary" },
                    new TechChip { Name = "Git", IconClass = "bi bi-git", ColorClass = "text-secondary" }
                };

                p.LecturerName = "Phạm Minh Đức";
                p.LecturerHocVi = "TS.";
                p.LecturerDept = "Khoa Công nghệ thông tin";
                p.LecturerEmail = "pmduc@uneti.edu.vn";
                p.LecturerPhone = "0945 678 901";
            }
            // 11. NHÓM CÔNG NGHỆ JAVA
            else if (name.Contains("Java", StringComparison.OrdinalIgnoreCase))
            {
                p.CategoryName = "Lập trình Java & Hệ sinh thái JVM";
                p.ThemePrimaryColor = "#dc2626";
                p.ThemeGradient = "linear-gradient(135deg, #fee2e2 0%, #fef2f2 45%, #fff7ed 100%)";
                p.ThemeBorderColor = "#fecaca";
                p.BrandBoxBg = "linear-gradient(135deg, #ef4444 0%, #991b1b 100%)";
                p.BrandBoxShadow = "rgba(220, 38, 38, 0.35)";

                p.ColorC1 = "#f97316";
                p.ColorC2 = "#ef4444";
                p.ColorAccent = "#c2410c";
                p.IconWhiteSvg = @"<svg width=""56"" height=""56"" viewBox=""0 0 24 24"" fill=""none"" stroke=""white"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round""><path d=""M18 8h1a4 4 0 0 1 0 8h-1""/><path d=""M2 8h16v9a4 4 0 0 1-4 4H6a4 4 0 0 1-4-4V8z""/><line x1=""6"" y1=""1"" x2=""6"" y2=""4""/><line x1=""10"" y1=""1"" x2=""10"" y2=""4""/><line x1=""14"" y1=""1"" x2=""14"" y2=""4""/></svg>";

                p.BrandLogoSvg = @"
                    <svg width=""48"" height=""48"" viewBox=""0 0 128 128"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                        <rect x=""14"" y=""14"" width=""100"" height=""100"" rx=""24"" fill=""#DC2626""/>
                        <path d=""M38 52 H78 V76 C78 88 68 94 58 94 C48 94 38 88 38 76 Z"" fill=""#FFFFFF""/>
                        <path d=""M78 58 H90 C97 58 97 72 90 72 H78"" stroke=""#FFFFFF"" stroke-width=""6"" fill=""none""/>
                        <path d=""M52 32 C52 38 58 38 58 44 M66 32 C66 38 72 38 72 44"" stroke=""#FECACA"" stroke-width=""5"" stroke-linecap=""round"" fill=""none""/>
                    </svg>";

                p.Objectives = new List<string>
                {
                    "Làm chủ ngôn ngữ Java: cú pháp, Collections Framework, Generics, Stream API và lập trình đa luồng.",
                    "Xây dựng ứng dụng Java kết nối cơ sở dữ liệu qua JDBC, lập trình mạng Socket và giao diện JavaFX.",
                    "Phát triển ứng dụng web doanh nghiệp với Spring Boot, RESTful API và triển khai theo mô hình MVC."
                };

                p.Modules = new List<ModuleItem>
                {
                    new ModuleItem { Number = "01", BadgeColor = "#dc2626", Title = "Java Core & Collections", Description = "Nền tảng Java SE, OOP trong Java, Collections và xử lý ngoại lệ.", Bullets = new() { "Cú pháp Java, JVM, JRE và cơ chế Garbage Collection.", "Collections Framework: List, Set, Map.", "Stream API, Lambda và lập trình đa luồng." } },
                    new ModuleItem { Number = "02", BadgeColor = "#0284c7", Title = "JDBC & Lập trình mạng", Description = "Kết nối CSDL quan hệ, lập trình Socket và xây dựng ứng dụng client-server.", Bullets = new() { "Kết nối và truy vấn CSDL qua JDBC.", "Lập trình mạng TCP/UDP với Socket.", "Đa luồng trong ứng dụng server." } },
                    new ModuleItem { Number = "03", BadgeColor = "#16a34a", Title = "Spring Boot & REST API", Description = "Phát triển backend hiện đại với Spring Boot, JPA và bảo mật JWT.", Bullets = new() { "Khởi tạo dự án Spring Boot và Dependency Injection.", "Spring Data JPA và quản lý transaction.", "Xây dựng RESTful API và xác thực JWT." } }
                };

                p.TechStack = new List<TechChip>
                {
                    new TechChip { Name = "Java 21", IconClass = "bi bi-filetype-java", ColorClass = "text-danger" },
                    new TechChip { Name = "Spring Boot", IconClass = "bi bi-leaf", ColorClass = "text-success" },
                    new TechChip { Name = "Maven", IconClass = "bi bi-box-seam", ColorClass = "text-warning" },
                    new TechChip { Name = "MySQL", IconClass = "bi bi-database", ColorClass = "text-primary" },
                    new TechChip { Name = "IntelliJ IDEA", IconClass = "bi bi-terminal", ColorClass = "text-secondary" },
                    new TechChip { Name = "Git", IconClass = "bi bi-git", ColorClass = "text-secondary" }
                };

                p.LecturerName = "Nguyễn Văn An";
                p.LecturerHocVi = "ThS.";
                p.LecturerDept = "Khoa Công nghệ thông tin";
                p.LecturerEmail = "nguyenvanan@uneti.edu.vn";
                p.LecturerPhone = "0987 654 321";
            }
            // 12. NHÓM PHÂN TÍCH & THIẾT KẾ HỆ THỐNG THÔNG TIN
            else if (name.Contains("Phân tích và thiết kế", StringComparison.OrdinalIgnoreCase) || name.Contains("hệ thống thông tin", StringComparison.OrdinalIgnoreCase))
            {
                p.CategoryName = "Phân tích & Thiết kế Hệ thống thông tin";
                p.ThemePrimaryColor = "#4f46e5";
                p.ThemeGradient = "linear-gradient(135deg, #e0e7ff 0%, #eef2ff 45%, #eff6ff 100%)";
                p.ThemeBorderColor = "#c7d2fe";
                p.BrandBoxBg = "linear-gradient(135deg, #6366f1 0%, #4338ca 100%)";
                p.BrandBoxShadow = "rgba(79, 70, 229, 0.35)";

                p.ColorC1 = "#8b5cf6";
                p.ColorC2 = "#d946ef";
                p.ColorAccent = "#9333ea";
                p.IconWhiteSvg = @"<svg width=""56"" height=""56"" viewBox=""0 0 24 24"" fill=""none"" stroke=""white"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round""><rect x=""3"" y=""3"" width=""7"" height=""7""/><rect x=""14"" y=""3"" width=""7"" height=""7""/><rect x=""14"" y=""14"" width=""7"" height=""7""/><rect x=""3"" y=""14"" width=""7"" height=""7""/></svg>";

                p.BrandLogoSvg = @"
                    <svg width=""48"" height=""48"" viewBox=""0 0 128 128"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                        <rect x=""14"" y=""14"" width=""100"" height=""100"" rx=""24"" fill=""#4F46E5""/>
                        <rect x=""30"" y=""34"" width=""34"" height=""22"" rx=""6"" fill=""#FFFFFF""/>
                        <rect x=""74"" y=""72"" width=""34"" height=""22"" rx=""6"" fill=""#C7D2FE""/>
                        <path d=""M47 56 V84 H72"" stroke=""#A5B4FC"" stroke-width=""5"" fill=""none""/>
                        <path d=""M66 78 L72 84 L78 78"" stroke=""#A5B4FC"" stroke-width=""5"" stroke-linecap=""round"" stroke-linejoin=""round""/>
                    </svg>";

                p.Objectives = new List<string>
                {
                    "Nắm vững quy trình phát triển hệ thống thông tin: khảo sát hiện trạng, phân tích yêu cầu và đặc tả nghiệp vụ.",
                    "Sử dụng thành thạo ngôn ngữ mô hình hóa UML (Use Case, Class Diagram, Sequence Diagram) để thiết kế hệ thống.",
                    "Thiết kế cơ sở dữ liệu quan hệ, kiến trúc hệ thống và lập kế hoạch triển khai theo mô hình Agile/Scrum."
                };

                p.Modules = new List<ModuleItem>
                {
                    new ModuleItem { Number = "01", BadgeColor = "#4f46e5", Title = "Khảo sát & Phân tích yêu cầu", Description = "Thu thập yêu cầu người dùng, phân tích nghiệp vụ và đặc tả chức năng hệ thống.", Bullets = new() { "Kỹ thuật phỏng vấn và khảo sát hiện trạng.", "Phân tích yêu cầu chức năng/phi chức năng.", "Đặc tả yêu cầu bằng tài liệu SRS." } },
                    new ModuleItem { Number = "02", BadgeColor = "#0284c7", Title = "Mô hình hóa với UML", Description = "Vẽ sơ đồ Use Case, Class, Sequence và Activity Diagram cho hệ thống.", Bullets = new() { "Sơ đồ Use Case và đặc tả kịch bản.", "Sơ đồ lớp (Class Diagram) và quan hệ.", "Sơ đồ tuần tự và sơ đồ hoạt động." } },
                    new ModuleItem { Number = "03", BadgeColor = "#10b981", Title = "Thiết kế CSDL & Kiến trúc", Description = "Thiết kế lược đồ CSDL, kiến trúc phân tầng và quy trình phát triển Agile.", Bullets = new() { "Thiết kế CSDL quan hệ từ Class Diagram.", "Kiến trúc 3 tầng và mô hình MVC.", "Quản lý dự án phần mềm theo Scrum." } }
                };

                p.TechStack = new List<TechChip>
                {
                    new TechChip { Name = "UML", IconClass = "bi bi-diagram-3", ColorClass = "text-primary" },
                    new TechChip { Name = "Draw.io", IconClass = "bi bi-bounding-box", ColorClass = "text-info" },
                    new TechChip { Name = "SQL Server", IconClass = "bi bi-database", ColorClass = "text-warning" },
                    new TechChip { Name = "Agile / Scrum", IconClass = "bi bi-kanban", ColorClass = "text-success" },
                    new TechChip { Name = "Figma", IconClass = "bi bi-palette", ColorClass = "text-danger" },
                    new TechChip { Name = "Git", IconClass = "bi bi-git", ColorClass = "text-secondary" }
                };

                p.LecturerName = "Trần Thị Mai Lan";
                p.LecturerHocVi = "ThS.";
                p.LecturerDept = "Khoa Công nghệ thông tin";
                p.LecturerEmail = "ttmlan@uneti.edu.vn";
                p.LecturerPhone = "0923 456 789";
            }
            // 13. CÁC MÔN HỌC KHÁC (FALLBACK CHUẨN ĐA NGÀNH)
            else
            {
                p.CategoryName = "Chương trình Đào tạo Đại học Chính quy";
                p.ThemePrimaryColor = "#1d4ed8";
                p.ThemeGradient = "linear-gradient(135deg, #e0f2fe 0%, #edf8ff 45%, #f0fdf4 100%)";
                p.ThemeBorderColor = "#bae6fd";
                p.BrandBoxBg = "linear-gradient(135deg, #1d4ed8 0%, #1e40af 100%)";
                p.BrandBoxShadow = "rgba(29, 78, 216, 0.35)";

                p.BrandLogoSvg = @"
                    <svg width=""48"" height=""48"" viewBox=""0 0 128 128"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                        <rect x=""14"" y=""14"" width=""100"" height=""100"" rx=""24"" fill=""#1D4ED8""/>
                        <!-- Academic Book & Graduation Cap -->
                        <path d=""M64 30 L98 46 L64 62 L30 46 Z"" fill=""#FFFFFF""/>
                        <path d=""M36 56 V78 C36 86, 92 86, 92 78 V56 Z"" fill=""#93C5FD""/>
                        <circle cx=""98"" cy=""48"" r=""3"" fill=""#FDE047""/>
                        <path d=""M98 51 V75"" stroke=""#FDE047"" stroke-width=""2""/>
                    </svg>";

                p.Objectives = new List<string>
                {
                    $"Nắm vững toàn diện hệ thống lý thuyết nền tảng, khái niệm chuẩn mực và phương pháp luận của học phần {name}.",
                    "Rèn luyện kỹ năng thực hành, giải quyết bài toán nghiệp vụ chuyên môn và làm việc nhóm hiệu quả.",
                    $"Đạt chuẩn đầu ra chương trình đào tạo chính quy của Trường ĐH Kinh tế - Kỹ thuật Công nghiệp và tích lũy {monHoc.SoTinChi} tín chỉ."
                };

                p.Modules = new List<ModuleItem>
                {
                    new ModuleItem { Number = "01", BadgeColor = "#1d4ed8", Title = "Cơ sở Lý thuyết & Tổng quan", Description = $"Khái niệm căn bản, nguyên lý và đối tượng nghiên cứu của học phần {name}.", Bullets = new() { "Hệ thống thuật ngữ và khái niệm cốt lõi.", "Các quy chuẩn và nguyên lý vận hành.", "Phương pháp tiếp cận chuyên ngành." } },
                    new ModuleItem { Number = "02", BadgeColor = "#0284c7", Title = "Phương pháp & Kỹ năng Nghiệp vụ", Description = "Vận dụng kiến thức vào thực hành tình huống, xử lý số liệu và đồ án môn học.", Bullets = new() { "Quy trình thực hiện bài tập và tình huống thực tế.", "Phân tích, xử lý và đánh giá dữ liệu.", "Làm việc nhóm và thuyết trình kết quả." } },
                    new ModuleItem { Number = "03", BadgeColor = "#10b981", Title = "Ứng dụng Thực tiễn & Đánh giá", Description = "Hoàn thiện đề tài nghiên cứu, kiểm tra thực hành và chuẩn bị thi kết thúc học phần.", Bullets = new() { "Tổng kết các chuyên đề trọng tâm.", "Đánh giá kết quả học tập qua bài thi/đồ án.", "Hướng nghiệp và liên hệ môn học tiếp theo." } }
                };

                p.TechStack = new List<TechChip>
                {
                    new TechChip { Name = "Hệ thống học tập LMS", IconClass = "bi bi-laptop", ColorClass = "text-primary" },
                    new TechChip { Name = "Giáo trình Chuẩn Bộ GD&ĐT", IconClass = "bi bi-book", ColorClass = "text-info" },
                    new TechChip { Name = "Công cụ Phần mềm Chuyên môn", IconClass = "bi bi-tools", ColorClass = "text-warning" },
                    new TechChip { Name = "Tài liệu Trực tuyến", IconClass = "bi bi-cloud-arrow-down", ColorClass = "text-success" },
                    new TechChip { Name = "Microsoft 365", IconClass = "bi bi-grid-fill", ColorClass = "text-danger" },
                    new TechChip { Name = "Thực hành Phòng máy", IconClass = "bi bi-pc-display", ColorClass = "text-secondary" }
                };

                p.LecturerName = "Nguyễn Văn An";
                p.LecturerHocVi = "ThS.";
                p.LecturerDept = "Khoa Công nghệ thông tin";
                p.LecturerEmail = "cntt@uneti.edu.vn";
                p.LecturerPhone = "024 3862 1504";
            }

            // Gán tự động --c1, --c2, --accent và icon theo bảng màu 6 sắc thái nếu chưa gán cụ thể
            if (string.IsNullOrEmpty(p.IconWhiteSvg))
            {
                var defaultPalettes = new (string c1, string c2, string accent)[]
                {
                    ("#3b82f6", "#8b5cf6", "#2563eb"), // C#
                    ("#a78bfa", "#ec4899", "#7c3aed"), // SQL
                    ("#f59e0b", "#fb923c", "#c2410c"), // React
                    ("#2dd4bf", "#38bdf8", "#0f766e"), // Flutter
                    ("#10b981", "#14b8a6", "#047857"), // Python
                    ("#f472b6", "#a855f7", "#be185d")  // OOP
                };
                int pIdx = Math.Abs(monHoc.MaMonHoc) % defaultPalettes.Length;
                p.ColorC1 = defaultPalettes[pIdx].c1;
                p.ColorC2 = defaultPalettes[pIdx].c2;
                p.ColorAccent = defaultPalettes[pIdx].accent;
                p.IconWhiteSvg = @"<svg width=""56"" height=""56"" viewBox=""0 0 24 24"" fill=""none"" stroke=""white"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round""><path d=""M2 3h6a4 4 0 0 1 4 4v14a3 3 0 0 0-3-3H2z""/><path d=""M22 3h-6a4 4 0 0 0-4 4v14a3 3 0 0 1 3-3h7z""/></svg>";
            }

            return p;
        }
    }
}
