using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBangGiaCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BangGia_DichVu_DichVuId",
                table: "BangGia");

            migrationBuilder.AddForeignKey(
                name: "FK_BangGia_DichVu_DichVuId",
                table: "BangGia",
                column: "DichVuId",
                principalTable: "DichVu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BangGia_DichVu_DichVuId",
                table: "BangGia");

            migrationBuilder.AddForeignKey(
                name: "FK_BangGia_DichVu_DichVuId",
                table: "BangGia",
                column: "DichVuId",
                principalTable: "DichVu",
                principalColumn: "Id");
        }
    }
}
