using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConsolidateFinancialDetailColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChiTietQuyChi_GhiChu",
                table: "ChiTietQuyThuChi");

            migrationBuilder.DropColumn(
                name: "ChiTietQuyChi_NhomThongKe",
                table: "ChiTietQuyThuChi");

            migrationBuilder.DropColumn(
                name: "YeuCauSuaChuaId",
                table: "ChiTietQuyThuChi");

            migrationBuilder.RenameColumn(
                name: "YeuCauThiCongId",
                table: "ChiTietQuyThuChi",
                newName: "YeuCauId");

            migrationBuilder.AlterColumn<string>(
                name: "NhomThongKe",
                table: "ChiTietQuyThuChi",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietQuyThuChi_YeuCauId",
                table: "ChiTietQuyThuChi",
                column: "YeuCauId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietQuyThuChi_YeuCau_YeuCauId",
                table: "ChiTietQuyThuChi",
                column: "YeuCauId",
                principalTable: "YeuCau",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietQuyThuChi_YeuCau_YeuCauId",
                table: "ChiTietQuyThuChi");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietQuyThuChi_YeuCauId",
                table: "ChiTietQuyThuChi");

            migrationBuilder.RenameColumn(
                name: "YeuCauId",
                table: "ChiTietQuyThuChi",
                newName: "YeuCauThiCongId");

            migrationBuilder.AlterColumn<string>(
                name: "NhomThongKe",
                table: "ChiTietQuyThuChi",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChiTietQuyChi_GhiChu",
                table: "ChiTietQuyThuChi",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChiTietQuyChi_NhomThongKe",
                table: "ChiTietQuyThuChi",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YeuCauSuaChuaId",
                table: "ChiTietQuyThuChi",
                type: "int",
                nullable: true);
        }
    }
}
