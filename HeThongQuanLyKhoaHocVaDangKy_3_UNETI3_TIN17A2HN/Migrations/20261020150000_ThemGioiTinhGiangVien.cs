using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Migrations
{
    /// <inheritdoc />
    public partial class ThemGioiTinhGiangVien : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM sys.columns 
                    WHERE object_id = OBJECT_ID(N'[dbo].[GiangVien]') 
                    AND name = 'GioiTinh'
                )
                BEGIN
                    ALTER TABLE [dbo].[GiangVien] ADD [GioiTinh] nvarchar(10) NULL;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT * FROM sys.columns 
                    WHERE object_id = OBJECT_ID(N'[dbo].[GiangVien]') 
                    AND name = 'GioiTinh'
                )
                BEGIN
                    ALTER TABLE [dbo].[GiangVien] DROP COLUMN [GioiTinh];
                END
            ");
        }
    }
}
