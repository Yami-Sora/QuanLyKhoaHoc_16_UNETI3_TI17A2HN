using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Migrations
{
    /// <inheritdoc />
    public partial class AddHinhAnhToHocVien : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT * FROM sys.columns 
                    WHERE object_id = OBJECT_ID(N'[dbo].[HocVien]') 
                    AND name = 'HinhAnh'
                )
                BEGIN
                    ALTER TABLE [dbo].[HocVien] DROP COLUMN [HinhAnh];
                END
            ");
        }
    }
}
