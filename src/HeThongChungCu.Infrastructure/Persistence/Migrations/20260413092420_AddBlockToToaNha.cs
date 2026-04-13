using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBlockToToaNha : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Block",
                table: "ToaNha",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Block",
                table: "ToaNha");
        }
    }
}
