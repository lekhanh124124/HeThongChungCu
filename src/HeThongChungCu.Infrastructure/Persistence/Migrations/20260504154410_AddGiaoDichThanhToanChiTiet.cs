using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGiaoDichThanhToanChiTiet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GiaoDichThanhToanChiTiet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GiaoDichThanhToanId = table.Column<int>(type: "int", nullable: false),
                    ChiTietHoaDonId = table.Column<int>(type: "int", nullable: false),
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GiaoDichThanhToanChiTiet");
        }
    }
}
