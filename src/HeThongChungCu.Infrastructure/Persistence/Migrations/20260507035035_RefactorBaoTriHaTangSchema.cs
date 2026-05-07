using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorBaoTriHaTangSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhieuBaoTri_DoiTac_DoiTacId",
                table: "PhieuBaoTri");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuBaoTri_YeuCau_YeuCauSuaChuaId",
                table: "PhieuBaoTri");

            migrationBuilder.DropIndex(
                name: "IX_PhieuBaoTri_DoiTacId",
                table: "PhieuBaoTri");

            migrationBuilder.DropIndex(
                name: "IX_PhieuBaoTri_YeuCauSuaChuaId",
                table: "PhieuBaoTri");

            migrationBuilder.DropColumn(
                name: "DoiTacId",
                table: "PhieuBaoTri");

            migrationBuilder.DropColumn(
                name: "LoaiBaoTri",
                table: "PhieuBaoTri");

            migrationBuilder.DropColumn(
                name: "YeuCauSuaChuaId",
                table: "PhieuBaoTri");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DoiTacId",
                table: "PhieuBaoTri",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoaiBaoTri",
                table: "PhieuBaoTri",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "YeuCauSuaChuaId",
                table: "PhieuBaoTri",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhieuBaoTri_DoiTacId",
                table: "PhieuBaoTri",
                column: "DoiTacId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuBaoTri_YeuCauSuaChuaId",
                table: "PhieuBaoTri",
                column: "YeuCauSuaChuaId");

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuBaoTri_DoiTac_DoiTacId",
                table: "PhieuBaoTri",
                column: "DoiTacId",
                principalTable: "DoiTac",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuBaoTri_YeuCau_YeuCauSuaChuaId",
                table: "PhieuBaoTri",
                column: "YeuCauSuaChuaId",
                principalTable: "YeuCau",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
