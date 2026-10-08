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
        public string LecturerBio { get; set; } = "Giảng viên có nhiều năm kinh nghiệm giảng dạy tại Trường ĐH Kinh tế - Kỹ thuật Công nghiệp (UNETI).";

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
                p.ThemePrimaryColor = "#0284c7";
                p.ThemeGradient = "linear-gradient(135deg, #e0f2fe 0%, #eff6ff 45%, #ecfeff 100%)";
                p.ThemeBorderColor = "#bae6fd";
                p.BrandBoxBg = "linear-gradient(135deg, #0284c7 0%, #0369a1 100%)";
                p.BrandBoxShadow = "rgba(2, 132, 199, 0.35)";

                p.BrandLogoSvg = @"
                    <svg width=""48"" height=""48"" viewBox=""0 0 128 128"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                        <rect x=""14"" y=""14"" width=""100"" height=""100"" rx=""24"" fill=""#0284C7""/>
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
                    new ModuleItem { Number = "01", BadgeColor = "#0284c7", Title = "Mô hình Dữ liệu & Chuẩn hóa", Description = "Thiết kế ERD, lược đồ bảng, khóa chính, khóa ngoại và các dạng chuẩn 1NF - 3NF.", Bullets = new() { "Phân tích yêu cầu và thiết kế lược đồ ERD.", "Chuyển đổi sang mô hình CSDL quan hệ.", "Quy chuẩn hóa 1NF, 2NF, 3NF và BCNF loại bỏ dư thừa." } },
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
                p.ThemePrimaryColor = "#06b6d4";
                p.ThemeGradient = "linear-gradient(135deg, #ecfeff 0%, #f0fdfa 45%, #eff6ff 100%)";
                p.ThemeBorderColor = "#a5f3fc";
                p.BrandBoxBg = "linear-gradient(135deg, #06b6d4 0%, #0891b2 100%)";
                p.BrandBoxShadow = "rgba(6, 182, 212, 0.35)";

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
                    new ModuleItem { Number = "01", BadgeColor = "#06b6d4", Title = "ES6+ & Nền tảng ReactJS", Description = "Cú pháp ES6, JSX, Component Lifecycle, Virtual DOM và kiến trúc SPA.", Bullets = new() { "JavaScript hiện đại: Destructuring, Spread, Promise/Async.", "Tư duy chia nhỏ Component và luồng Props.", "Cơ chế Render và Reconciliation của React." } },
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
                p.ThemePrimaryColor = "#0284c7";
                p.ThemeGradient = "linear-gradient(135deg, #e0f2fe 0%, #f0fdf4 45%, #eff6ff 100%)";
                p.ThemeBorderColor = "#bae6fd";
                p.BrandBoxBg = "linear-gradient(135deg, #0284c7 0%, #0369a1 100%)";
                p.BrandBoxShadow = "rgba(2, 132, 199, 0.35)";

                p.BrandLogoSvg = @"
                    <svg width=""48"" height=""48"" viewBox=""0 0 128 128"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                        <rect x=""14"" y=""14"" width=""100"" height=""100"" rx=""24"" fill=""#0284C7""/>
                        <!-- Mobile Flutter Icon -->
                        <path d=""M76 24 L34 66 L48 80 L90 38 Z"" fill=""#FFFFFF""/>
                        <path d=""M62 80 L48 94 L62 108 L76 94 Z"" fill=""#7DD3FC""/>
                        <path d=""M76 66 L62 80 L76 94 L90 80 Z"" fill=""#E0F2FE""/>
                    </svg>";

                p.Objectives = new List<string>
                {
                    "Làm chủ ngôn ngữ lập trình Dart hướng đối tượng, cây Widget Flutter (Stateless & Stateful) và cơ chế Hot Reload.",
                    "Thiết kế giao diện ứng dụng chuẩn Material 3 & Cupertino mượt mà, quản lý trạng thái với Provider/Bloc Pattern.",
                    "Lập trình tương tác Backend qua REST API, lưu trữ dữ liệu cục bộ SQLite và đóng gói xuất bản ứng dụng lên App Store/Google Play."
                };

                p.Modules = new List<ModuleItem>
                {
                    new ModuleItem { Number = "01", BadgeColor = "#0284c7", Title = "Ngôn ngữ Dart & Cây Widget", Description = "Cú pháp Dart, tư duy Widget trong Flutter, Layouts và Custom Animation.", Bullets = new() { "Ngôn ngữ Dart OOP, Null Safety và Async/Await.", "StatelessWidget, StatefulWidget và Vòng đời Widget.", "Hệ thống bố cục Column, Row, Stack và ListView." } },
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
            else if (name.Contains("Python", StringComparison.OrdinalIgnoreCase) || name.Contains("Dữ liệu", StringComparison.OrdinalIgnoreCase) || name.Contains("AI", StringComparison.OrdinalIgnoreCase) || name.Contains("Trí tuệ nhân tạo", StringComparison.OrdinalIgnoreCase))
            {
                p.CategoryName = "Khoa học Dữ liệu & Trí tuệ Nhân tạo";
                p.ThemePrimaryColor = "#d97706";
                p.ThemeGradient = "linear-gradient(135deg, #fef3c7 0%, #fffbeb 45%, #eff6ff 100%)";
                p.ThemeBorderColor = "#fde68a";
                p.BrandBoxBg = "linear-gradient(135deg, #d97706 0%, #b45309 100%)";
                p.BrandBoxShadow = "rgba(217, 119, 6, 0.35)";

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
            // 8. CÁC MÔN HỌC KHÁC (FALLBACK CHUẨN ĐA NGÀNH)
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
                    new TechChip { Name = "Hệ thống LMS UNETI", IconClass = "bi bi-laptop", ColorClass = "text-primary" },
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

            return p;
        }
    }
}
