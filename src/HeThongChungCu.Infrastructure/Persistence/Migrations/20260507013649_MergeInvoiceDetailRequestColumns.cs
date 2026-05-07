using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MergeInvoiceDetailRequestColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietHoaDon_YeuCau_YeuCauSuaChuaId",
                table: "ChiTietHoaDon");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietHoaDon_YeuCau_YeuCauThiCongId",
                table: "ChiTietHoaDon");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietHoaDon_YeuCauSuaChuaId",
                table: "ChiTietHoaDon");

            migrationBuilder.DropColumn(
                name: "YeuCauSuaChuaId",
                table: "ChiTietHoaDon");

            migrationBuilder.RenameColumn(
                name: "YeuCauThiCongId",
                table: "ChiTietHoaDon",
                newName: "YeuCauId");

            migrationBuilder.RenameIndex(
                name: "IX_ChiTietHoaDon_YeuCauThiCongId",
                table: "ChiTietHoaDon",
                newName: "IX_ChiTietHoaDon_YeuCauId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietHoaDon_YeuCau_YeuCauId",
                table: "ChiTietHoaDon",
                column: "YeuCauId",
                principalTable: "YeuCau",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietHoaDon_YeuCau_YeuCauId",
                table: "ChiTietHoaDon");

            migrationBuilder.RenameColumn(
                name: "YeuCauId",
                table: "ChiTietHoaDon",
                newName: "YeuCauThiCongId");

            migrationBuilder.RenameIndex(
                name: "IX_ChiTietHoaDon_YeuCauId",
                table: "ChiTietHoaDon",
                newName: "IX_ChiTietHoaDon_YeuCauThiCongId");

            migrationBuilder.AddColumn<int>(
                name: "YeuCauSuaChuaId",
                table: "ChiTietHoaDon",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoaDon_YeuCauSuaChuaId",
                table: "ChiTietHoaDon",
                column: "YeuCauSuaChuaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietHoaDon_YeuCau_YeuCauSuaChuaId",
                table: "ChiTietHoaDon",
                column: "YeuCauSuaChuaId",
                principalTable: "YeuCau",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietHoaDon_YeuCau_YeuCauThiCongId",
                table: "ChiTietHoaDon",
                column: "YeuCauThiCongId",
                principalTable: "YeuCau",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
