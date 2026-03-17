using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorDomainAndPolicyIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuanHeCuTrus_Users_UserId1",
                table: "QuanHeCuTrus");

            migrationBuilder.DropIndex(
                name: "IX_QuanHeCuTrus_UserId1",
                table: "QuanHeCuTrus");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "QuanHeCuTrus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "QuanHeCuTrus",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuanHeCuTrus_UserId1",
                table: "QuanHeCuTrus",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_QuanHeCuTrus_Users_UserId1",
                table: "QuanHeCuTrus",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
