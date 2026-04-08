using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChiTietGiaAuditable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "ChiTietGiaLuyTien",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "ChiTietGiaLuyTien",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "ChiTietGiaLuyTien",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ChiTietGiaLuyTien",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModifiedAt",
                table: "ChiTietGiaLuyTien",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "ChiTietGiaLuyTien",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "ChiTietGiaKhungGio",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "ChiTietGiaKhungGio",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "ChiTietGiaKhungGio",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ChiTietGiaKhungGio",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModifiedAt",
                table: "ChiTietGiaKhungGio",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "ChiTietGiaKhungGio",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ChiTietGiaLuyTien");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ChiTietGiaLuyTien");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ChiTietGiaLuyTien");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ChiTietGiaLuyTien");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "ChiTietGiaLuyTien");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "ChiTietGiaLuyTien");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ChiTietGiaKhungGio");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ChiTietGiaKhungGio");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ChiTietGiaKhungGio");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ChiTietGiaKhungGio");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "ChiTietGiaKhungGio");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "ChiTietGiaKhungGio");
        }
    }
}
