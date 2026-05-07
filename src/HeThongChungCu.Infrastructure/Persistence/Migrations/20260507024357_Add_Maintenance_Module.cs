using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_Maintenance_Module : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HangMucBaoTri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHangMuc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenHangMuc = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ThoiGianUocTinhPhut = table.Column<int>(type: "int", nullable: false),
                    ChiPhiUocTinh = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ChecklistTieuChuan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HangMucBaoTri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThietBi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThietBi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenThietBi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LoaiThietBi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ViTri = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NgayMua = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    NgayHetHanBaoHanh = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    GiaTriBanDau = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThietBi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LichBaoTri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThietBiId = table.Column<int>(type: "int", nullable: false),
                    HangMucBaoTriId = table.Column<int>(type: "int", nullable: false),
                    TanSuat = table.Column<int>(type: "int", nullable: false),
                    NgayBatDau = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    NgayKetThuc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NgayBaoTriGanNhat = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NgayBaoTriTiepTheo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichBaoTri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LichBaoTri_HangMucBaoTri_HangMucBaoTriId",
                        column: x => x.HangMucBaoTriId,
                        principalTable: "HangMucBaoTri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichBaoTri_ThietBi_ThietBiId",
                        column: x => x.ThietBiId,
                        principalTable: "ThietBi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhieuBaoTri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhieu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ThietBiId = table.Column<int>(type: "int", nullable: false),
                    HangMucBaoTriId = table.Column<int>(type: "int", nullable: false),
                    LichBaoTriId = table.Column<int>(type: "int", nullable: true),
                    LoaiBaoTri = table.Column<int>(type: "int", nullable: false),
                    YeuCauSuaChuaId = table.Column<int>(type: "int", nullable: true),
                    DoiTacId = table.Column<int>(type: "int", nullable: true),
                    HopDongDoiTacId = table.Column<int>(type: "int", nullable: true),
                    NgayLapPhieu = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    NgayDuKien = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    NgayThucTe = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ChiPhiThucTe = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    GhiChuXuLy = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LyDoHuy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NguoiKiemDuyetId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuBaoTri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhieuBaoTri_DoiTac_DoiTacId",
                        column: x => x.DoiTacId,
                        principalTable: "DoiTac",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuBaoTri_HangMucBaoTri_HangMucBaoTriId",
                        column: x => x.HangMucBaoTriId,
                        principalTable: "HangMucBaoTri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuBaoTri_HopDongDoiTac_HopDongDoiTacId",
                        column: x => x.HopDongDoiTacId,
                        principalTable: "HopDongDoiTac",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuBaoTri_LichBaoTri_LichBaoTriId",
                        column: x => x.LichBaoTriId,
                        principalTable: "LichBaoTri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PhieuBaoTri_ThietBi_ThietBiId",
                        column: x => x.ThietBiId,
                        principalTable: "ThietBi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuBaoTri_YeuCau_YeuCauSuaChuaId",
                        column: x => x.YeuCauSuaChuaId,
                        principalTable: "YeuCau",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "NhanSuBaoTri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhieuBaoTriId = table.Column<int>(type: "int", nullable: false),
                    NhanVienId = table.Column<int>(type: "int", nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanSuBaoTri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NhanSuBaoTri_NhanVien_NhanVienId",
                        column: x => x.NhanVienId,
                        principalTable: "NhanVien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NhanSuBaoTri_PhieuBaoTri_PhieuBaoTriId",
                        column: x => x.PhieuBaoTriId,
                        principalTable: "PhieuBaoTri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhieuBaoTriChecklist",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhieuBaoTriId = table.Column<int>(type: "int", nullable: false),
                    NoiDungChecklist = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    DatYeuCau = table.Column<bool>(type: "bit", nullable: true),
                    GhiChuThucTe = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    AnhMinhHoaUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuBaoTriChecklist", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhieuBaoTriChecklist_PhieuBaoTri_PhieuBaoTriId",
                        column: x => x.PhieuBaoTriId,
                        principalTable: "PhieuBaoTri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhieuBaoTriVatTu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhieuBaoTriId = table.Column<int>(type: "int", nullable: false),
                    TenVatTu = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuBaoTriVatTu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhieuBaoTriVatTu_PhieuBaoTri_PhieuBaoTriId",
                        column: x => x.PhieuBaoTriId,
                        principalTable: "PhieuBaoTri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HangMucBaoTri_MaHangMuc",
                table: "HangMucBaoTri",
                column: "MaHangMuc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LichBaoTri_HangMucBaoTriId",
                table: "LichBaoTri",
                column: "HangMucBaoTriId");

            migrationBuilder.CreateIndex(
                name: "IX_LichBaoTri_NgayBaoTriTiepTheo_IsActive",
                table: "LichBaoTri",
                columns: new[] { "NgayBaoTriTiepTheo", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_LichBaoTri_ThietBiId",
                table: "LichBaoTri",
                column: "ThietBiId");

            migrationBuilder.CreateIndex(
                name: "IX_NhanSuBaoTri_NhanVienId",
                table: "NhanSuBaoTri",
                column: "NhanVienId");

            migrationBuilder.CreateIndex(
                name: "IX_NhanSuBaoTri_PhieuBaoTriId",
                table: "NhanSuBaoTri",
                column: "PhieuBaoTriId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuBaoTri_DoiTacId",
                table: "PhieuBaoTri",
                column: "DoiTacId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuBaoTri_HangMucBaoTriId",
                table: "PhieuBaoTri",
                column: "HangMucBaoTriId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuBaoTri_HopDongDoiTacId",
                table: "PhieuBaoTri",
                column: "HopDongDoiTacId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuBaoTri_LichBaoTriId",
                table: "PhieuBaoTri",
                column: "LichBaoTriId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuBaoTri_MaPhieu",
                table: "PhieuBaoTri",
                column: "MaPhieu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhieuBaoTri_NgayDuKien",
                table: "PhieuBaoTri",
                column: "NgayDuKien");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuBaoTri_ThietBiId",
                table: "PhieuBaoTri",
                column: "ThietBiId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuBaoTri_TrangThai",
                table: "PhieuBaoTri",
                column: "TrangThai");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuBaoTri_YeuCauSuaChuaId",
                table: "PhieuBaoTri",
                column: "YeuCauSuaChuaId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuBaoTriChecklist_PhieuBaoTriId",
                table: "PhieuBaoTriChecklist",
                column: "PhieuBaoTriId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuBaoTriVatTu_PhieuBaoTriId",
                table: "PhieuBaoTriVatTu",
                column: "PhieuBaoTriId");

            migrationBuilder.CreateIndex(
                name: "IX_ThietBi_MaThietBi",
                table: "ThietBi",
                column: "MaThietBi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThietBi_TenThietBi",
                table: "ThietBi",
                column: "TenThietBi");

            migrationBuilder.CreateIndex(
                name: "IX_ThietBi_TrangThai",
                table: "ThietBi",
                column: "TrangThai");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NhanSuBaoTri");

            migrationBuilder.DropTable(
                name: "PhieuBaoTriChecklist");

            migrationBuilder.DropTable(
                name: "PhieuBaoTriVatTu");

            migrationBuilder.DropTable(
                name: "PhieuBaoTri");

            migrationBuilder.DropTable(
                name: "LichBaoTri");

            migrationBuilder.DropTable(
                name: "HangMucBaoTri");

            migrationBuilder.DropTable(
                name: "ThietBi");
        }
    }
}
