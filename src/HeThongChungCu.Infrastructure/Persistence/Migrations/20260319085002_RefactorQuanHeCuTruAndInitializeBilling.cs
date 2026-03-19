using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorQuanHeCuTruAndInitializeBilling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiSoTieuThus_CanHos_CanHoId",
                table: "ChiSoTieuThus");

            migrationBuilder.DropColumn(
                name: "IsKetThuc",
                table: "QuanHeCuTrus");

            migrationBuilder.RenameColumn(
                name: "LoaiDichVuId",
                table: "ChiSoTieuThus",
                newName: "DichVuId");

            migrationBuilder.RenameColumn(
                name: "ChiSo",
                table: "ChiSoTieuThus",
                newName: "ChiSoMoi");

            migrationBuilder.AddColumn<int>(
                name: "TrangThaiCuTruId",
                table: "QuanHeCuTrus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "ChiSoCu",
                table: "ChiSoTieuThus",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "BangGias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DichVuId = table.Column<int>(type: "int", nullable: false),
                    TenBangGia = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NgayApDung = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoaiDinhGiaId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BangGias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CauHinhLais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCauHinh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LaiSuatThang = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoNgayChoPhep = table.Column<int>(type: "int", nullable: false),
                    NguongQuaHanNhe = table.Column<int>(type: "int", nullable: false),
                    NguongQuaHanNang = table.Column<int>(type: "int", nullable: false),
                    NgayApDung = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHinhLais", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DichVus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDichVu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenDichVu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DonViTinh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DichVus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HoaDons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHoaDon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CanHoId = table.Column<int>(type: "int", nullable: false),
                    Thang = table.Column<int>(type: "int", nullable: false),
                    Nam = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HanThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThaiHoaDonId = table.Column<int>(type: "int", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BangGiaLuyTiens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BangGiaId = table.Column<int>(type: "int", nullable: false),
                    TuMuc = table.Column<double>(type: "float", nullable: false),
                    DenMuc = table.Column<double>(type: "float", nullable: true),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BangGiaLuyTiens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BangGiaLuyTiens_BangGias_BangGiaId",
                        column: x => x.BangGiaId,
                        principalTable: "BangGias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietHoaDons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoaDonId = table.Column<int>(type: "int", nullable: false),
                    LoaiChiTietId = table.Column<int>(type: "int", nullable: false),
                    DichVuId = table.Column<int>(type: "int", nullable: false),
                    TenDichVu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChiSoDau = table.Column<double>(type: "float", nullable: true),
                    ChiSoCuoi = table.Column<double>(type: "float", nullable: true),
                    SoLuong = table.Column<double>(type: "float", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietHoaDons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietHoaDons_HoaDons_HoaDonId",
                        column: x => x.HoaDonId,
                        principalTable: "HoaDons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LaiChamTras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoaDonId = table.Column<int>(type: "int", nullable: false),
                    NgayTinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoTienGoc = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoNgayCham = table.Column<int>(type: "int", nullable: false),
                    LaiSuat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoTienLai = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaiChamTras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaiChamTras_HoaDons_HoaDonId",
                        column: x => x.HoaDonId,
                        principalTable: "HoaDons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThanhToans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoaDonId = table.Column<int>(type: "int", nullable: false),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PhuongThucThanhToanId = table.Column<int>(type: "int", nullable: false),
                    MaGiaoDich = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhToans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThanhToans_HoaDons_HoaDonId",
                        column: x => x.HoaDonId,
                        principalTable: "HoaDons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiSoTieuThus_DichVuId",
                table: "ChiSoTieuThus",
                column: "DichVuId");

            migrationBuilder.CreateIndex(
                name: "IX_BangGiaLuyTiens_BangGiaId",
                table: "BangGiaLuyTiens",
                column: "BangGiaId");

            migrationBuilder.CreateIndex(
                name: "IX_CauHinhLais_MaCauHinh",
                table: "CauHinhLais",
                column: "MaCauHinh",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoaDons_HoaDonId",
                table: "ChiTietHoaDons",
                column: "HoaDonId");

            migrationBuilder.CreateIndex(
                name: "IX_DichVus_MaDichVu",
                table: "DichVus",
                column: "MaDichVu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_MaHoaDon",
                table: "HoaDons",
                column: "MaHoaDon",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LaiChamTras_HoaDonId",
                table: "LaiChamTras",
                column: "HoaDonId");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToans_HoaDonId",
                table: "ThanhToans",
                column: "HoaDonId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiSoTieuThus_CanHos_CanHoId",
                table: "ChiSoTieuThus",
                column: "CanHoId",
                principalTable: "CanHos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiSoTieuThus_DichVus_DichVuId",
                table: "ChiSoTieuThus",
                column: "DichVuId",
                principalTable: "DichVus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiSoTieuThus_CanHos_CanHoId",
                table: "ChiSoTieuThus");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiSoTieuThus_DichVus_DichVuId",
                table: "ChiSoTieuThus");

            migrationBuilder.DropTable(
                name: "BangGiaLuyTiens");

            migrationBuilder.DropTable(
                name: "CauHinhLais");

            migrationBuilder.DropTable(
                name: "ChiTietHoaDons");

            migrationBuilder.DropTable(
                name: "DichVus");

            migrationBuilder.DropTable(
                name: "LaiChamTras");

            migrationBuilder.DropTable(
                name: "ThanhToans");

            migrationBuilder.DropTable(
                name: "BangGias");

            migrationBuilder.DropTable(
                name: "HoaDons");

            migrationBuilder.DropIndex(
                name: "IX_ChiSoTieuThus_DichVuId",
                table: "ChiSoTieuThus");

            migrationBuilder.DropColumn(
                name: "TrangThaiCuTruId",
                table: "QuanHeCuTrus");

            migrationBuilder.DropColumn(
                name: "ChiSoCu",
                table: "ChiSoTieuThus");

            migrationBuilder.RenameColumn(
                name: "DichVuId",
                table: "ChiSoTieuThus",
                newName: "LoaiDichVuId");

            migrationBuilder.RenameColumn(
                name: "ChiSoMoi",
                table: "ChiSoTieuThus",
                newName: "ChiSo");

            migrationBuilder.AddColumn<bool>(
                name: "IsKetThuc",
                table: "QuanHeCuTrus",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiSoTieuThus_CanHos_CanHoId",
                table: "ChiSoTieuThus",
                column: "CanHoId",
                principalTable: "CanHos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
