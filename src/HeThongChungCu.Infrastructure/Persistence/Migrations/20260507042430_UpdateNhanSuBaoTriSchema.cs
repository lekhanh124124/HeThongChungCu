using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNhanSuBaoTriSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "NhanVienId",
                table: "NhanSuBaoTri",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "HoTen",
                table: "NhanSuBaoTri",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LoaiNhanSuId",
                table: "NhanSuBaoTri",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SoCCCD",
                table: "NhanSuBaoTri",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SoDienThoai",
                table: "NhanSuBaoTri",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoTen",
                table: "NhanSuBaoTri");

            migrationBuilder.DropColumn(
                name: "LoaiNhanSuId",
                table: "NhanSuBaoTri");

            migrationBuilder.DropColumn(
                name: "SoCCCD",
                table: "NhanSuBaoTri");

            migrationBuilder.DropColumn(
                name: "SoDienThoai",
                table: "NhanSuBaoTri");

            migrationBuilder.AlterColumn<int>(
                name: "NhanVienId",
                table: "NhanSuBaoTri",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
