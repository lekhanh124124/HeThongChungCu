using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddToaNhaIdToThietBi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ToaNhaId",
                table: "ThietBi",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThietBi_ToaNhaId",
                table: "ThietBi",
                column: "ToaNhaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ThietBi_ToaNha_ToaNhaId",
                table: "ThietBi",
                column: "ToaNhaId",
                principalTable: "ToaNha",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThietBi_ToaNha_ToaNhaId",
                table: "ThietBi");

            migrationBuilder.DropIndex(
                name: "IX_ThietBi_ToaNhaId",
                table: "ThietBi");

            migrationBuilder.DropColumn(
                name: "ToaNhaId",
                table: "ThietBi");
        }
    }
}
