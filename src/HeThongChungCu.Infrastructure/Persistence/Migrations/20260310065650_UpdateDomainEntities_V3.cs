using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDomainEntities_V3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChiSoCu",
                table: "ChiSoTieuThus");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                table: "QuanHeCuTrus",
                newName: "IsKetThuc");

            migrationBuilder.RenameColumn(
                name: "ChiSoMoi",
                table: "ChiSoTieuThus",
                newName: "ChiSo");

            migrationBuilder.AddColumn<bool>(
                name: "IsLock",
                table: "ChiSoTieuThus",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLock",
                table: "ChiSoTieuThus");

            migrationBuilder.RenameColumn(
                name: "IsKetThuc",
                table: "QuanHeCuTrus",
                newName: "TrangThai");

            migrationBuilder.RenameColumn(
                name: "ChiSo",
                table: "ChiSoTieuThus",
                newName: "ChiSoMoi");

            migrationBuilder.AddColumn<double>(
                name: "ChiSoCu",
                table: "ChiSoTieuThus",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
