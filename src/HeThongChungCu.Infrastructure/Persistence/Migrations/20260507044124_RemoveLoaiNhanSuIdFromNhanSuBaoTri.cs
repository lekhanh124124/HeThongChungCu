using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLoaiNhanSuIdFromNhanSuBaoTri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoaiNhanSuId",
                table: "NhanSuBaoTri");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoaiNhanSuId",
                table: "NhanSuBaoTri",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
