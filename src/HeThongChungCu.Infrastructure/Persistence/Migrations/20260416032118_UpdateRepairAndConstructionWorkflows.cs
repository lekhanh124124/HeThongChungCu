using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRepairAndConstructionWorkflows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NhanVienThucHienId",
                table: "YeuCau",
                newName: "HopDongDoiTacId");

            migrationBuilder.AddColumn<decimal>(
                name: "ChiPhiDuKien",
                table: "YeuCau",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ChiPhiThucTe",
                table: "YeuCau",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhiChuBaoGia",
                table: "YeuCau",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhiChuThuCoc",
                table: "YeuCau",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDaThuCoc",
                table: "YeuCau",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMienPhi",
                table: "YeuCau",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "NgayDuyetSoBo",
                table: "YeuCau",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TienDatCoc",
                table: "YeuCau",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NhanSuYeuCau",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YeuCauId = table.Column<int>(type: "int", nullable: false),
                    NhanVienId = table.Column<int>(type: "int", nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoCCCD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    VaiTro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LoaiNhanSu = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanSuYeuCau", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NhanSuYeuCau_NhanVien_NhanVienId",
                        column: x => x.NhanVienId,
                        principalTable: "NhanVien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NhanSuYeuCau_YeuCau_YeuCauId",
                        column: x => x.YeuCauId,
                        principalTable: "YeuCau",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_YeuCau_HopDongDoiTacId",
                table: "YeuCau",
                column: "HopDongDoiTacId");

            migrationBuilder.CreateIndex(
                name: "IX_NhanSuYeuCau_NhanVienId",
                table: "NhanSuYeuCau",
                column: "NhanVienId");

            migrationBuilder.CreateIndex(
                name: "IX_NhanSuYeuCau_YeuCauId",
                table: "NhanSuYeuCau",
                column: "YeuCauId");

            migrationBuilder.AddForeignKey(
                name: "FK_YeuCau_HopDongDoiTac_HopDongDoiTacId",
                table: "YeuCau",
                column: "HopDongDoiTacId",
                principalTable: "HopDongDoiTac",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_YeuCau_HopDongDoiTac_HopDongDoiTacId",
                table: "YeuCau");

            migrationBuilder.DropTable(
                name: "NhanSuYeuCau");

            migrationBuilder.DropIndex(
                name: "IX_YeuCau_HopDongDoiTacId",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "ChiPhiDuKien",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "ChiPhiThucTe",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "GhiChuBaoGia",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "GhiChuThuCoc",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "IsDaThuCoc",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "IsMienPhi",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "NgayDuyetSoBo",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "TienDatCoc",
                table: "YeuCau");

            migrationBuilder.RenameColumn(
                name: "HopDongDoiTacId",
                table: "YeuCau",
                newName: "NhanVienThucHienId");
        }
    }
}
