using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovePriorityFromRepairRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
