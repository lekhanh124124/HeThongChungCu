using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTenCanHoToCanHo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenCanHo",
                table: "CanHos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenCanHo",
                table: "CanHos");
        }
    }
}
