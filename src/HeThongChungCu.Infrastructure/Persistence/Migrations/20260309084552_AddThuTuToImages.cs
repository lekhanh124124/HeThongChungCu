using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddThuTuToImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnhDaiDienUrl",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiaChi",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DiaChi",
                table: "ToaNhas",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MoTa",
                table: "ToaNhas",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoTangHam",
                table: "ToaNhas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TrangThaiToaNhaId",
                table: "ToaNhas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "QuanHeCuTrus",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoaiCanHoId",
                table: "CanHos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CanHoHinhAnhs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CanHoId = table.Column<int>(type: "int", nullable: false),
                    HinhAnhUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsThumbnail = table.Column<bool>(type: "bit", nullable: false),
                    ThuTu = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanHoHinhAnhs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanHoHinhAnhs_CanHos_CanHoId",
                        column: x => x.CanHoId,
                        principalTable: "CanHos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiSoTieuThus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CanHoId = table.Column<int>(type: "int", nullable: false),
                    LoaiDichVuId = table.Column<int>(type: "int", nullable: false),
                    ChiSoCu = table.Column<double>(type: "float", nullable: false),
                    ChiSoMoi = table.Column<double>(type: "float", nullable: false),
                    Thang = table.Column<int>(type: "int", nullable: false),
                    Nam = table.Column<int>(type: "int", nullable: false),
                    NgayChot = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiSoTieuThus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiSoTieuThus_CanHos_CanHoId",
                        column: x => x.CanHoId,
                        principalTable: "CanHos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhuongTiens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CanHoId = table.Column<int>(type: "int", nullable: false),
                    TenPhuongTien = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LoaiPhuongTienId = table.Column<int>(type: "int", nullable: false),
                    BienSo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MauXe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuongTiens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhuongTiens_CanHos_CanHoId",
                        column: x => x.CanHoId,
                        principalTable: "CanHos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ToaNhaHinhAnhs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ToaNhaId = table.Column<int>(type: "int", nullable: false),
                    HinhAnhUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsThumbnail = table.Column<bool>(type: "bit", nullable: false),
                    ThuTu = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToaNhaHinhAnhs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToaNhaHinhAnhs_ToaNhas_ToaNhaId",
                        column: x => x.ToaNhaId,
                        principalTable: "ToaNhas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThePhuongTiens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhuongTienId = table.Column<int>(type: "int", nullable: false),
                    MaThe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThePhuongTiens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThePhuongTiens_PhuongTiens_PhuongTienId",
                        column: x => x.PhuongTienId,
                        principalTable: "PhuongTiens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuanHeCuTrus_UserId1",
                table: "QuanHeCuTrus",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_CanHoHinhAnhs_CanHoId",
                table: "CanHoHinhAnhs",
                column: "CanHoId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiSoTieuThus_CanHoId",
                table: "ChiSoTieuThus",
                column: "CanHoId");

            migrationBuilder.CreateIndex(
                name: "IX_PhuongTiens_CanHoId",
                table: "PhuongTiens",
                column: "CanHoId");

            migrationBuilder.CreateIndex(
                name: "IX_ThePhuongTiens_PhuongTienId",
                table: "ThePhuongTiens",
                column: "PhuongTienId");

            migrationBuilder.CreateIndex(
                name: "IX_ToaNhaHinhAnhs_ToaNhaId",
                table: "ToaNhaHinhAnhs",
                column: "ToaNhaId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuanHeCuTrus_Users_UserId1",
                table: "QuanHeCuTrus",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuanHeCuTrus_Users_UserId1",
                table: "QuanHeCuTrus");

            migrationBuilder.DropTable(
                name: "CanHoHinhAnhs");

            migrationBuilder.DropTable(
                name: "ChiSoTieuThus");

            migrationBuilder.DropTable(
                name: "ThePhuongTiens");

            migrationBuilder.DropTable(
                name: "ToaNhaHinhAnhs");

            migrationBuilder.DropTable(
                name: "PhuongTiens");

            migrationBuilder.DropIndex(
                name: "IX_QuanHeCuTrus_UserId1",
                table: "QuanHeCuTrus");

            migrationBuilder.DropColumn(
                name: "AnhDaiDienUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DiaChi",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DiaChi",
                table: "ToaNhas");

            migrationBuilder.DropColumn(
                name: "MoTa",
                table: "ToaNhas");

            migrationBuilder.DropColumn(
                name: "SoTangHam",
                table: "ToaNhas");

            migrationBuilder.DropColumn(
                name: "TrangThaiToaNhaId",
                table: "ToaNhas");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "QuanHeCuTrus");

            migrationBuilder.DropColumn(
                name: "LoaiCanHoId",
                table: "CanHos");
        }
    }
}
