using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorTaiChinh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThuChiQuy");

            migrationBuilder.CreateTable(
                name: "QuyThuChi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaGiaoDich = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TongSoTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayGiaoDich = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PhuongThucThanhToanId = table.Column<int>(type: "int", nullable: false),
                    NguoiGiaoDich = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ChungTuGoc = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LoaiGiaoDichId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuyThuChi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietQuyThuChi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuyThuChiId = table.Column<int>(type: "int", nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoaiGiaoDichId = table.Column<int>(type: "int", nullable: false),
                    ChiTietQuyChi_NhomThongKe = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ChiTietQuyChi_GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DichVuId = table.Column<int>(type: "int", nullable: true),
                    YeuCauThiCongId = table.Column<int>(type: "int", nullable: true),
                    YeuCauSuaChuaId = table.Column<int>(type: "int", nullable: true),
                    NhomThongKe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietQuyThuChi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietQuyThuChi_DichVu_DichVuId",
                        column: x => x.DichVuId,
                        principalTable: "DichVu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChiTietQuyThuChi_QuyThuChi_QuyThuChiId",
                        column: x => x.QuyThuChiId,
                        principalTable: "QuyThuChi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietQuyThuChi_DichVuId",
                table: "ChiTietQuyThuChi",
                column: "DichVuId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietQuyThuChi_QuyThuChiId",
                table: "ChiTietQuyThuChi",
                column: "QuyThuChiId");

            migrationBuilder.CreateIndex(
                name: "IX_QuyThuChi_LoaiGiaoDichId",
                table: "QuyThuChi",
                column: "LoaiGiaoDichId");

            migrationBuilder.CreateIndex(
                name: "IX_QuyThuChi_MaGiaoDich",
                table: "QuyThuChi",
                column: "MaGiaoDich",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuyThuChi_NgayGiaoDich",
                table: "QuyThuChi",
                column: "NgayGiaoDich");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietQuyThuChi");

            migrationBuilder.DropTable(
                name: "QuyThuChi");

            migrationBuilder.CreateTable(
                name: "ThuChiQuy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChungTuGoc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    KhoanMucId = table.Column<int>(type: "int", nullable: false),
                    LoaiGiaoDichId = table.Column<int>(type: "int", nullable: false),
                    MaGiaoDich = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    NgayGiaoDich = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    NguoiGiaoDich = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PhuongThucThanhToanId = table.Column<int>(type: "int", nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThuChiQuy", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThuChiQuy_KhoanMucId",
                table: "ThuChiQuy",
                column: "KhoanMucId");

            migrationBuilder.CreateIndex(
                name: "IX_ThuChiQuy_LoaiGiaoDichId",
                table: "ThuChiQuy",
                column: "LoaiGiaoDichId");

            migrationBuilder.CreateIndex(
                name: "IX_ThuChiQuy_NgayGiaoDich",
                table: "ThuChiQuy",
                column: "NgayGiaoDich");
        }
    }
}
