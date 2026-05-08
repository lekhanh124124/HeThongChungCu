using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUngVienIdToLuaChonKhaoSat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UngVienId",
                table: "LuaChonKhaoSat",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UngVienId",
                table: "LuaChonKhaoSat");
        }
    }
}
