using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePhuongTienImagesAndCardStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "ThePhuongTien");

            migrationBuilder.RenameColumn(
                name: "QuanHeCuTruId",
                table: "YeuCauCuTru",
                newName: "YeuCauQuanHeCuTruId");

            migrationBuilder.AddColumn<int>(
                name: "TrangThaiId",
                table: "ThePhuongTien",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TepHinhAnhPhuongTien",
                columns: table => new
                {
                    HinhAnhPhuongTiensId = table.Column<int>(type: "int", nullable: false),
                    PhuongTienId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TepHinhAnhPhuongTien", x => new { x.HinhAnhPhuongTiensId, x.PhuongTienId });
                    table.ForeignKey(
                        name: "FK_TepHinhAnhPhuongTien_PhuongTien_PhuongTienId",
                        column: x => x.PhuongTienId,
                        principalTable: "PhuongTien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TepHinhAnhPhuongTien_TepTaiLieu_HinhAnhPhuongTiensId",
                        column: x => x.HinhAnhPhuongTiensId,
                        principalTable: "TepTaiLieu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YeuCauPhuongTien",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CanHoId = table.Column<int>(type: "int", nullable: false),
                    YeuCauPhuongTienId = table.Column<int>(type: "int", nullable: true),
                    LoaiYeuCauId = table.Column<int>(type: "int", nullable: false),
                    TrangThaiId = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LyDo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NguoiXuLyId = table.Column<int>(type: "int", nullable: true),
                    NgayXuLy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    YeuCauTenPhuongTien = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    YeuCauLoaiPhuongTienId = table.Column<int>(type: "int", nullable: false),
                    YeuCauBienSo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    YeuCauMauXe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCauPhuongTien", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YeuCauPhuongTien_CanHo_CanHoId",
                        column: x => x.CanHoId,
                        principalTable: "CanHo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_YeuCauPhuongTien_PhuongTien_YeuCauPhuongTienId",
                        column: x => x.YeuCauPhuongTienId,
                        principalTable: "PhuongTien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TepYeuCauHinhAnhPhuongTien",
                columns: table => new
                {
                    YeuCauHinhAnhPhuongTiensId = table.Column<int>(type: "int", nullable: false),
                    YeuCauPhuongTienId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TepYeuCauHinhAnhPhuongTien", x => new { x.YeuCauHinhAnhPhuongTiensId, x.YeuCauPhuongTienId });
                    table.ForeignKey(
                        name: "FK_TepYeuCauHinhAnhPhuongTien_TepTaiLieu_YeuCauHinhAnhPhuongTiensId",
                        column: x => x.YeuCauHinhAnhPhuongTiensId,
                        principalTable: "TepTaiLieu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TepYeuCauHinhAnhPhuongTien_YeuCauPhuongTien_YeuCauPhuongTienId",
                        column: x => x.YeuCauPhuongTienId,
                        principalTable: "YeuCauPhuongTien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TepHinhAnhPhuongTien_PhuongTienId",
                table: "TepHinhAnhPhuongTien",
                column: "PhuongTienId");

            migrationBuilder.CreateIndex(
                name: "IX_TepYeuCauHinhAnhPhuongTien_YeuCauPhuongTienId",
                table: "TepYeuCauHinhAnhPhuongTien",
                column: "YeuCauPhuongTienId");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauPhuongTien_CanHoId",
                table: "YeuCauPhuongTien",
                column: "CanHoId");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauPhuongTien_YeuCauPhuongTienId",
                table: "YeuCauPhuongTien",
                column: "YeuCauPhuongTienId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TepHinhAnhPhuongTien");

            migrationBuilder.DropTable(
                name: "TepYeuCauHinhAnhPhuongTien");

            migrationBuilder.DropTable(
                name: "YeuCauPhuongTien");

            migrationBuilder.DropColumn(
                name: "TrangThaiId",
                table: "ThePhuongTien");

            migrationBuilder.RenameColumn(
                name: "YeuCauQuanHeCuTruId",
                table: "YeuCauCuTru",
                newName: "QuanHeCuTruId");

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "ThePhuongTien",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
