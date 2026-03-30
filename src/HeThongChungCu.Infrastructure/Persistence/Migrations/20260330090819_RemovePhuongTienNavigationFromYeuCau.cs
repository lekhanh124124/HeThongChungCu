using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovePhuongTienNavigationFromYeuCau : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_YeuCauPhuongTien_PhuongTien_YeuCauPhuongTienId",
                table: "YeuCauPhuongTien");

            migrationBuilder.DropIndex(
                name: "IX_YeuCauPhuongTien_YeuCauPhuongTienId",
                table: "YeuCauPhuongTien");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_YeuCauPhuongTien_YeuCauPhuongTienId",
                table: "YeuCauPhuongTien",
                column: "YeuCauPhuongTienId");

            migrationBuilder.AddForeignKey(
                name: "FK_YeuCauPhuongTien_PhuongTien_YeuCauPhuongTienId",
                table: "YeuCauPhuongTien",
                column: "YeuCauPhuongTienId",
                principalTable: "PhuongTien",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
