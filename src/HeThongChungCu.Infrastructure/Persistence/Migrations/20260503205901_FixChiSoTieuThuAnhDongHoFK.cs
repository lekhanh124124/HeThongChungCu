using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixChiSoTieuThuAnhDongHoFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiSoTieuThu_TaiLieu_AnhDongHoId",
                table: "ChiSoTieuThu");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiSoTieuThu_TepTaiLieu_AnhDongHoId",
                table: "ChiSoTieuThu",
                column: "AnhDongHoId",
                principalTable: "TepTaiLieu",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiSoTieuThu_TepTaiLieu_AnhDongHoId",
                table: "ChiSoTieuThu");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiSoTieuThu_TaiLieu_AnhDongHoId",
                table: "ChiSoTieuThu",
                column: "AnhDongHoId",
                principalTable: "TaiLieu",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
