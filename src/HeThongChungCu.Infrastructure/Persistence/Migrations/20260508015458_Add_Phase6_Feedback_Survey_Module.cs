using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_Phase6_Feedback_Survey_Module : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiemDanhGia",
                table: "YeuCau",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoaiPhanAnhId",
                table: "YeuCau",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "NgayDanhGia",
                table: "YeuCau",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NhanXetDanhGia",
                table: "YeuCau",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TieuDe",
                table: "YeuCau",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrangThaiPhanAnhId",
                table: "YeuCau",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "KhaoSat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TieuDe = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    LoaiKhaoSatId = table.Column<int>(type: "int", nullable: false),
                    CoCheTinhDiemId = table.Column<int>(type: "int", nullable: false),
                    TrangThaiId = table.Column<int>(type: "int", nullable: false),
                    NgayBatDau = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    NgayKetThuc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TyleThamGiaToiThieu = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    TyLeDongYToiThieu = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    IsAnDanh = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhaoSat", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TraLoiPhanAnh",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YeuCauPhanAnhId = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsNhanVien = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraLoiPhanAnh", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TraLoiPhanAnh_YeuCau_YeuCauPhanAnhId",
                        column: x => x.YeuCauPhanAnhId,
                        principalTable: "YeuCau",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BieuQuyetCuDan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KhaoSatId = table.Column<int>(type: "int", nullable: false),
                    CanHoId = table.Column<int>(type: "int", nullable: false),
                    TrongSoBieuQuyet = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    IsOtpVerified = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BieuQuyetCuDan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BieuQuyetCuDan_CanHo_CanHoId",
                        column: x => x.CanHoId,
                        principalTable: "CanHo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BieuQuyetCuDan_KhaoSat_KhaoSatId",
                        column: x => x.KhaoSatId,
                        principalTable: "KhaoSat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CauHoiKhaoSat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KhaoSatId = table.Column<int>(type: "int", nullable: false),
                    NoiDungCauHoi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsBatBuoc = table.Column<bool>(type: "bit", nullable: false),
                    IsMultiSelect = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHoiKhaoSat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CauHoiKhaoSat_KhaoSat_KhaoSatId",
                        column: x => x.KhaoSatId,
                        principalTable: "KhaoSat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LuaChonKhaoSat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CauHoiKhaoSatId = table.Column<int>(type: "int", nullable: false),
                    NoiDungLuaChon = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsUngVienBQT = table.Column<bool>(type: "bit", nullable: false),
                    TieuSuUngVien = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LuaChonKhaoSat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LuaChonKhaoSat_CauHoiKhaoSat_CauHoiKhaoSatId",
                        column: x => x.CauHoiKhaoSatId,
                        principalTable: "CauHoiKhaoSat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietBieuQuyet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BieuQuyetCuDanId = table.Column<int>(type: "int", nullable: false),
                    LuaChonKhaoSatId = table.Column<int>(type: "int", nullable: false),
                    NoiDungTraLoiTuDo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietBieuQuyet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietBieuQuyet_BieuQuyetCuDan_BieuQuyetCuDanId",
                        column: x => x.BieuQuyetCuDanId,
                        principalTable: "BieuQuyetCuDan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietBieuQuyet_LuaChonKhaoSat_LuaChonKhaoSatId",
                        column: x => x.LuaChonKhaoSatId,
                        principalTable: "LuaChonKhaoSat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BieuQuyetCuDan_CanHoId",
                table: "BieuQuyetCuDan",
                column: "CanHoId");

            migrationBuilder.CreateIndex(
                name: "IX_BieuQuyetCuDan_KhaoSatId",
                table: "BieuQuyetCuDan",
                column: "KhaoSatId");

            migrationBuilder.CreateIndex(
                name: "IX_CauHoiKhaoSat_KhaoSatId",
                table: "CauHoiKhaoSat",
                column: "KhaoSatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietBieuQuyet_BieuQuyetCuDanId",
                table: "ChiTietBieuQuyet",
                column: "BieuQuyetCuDanId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietBieuQuyet_LuaChonKhaoSatId",
                table: "ChiTietBieuQuyet",
                column: "LuaChonKhaoSatId");

            migrationBuilder.CreateIndex(
                name: "IX_LuaChonKhaoSat_CauHoiKhaoSatId",
                table: "LuaChonKhaoSat",
                column: "CauHoiKhaoSatId");

            migrationBuilder.CreateIndex(
                name: "IX_TraLoiPhanAnh_YeuCauPhanAnhId",
                table: "TraLoiPhanAnh",
                column: "YeuCauPhanAnhId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietBieuQuyet");

            migrationBuilder.DropTable(
                name: "TraLoiPhanAnh");

            migrationBuilder.DropTable(
                name: "BieuQuyetCuDan");

            migrationBuilder.DropTable(
                name: "LuaChonKhaoSat");

            migrationBuilder.DropTable(
                name: "CauHoiKhaoSat");

            migrationBuilder.DropTable(
                name: "KhaoSat");

            migrationBuilder.DropColumn(
                name: "DiemDanhGia",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "LoaiPhanAnhId",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "NgayDanhGia",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "NhanXetDanhGia",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "TieuDe",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "TrangThaiPhanAnhId",
                table: "YeuCau");
        }
    }
}
