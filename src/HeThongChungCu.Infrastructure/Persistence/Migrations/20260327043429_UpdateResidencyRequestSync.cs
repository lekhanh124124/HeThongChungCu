using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateResidencyRequestSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_YeuCauCuTru_QuanHeCuTru_QuanHeCuTruId",
                table: "YeuCauCuTru");

            migrationBuilder.DropIndex(
                name: "IX_YeuCauCuTru_QuanHeCuTruId",
                table: "YeuCauCuTru");

            migrationBuilder.AddColumn<int>(
                name: "TaiLieuCuTruId",
                table: "YeuCauTaiLieuCuTru",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaiLieuCuTruId",
                table: "YeuCauTaiLieuCuTru");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauCuTru_QuanHeCuTruId",
                table: "YeuCauCuTru",
                column: "QuanHeCuTruId");

            migrationBuilder.AddForeignKey(
                name: "FK_YeuCauCuTru_QuanHeCuTru_QuanHeCuTruId",
                table: "YeuCauCuTru",
                column: "QuanHeCuTruId",
                principalTable: "QuanHeCuTru",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
