using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGiaoDichThanhToanSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GiaoDichThanhToan_HoaDon_HoaDonId",
                table: "GiaoDichThanhToan");

            migrationBuilder.DropTable(
                name: "GiaoDichThanhToanChiTiet");

            migrationBuilder.RenameColumn(
                name: "HoaDonId",
                table: "GiaoDichThanhToan",
                newName: "ChiTietHoaDonId");

            migrationBuilder.RenameIndex(
                name: "IX_GiaoDichThanhToan_HoaDonId",
                table: "GiaoDichThanhToan",
                newName: "IX_GiaoDichThanhToan_ChiTietHoaDonId");

            migrationBuilder.CreateTable(
                name: "PhienThanhToan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThanhToan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HoaDonId = table.Column<int>(type: "int", nullable: false),
                    ChiTietHoaDonIds = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TrangThaiThanhToanId = table.Column<int>(type: "int", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhienThanhToan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhienThanhToan_HoaDon_HoaDonId",
                        column: x => x.HoaDonId,
                        principalTable: "HoaDon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhienThanhToan_HoaDonId",
                table: "PhienThanhToan",
                column: "HoaDonId");

            migrationBuilder.CreateIndex(
                name: "IX_PhienThanhToan_MaThanhToan",
                table: "PhienThanhToan",
                column: "MaThanhToan",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GiaoDichThanhToan_ChiTietHoaDon_ChiTietHoaDonId",
                table: "GiaoDichThanhToan",
                column: "ChiTietHoaDonId",
                principalTable: "ChiTietHoaDon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GiaoDichThanhToan_ChiTietHoaDon_ChiTietHoaDonId",
                table: "GiaoDichThanhToan");

            migrationBuilder.DropTable(
                name: "PhienThanhToan");

            migrationBuilder.RenameColumn(
                name: "ChiTietHoaDonId",
                table: "GiaoDichThanhToan",
                newName: "HoaDonId");

            migrationBuilder.RenameIndex(
                name: "IX_GiaoDichThanhToan_ChiTietHoaDonId",
                table: "GiaoDichThanhToan",
                newName: "IX_GiaoDichThanhToan_HoaDonId");

            migrationBuilder.CreateTable(
                name: "GiaoDichThanhToanChiTiet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChiTietHoaDonId = table.Column<int>(type: "int", nullable: false),
                    GiaoDichThanhToanId = table.Column<int>(type: "int", nullable: false),
                    SoTienPhanBo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiaoDichThanhToanChiTiet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GiaoDichThanhToanChiTiet_ChiTietHoaDon_ChiTietHoaDonId",
                        column: x => x.ChiTietHoaDonId,
                        principalTable: "ChiTietHoaDon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GiaoDichThanhToanChiTiet_GiaoDichThanhToan_GiaoDichThanhToanId",
                        column: x => x.GiaoDichThanhToanId,
                        principalTable: "GiaoDichThanhToan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GiaoDichThanhToanChiTiet_ChiTietHoaDonId",
                table: "GiaoDichThanhToanChiTiet",
                column: "ChiTietHoaDonId");

            migrationBuilder.CreateIndex(
                name: "IX_GiaoDichThanhToanChiTiet_GiaoDichThanhToanId_ChiTietHoaDonId",
                table: "GiaoDichThanhToanChiTiet",
                columns: new[] { "GiaoDichThanhToanId", "ChiTietHoaDonId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GiaoDichThanhToan_HoaDon_HoaDonId",
                table: "GiaoDichThanhToan",
                column: "HoaDonId",
                principalTable: "HoaDon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
