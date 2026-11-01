// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Module 1: Cấu hình ứng dụng

using System.Globalization;
using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Helpers;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình DbContext với SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

// 2. Cấu hình Session & Memory Cache
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 3. Đăng ký Repository Pattern (Module 3: Ngọc Tùng Lâm - 23103100098)
builder.Services.AddScoped<HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories.IHocVienRepository, HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories.HocVienRepository>();
builder.Services.AddScoped<HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories.IDangKyKhoaHocRepository, HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Repositories.DangKyKhoaHocRepository>();

// 4. Đăng ký Repository Pattern (Module 5: Nguyễn Công Thành - 23103100102)
builder.Services.AddScoped<IKetQuaHocTapRepository, KetQuaHocTapRepository>();
builder.Services.AddScoped<IThongKeRepository, ThongKeRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

// Cấu hình Culture chuẩn (dấu chấm . cho số thập phân) để tương thích nhập liệu số thực
var defaultCulture = (CultureInfo)CultureInfo.GetCultureInfo("vi-VN").Clone();
defaultCulture.NumberFormat.NumberDecimalSeparator = ".";
defaultCulture.NumberFormat.NumberGroupSeparator = ",";
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    // Đăng ký ModelBinder hỗ trợ cả 8.5 và 8,5
    options.ModelBinderProviders.Insert(0, new FlexibleDecimalModelBinderProvider());
});

var app = builder.Build();

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(defaultCulture),
    SupportedCultures = new[] { defaultCulture },
    SupportedUICultures = new[] { defaultCulture }
};
app.UseRequestLocalization(localizationOptions);

// 3. TỰ ĐỘNG CHẠY MIGRATION VÀ SEED DATA KHI KHỞI ĐỘNG WEB
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Tự động áp dụng Migration tạo Database & bảng nếu chưa có
        context.Database.Migrate();

        // Đảm bảo cột HinhAnh trong bảng HocVien luôn tồn tại cho mọi máy clone và mọi môi trường DB
        try
        {
            context.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (
                    SELECT * FROM sys.columns 
                    WHERE object_id = OBJECT_ID(N'[dbo].[HocVien]') 
                    AND name = 'HinhAnh'
                )
                BEGIN
                    ALTER TABLE [dbo].[HocVien] ADD [HinhAnh] nvarchar(255) NULL;
                END
            ");
        }
        catch { }

        // Tự động nạp dữ liệu mẫu ban đầu
        DbInitializer.Seed(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Đã xảy ra lỗi trong quá trình tự động Migrate và Seed Data.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Bật Session Middleware trước Authorization
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
