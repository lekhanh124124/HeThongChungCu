using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddYeuCauSuaChuaThiCong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "LoaiYeuCauId",
                table: "YeuCau",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DuKienBatDau",
                table: "YeuCau",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DuKienKetThuc",
                table: "YeuCau",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HangMucThiCong",
                table: "YeuCau",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "HenDen",
                table: "YeuCau",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "HenTu",
                table: "YeuCau",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KetQuaXuLy",
                table: "YeuCau",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoaiSuCoId",
                table: "YeuCau",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LyDoHuy",
                table: "YeuCau",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MoTaViTri",
                table: "YeuCau",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MucDoUuTienChotId",
                table: "YeuCau",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MucDoUuTienDeXuatId",
                table: "YeuCau",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "NgayChotUuTien",
                table: "YeuCau",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NguoiChotUuTienId",
                table: "YeuCau",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NguoiDaiDien",
                table: "YeuCau",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PhamViId",
                table: "YeuCau",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoDienThoaiDaiDien",
                table: "YeuCau",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenDonViThiCong",
                table: "YeuCau",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrangThaiSuaChuaId",
                table: "YeuCau",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrangThaiThiCongId",
                table: "YeuCau",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LoaiTepTaiLieu",
                table: "TepTaiLieu",
                type: "nvarchar(34)",
                maxLength: 34,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(21)",
                oldMaxLength: 21);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DuKienBatDau",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "DuKienKetThuc",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "HangMucThiCong",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "HenDen",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "HenTu",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "KetQuaXuLy",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "LoaiSuCoId",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "LyDoHuy",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "MoTaViTri",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "MucDoUuTienChotId",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "MucDoUuTienDeXuatId",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "NgayChotUuTien",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "NguoiChotUuTienId",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "NguoiDaiDien",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "PhamViId",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "SoDienThoaiDaiDien",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "TenDonViThiCong",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "TrangThaiSuaChuaId",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "TrangThaiThiCongId",
                table: "YeuCau");

            migrationBuilder.AlterColumn<int>(
                name: "LoaiYeuCauId",
                table: "YeuCau",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LoaiTepTaiLieu",
                table: "TepTaiLieu",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(34)",
                oldMaxLength: 34);
        }
    }
}
