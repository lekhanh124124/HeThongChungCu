using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTPHQuyThuChi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietQuyThuChi_YeuCau_YeuCauId",
                table: "ChiTietQuyThuChi");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietQuyThuChi_YeuCauId",
                table: "ChiTietQuyThuChi");

            migrationBuilder.DropColumn(
                name: "LoaiGiaoDichId",
                table: "ChiTietQuyThuChi");

            migrationBuilder.DropColumn(
                name: "YeuCauId",
                table: "ChiTietQuyThuChi");

            migrationBuilder.AlterColumn<string>(
                name: "NhomThongKe",
                table: "ChiTietQuyThuChi",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NhomThongKe",
                table: "ChiTietQuyThuChi",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<int>(
                name: "LoaiGiaoDichId",
                table: "ChiTietQuyThuChi",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "YeuCauId",
                table: "ChiTietQuyThuChi",
                type: "int",
                nullable: true);

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
    }
}
