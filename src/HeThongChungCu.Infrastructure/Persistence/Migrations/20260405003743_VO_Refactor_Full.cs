using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class VO_Refactor_Full : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YeuCauDiaChi_PhuongXa",
                table: "YeuCau",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YeuCauDiaChi_QuanHuyen",
                table: "YeuCau",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YeuCauDiaChi_SoNhaTenDuong",
                table: "YeuCau",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YeuCauDiaChi_TinhThanhPho",
                table: "YeuCau",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiaChi_PhuongXa",
                table: "ToaNha",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DiaChi_QuanHuyen",
                table: "ToaNha",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DiaChi_SoNhaTenDuong",
                table: "ToaNha",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DiaChi_TinhThanhPho",
                table: "ToaNha",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "NguoiDung",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DiaChi",
                table: "NguoiDung",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiaChi_PhuongXa",
                table: "NguoiDung",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DiaChi_QuanHuyen",
                table: "NguoiDung",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DiaChi_SoNhaTenDuong",
                table: "NguoiDung",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DiaChi_TinhThanhPho",
                table: "NguoiDung",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "DiaChi",
                table: "DoiTac",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiaChi_PhuongXa",
                table: "DoiTac",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DiaChi_QuanHuyen",
                table: "DoiTac",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DiaChi_SoNhaTenDuong",
                table: "DoiTac",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DiaChi_TinhThanhPho",
                table: "DoiTac",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YeuCauDiaChi_PhuongXa",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "YeuCauDiaChi_QuanHuyen",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "YeuCauDiaChi_SoNhaTenDuong",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "YeuCauDiaChi_TinhThanhPho",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "DiaChi_PhuongXa",
                table: "ToaNha");

            migrationBuilder.DropColumn(
                name: "DiaChi_QuanHuyen",
                table: "ToaNha");

            migrationBuilder.DropColumn(
                name: "DiaChi_SoNhaTenDuong",
                table: "ToaNha");

            migrationBuilder.DropColumn(
                name: "DiaChi_TinhThanhPho",
                table: "ToaNha");

            migrationBuilder.DropColumn(
                name: "DiaChi_PhuongXa",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "DiaChi_QuanHuyen",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "DiaChi_SoNhaTenDuong",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "DiaChi_TinhThanhPho",
                table: "NguoiDung");

            migrationBuilder.DropColumn(
                name: "DiaChi_PhuongXa",
                table: "DoiTac");

            migrationBuilder.DropColumn(
                name: "DiaChi_QuanHuyen",
                table: "DoiTac");

            migrationBuilder.DropColumn(
                name: "DiaChi_SoNhaTenDuong",
                table: "DoiTac");

            migrationBuilder.DropColumn(
                name: "DiaChi_TinhThanhPho",
                table: "DoiTac");

            migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "NguoiDung",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DiaChi",
                table: "NguoiDung",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DiaChi",
                table: "DoiTac",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }
    }
}
