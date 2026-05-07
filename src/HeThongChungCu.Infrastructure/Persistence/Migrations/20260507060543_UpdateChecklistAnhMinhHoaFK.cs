using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChecklistAnhMinhHoaFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnhMinhHoaUrl",
                table: "PhieuBaoTriChecklist");

            migrationBuilder.AddColumn<int>(
                name: "AnhMinhHoaId",
                table: "PhieuBaoTriChecklist",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhieuBaoTriChecklist_AnhMinhHoaId",
                table: "PhieuBaoTriChecklist",
                column: "AnhMinhHoaId");

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuBaoTriChecklist_TepTaiLieu_AnhMinhHoaId",
                table: "PhieuBaoTriChecklist",
                column: "AnhMinhHoaId",
                principalTable: "TepTaiLieu",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhieuBaoTriChecklist_TepTaiLieu_AnhMinhHoaId",
                table: "PhieuBaoTriChecklist");

            migrationBuilder.DropIndex(
                name: "IX_PhieuBaoTriChecklist_AnhMinhHoaId",
                table: "PhieuBaoTriChecklist");

            migrationBuilder.DropColumn(
                name: "AnhMinhHoaId",
                table: "PhieuBaoTriChecklist");

            migrationBuilder.AddColumn<string>(
                name: "AnhMinhHoaUrl",
                table: "PhieuBaoTriChecklist",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
