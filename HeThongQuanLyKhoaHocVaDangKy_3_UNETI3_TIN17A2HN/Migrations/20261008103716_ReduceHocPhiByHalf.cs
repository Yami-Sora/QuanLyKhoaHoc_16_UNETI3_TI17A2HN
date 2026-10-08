using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongQuanLyKhoaHocVaDangKy_3_UNETI3_TIN17A2HN.Migrations
{
    public partial class ReduceHocPhiByHalf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tùy chỉnh: Cập nhật giảm học phí tất cả các môn học xuống một nửa
            migrationBuilder.Sql("UPDATE MonHoc SET HocPhi = HocPhi / 2;");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Tùy chỉnh: Khôi phục lại học phí cũ
            migrationBuilder.Sql("UPDATE MonHoc SET HocPhi = HocPhi * 2;");
        }
    }
}
