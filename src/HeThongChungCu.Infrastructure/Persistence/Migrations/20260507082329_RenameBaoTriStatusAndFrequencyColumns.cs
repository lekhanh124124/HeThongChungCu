using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameBaoTriStatusAndFrequencyColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TrangThai",
                table: "ThietBi",
                newName: "TrangThaiThietBiId");

            migrationBuilder.RenameIndex(
                name: "IX_ThietBi_TrangThai",
                table: "ThietBi",
                newName: "IX_ThietBi_TrangThaiThietBiId");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                table: "PhieuBaoTri",
                newName: "TrangThaiPhieuBaoTriId");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuBaoTri_TrangThai",
                table: "PhieuBaoTri",
                newName: "IX_PhieuBaoTri_TrangThaiPhieuBaoTriId");

            migrationBuilder.RenameColumn(
                name: "TanSuat",
                table: "LichBaoTri",
                newName: "TanSuatBaoTriId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TrangThaiThietBiId",
                table: "ThietBi",
                newName: "TrangThai");

            migrationBuilder.RenameIndex(
                name: "IX_ThietBi_TrangThaiThietBiId",
                table: "ThietBi",
                newName: "IX_ThietBi_TrangThai");

            migrationBuilder.RenameColumn(
                name: "TrangThaiPhieuBaoTriId",
                table: "PhieuBaoTri",
                newName: "TrangThai");

            migrationBuilder.RenameIndex(
                name: "IX_PhieuBaoTri_TrangThaiPhieuBaoTriId",
                table: "PhieuBaoTri",
                newName: "IX_PhieuBaoTri_TrangThai");

            migrationBuilder.RenameColumn(
                name: "TanSuatBaoTriId",
                table: "LichBaoTri",
                newName: "TanSuat");
        }
    }
}
