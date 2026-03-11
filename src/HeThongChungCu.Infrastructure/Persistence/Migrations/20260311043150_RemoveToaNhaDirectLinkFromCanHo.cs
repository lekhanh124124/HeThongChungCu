using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveToaNhaDirectLinkFromCanHo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CanHos_ToaNhas_ToaNhaId",
                table: "CanHos");

            migrationBuilder.DropIndex(
                name: "IX_CanHos_ToaNhaId",
                table: "CanHos");

            migrationBuilder.DropColumn(
                name: "ToaNhaId",
                table: "CanHos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ToaNhaId",
                table: "CanHos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CanHos_ToaNhaId",
                table: "CanHos",
                column: "ToaNhaId");

            migrationBuilder.AddForeignKey(
                name: "FK_CanHos_ToaNhas_ToaNhaId",
                table: "CanHos",
                column: "ToaNhaId",
                principalTable: "ToaNhas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
