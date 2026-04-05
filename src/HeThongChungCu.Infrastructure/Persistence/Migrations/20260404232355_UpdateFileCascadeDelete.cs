using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFileCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TepTaiLieu_PhuongTien_PhuongTienId",
                table: "TepTaiLieu");

            migrationBuilder.DropForeignKey(
                name: "FK_TepTaiLieu_TaiLieu_TaiLieuId",
                table: "TepTaiLieu");

            migrationBuilder.DropForeignKey(
                name: "FK_TepTaiLieu_YeuCau_YeuCauId",
                table: "TepTaiLieu");

            migrationBuilder.AddForeignKey(
                name: "FK_TepTaiLieu_PhuongTien_PhuongTienId",
                table: "TepTaiLieu",
                column: "PhuongTienId",
                principalTable: "PhuongTien",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TepTaiLieu_TaiLieu_TaiLieuId",
                table: "TepTaiLieu",
                column: "TaiLieuId",
                principalTable: "TaiLieu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TepTaiLieu_YeuCau_YeuCauId",
                table: "TepTaiLieu",
                column: "YeuCauId",
                principalTable: "YeuCau",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TepTaiLieu_PhuongTien_PhuongTienId",
                table: "TepTaiLieu");

            migrationBuilder.DropForeignKey(
                name: "FK_TepTaiLieu_TaiLieu_TaiLieuId",
                table: "TepTaiLieu");

            migrationBuilder.DropForeignKey(
                name: "FK_TepTaiLieu_YeuCau_YeuCauId",
                table: "TepTaiLieu");

            migrationBuilder.AddForeignKey(
                name: "FK_TepTaiLieu_PhuongTien_PhuongTienId",
                table: "TepTaiLieu",
                column: "PhuongTienId",
                principalTable: "PhuongTien",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TepTaiLieu_TaiLieu_TaiLieuId",
                table: "TepTaiLieu",
                column: "TaiLieuId",
                principalTable: "TaiLieu",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TepTaiLieu_YeuCau_YeuCauId",
                table: "TepTaiLieu",
                column: "YeuCauId",
                principalTable: "YeuCau",
                principalColumn: "Id");
        }
    }
}
