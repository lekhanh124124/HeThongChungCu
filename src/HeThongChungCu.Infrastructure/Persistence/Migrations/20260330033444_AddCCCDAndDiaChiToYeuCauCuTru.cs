using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCCCDAndDiaChiToYeuCauCuTru : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YeuCauCCCD",
                table: "YeuCauCuTru",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YeuCauDiaChi",
                table: "YeuCauCuTru",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YeuCauCCCD",
                table: "YeuCauCuTru");

            migrationBuilder.DropColumn(
                name: "YeuCauDiaChi",
                table: "YeuCauCuTru");
        }
    }
}
