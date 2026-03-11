using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSoTangFromToaNha : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoTang",
                table: "ToaNhas");

            migrationBuilder.DropColumn(
                name: "SoTangHam",
                table: "ToaNhas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SoTang",
                table: "ToaNhas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SoTangHam",
                table: "ToaNhas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
