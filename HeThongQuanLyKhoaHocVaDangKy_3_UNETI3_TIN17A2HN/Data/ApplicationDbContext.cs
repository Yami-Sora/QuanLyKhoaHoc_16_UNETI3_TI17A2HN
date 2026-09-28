// Họ và tên: Trần Văn Thành
// Mã sinh viên: 23103100076
// Nội dung thực hiện: Module 1 - Cấu hình DbContext, Fluent API và Ràng buộc toàn vẹn CSDL

using Microsoft.EntityFrameworkCore;
using HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Models;

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaiKhoan> TaiKhoans => Set<TaiKhoan>();
        public DbSet<MonHoc> MonHocs => Set<MonHoc>();
        public DbSet<GiangVien> GiangViens => Set<GiangVien>();
        public DbSet<KhoaHoc> KhoaHocs => Set<KhoaHoc>();
        public DbSet<HocVien> HocViens => Set<HocVien>();
        public DbSet<DangKyKhoaHoc> DangKyKhoaHocs => Set<DangKyKhoaHoc>();
        public DbSet<KetQuaHocTap> KetQuaHocTaps => Set<KetQuaHocTap>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Chỉ mục Unique chống trùng tên và trùng email
            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(t => t.TenDangNhap)
                .IsUnique();

            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(t => t.Email)
                .IsUnique();

            modelBuilder.Entity<MonHoc>()
                .HasIndex(m => m.TenMonHoc)
                .IsUnique();

            // 2. Ràng buộc quan hệ MonHoc -> KhoaHoc (Chặn xóa Cascade - Restrict)
            modelBuilder.Entity<KhoaHoc>()
                .HasOne(k => k.MonHoc)
                .WithMany(m => m.KhoaHocs)
                .HasForeignKey(k => k.MaMonHoc)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. Ràng buộc quan hệ GiangVien -> KhoaHoc (Chặn xóa Cascade - Restrict)
            modelBuilder.Entity<KhoaHoc>()
                .HasOne(k => k.GiangVien)
                .WithMany(g => g.KhoaHocs)
                .HasForeignKey(k => k.MaGiangVien)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. Ràng buộc quan hệ HocVien -> DangKyKhoaHoc (Chặn xóa Cascade - Restrict)
            modelBuilder.Entity<DangKyKhoaHoc>()
                .HasOne(d => d.HocVien)
                .WithMany(h => h.DangKyKhoaHocs)
                .HasForeignKey(d => d.MaHocVien)
                .OnDelete(DeleteBehavior.Restrict);

            // 5. Ràng buộc quan hệ KhoaHoc -> DangKyKhoaHoc (Chặn xóa Cascade - Restrict)
            modelBuilder.Entity<DangKyKhoaHoc>()
                .HasOne(d => d.KhoaHoc)
                .WithMany(k => k.DangKyKhoaHocs)
                .HasForeignKey(d => d.MaKhoaHoc)
                .OnDelete(DeleteBehavior.Restrict);

            // 6. Ràng buộc 1 - 1 giữa DangKyKhoaHoc và KetQuaHocTap (Cascade)
            modelBuilder.Entity<KetQuaHocTap>()
                .HasOne(kq => kq.DangKyKhoaHoc)
                .WithOne(dk => dk.KetQuaHocTap)
                .HasForeignKey<KetQuaHocTap>(kq => kq.MaDangKy)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
