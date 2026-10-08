// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Module 1: Cấu hình ứng dụng

using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data;

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

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

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
 