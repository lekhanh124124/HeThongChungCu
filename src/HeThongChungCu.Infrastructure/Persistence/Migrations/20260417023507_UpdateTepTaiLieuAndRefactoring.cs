using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTepTaiLieuAndRefactoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoaiYeuCauCuDan",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "LoaiTepTaiLieu",
                table: "TepTaiLieu");

            migrationBuilder.DropColumn(
                name: "LoaiTaiLieu",
                table: "TaiLieu");

            migrationBuilder.DropColumn(
                name: "LoaiNhanSu",
                table: "NhanSuYeuCau");

            migrationBuilder.RenameColumn(
                name: "LoaiYeuCauId",
                table: "YeuCau",
                newName: "YeuCauPhuongTien_LoaiHanhDongYeuCauId");

            migrationBuilder.AddColumn<int>(
                name: "LoaiHanhDongYeuCauId",
                table: "YeuCau",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoaiYeuCauCuDanId",
                table: "YeuCau",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LoaiTepId",
                table: "TepTaiLieu",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LoaiTaiLieuId",
                table: "TaiLieu",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LoaiNhanSuId",
                table: "NhanSuYeuCau",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoaiHanhDongYeuCauId",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "LoaiYeuCauCuDanId",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "LoaiTepId",
                table: "TepTaiLieu");

            migrationBuilder.DropColumn(
                name: "LoaiTaiLieuId",
                table: "TaiLieu");

            migrationBuilder.DropColumn(
                name: "LoaiNhanSuId",
                table: "NhanSuYeuCau");

            migrationBuilder.RenameColumn(
                name: "YeuCauPhuongTien_LoaiHanhDongYeuCauId",
                table: "YeuCau",
                newName: "LoaiYeuCauId");

            migrationBuilder.AddColumn<string>(
                name: "LoaiYeuCauCuDan",
                table: "YeuCau",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LoaiTepTaiLieu",
                table: "TepTaiLieu",
                type: "nvarchar(34)",
                maxLength: 34,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LoaiTaiLieu",
                table: "TaiLieu",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LoaiNhanSu",
                table: "NhanSuYeuCau",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");
        }
    }
}
