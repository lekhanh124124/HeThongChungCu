using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DoiTac",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDoiTac = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenCongTy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NguoiDaiDien = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SoGiayPhepKD = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaSoThue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgayKyHopDong = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayHetHan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThaiHopDongId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoiTac", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ho = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioiTinhId = table.Column<int>(type: "int", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CCCD = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TepTaiLieu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TepTaiLieu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThongBao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TieuDe = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiThongBao = table.Column<int>(type: "int", nullable: false),
                    ReferenceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Metadata = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ToaNha",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaToaNha = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenToaNha = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThaiToaNhaId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToaNha", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NhanVien",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungId = table.Column<int>(type: "int", nullable: false),
                    LoaiNhanVienId = table.Column<int>(type: "int", nullable: false),
                    TrangThaiNhanVienId = table.Column<int>(type: "int", nullable: false),
                    MaNhanVien = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayVaoLam = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayNghiLam = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_NhanVien", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NhanVien_NguoiDung_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "NguoiDung",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaiLieuNguoiDung",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDungId = table.Column<int>(type: "int", nullable: true),
                    LoaiGiayToId = table.Column<int>(type: "int", nullable: false),
                    SoGiayTo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayPhatHanh = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiLieuNguoiDung", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaiLieuNguoiDung_NguoiDung_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "NguoiDung",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DichVu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoiTacId = table.Column<int>(type: "int", nullable: true),
                    MaDichVu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenDichVu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LoaiDichVuId = table.Column<int>(type: "int", nullable: false),
                    DonViTinh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsBatBuoc = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IconId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DichVu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DichVu_DoiTac_DoiTacId",
                        column: x => x.DoiTacId,
                        principalTable: "DoiTac",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DichVu_TepTaiLieu_IconId",
                        column: x => x.IconId,
                        principalTable: "TepTaiLieu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HoaDonDoiTac",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoiTacId = table.Column<int>(type: "int", nullable: false),
                    Thang = table.Column<int>(type: "int", nullable: false),
                    Nam = table.Column<int>(type: "int", nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayGhiNhan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileHoaDonId = table.Column<int>(type: "int", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiThanhToanId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDonDoiTac", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoaDonDoiTac_DoiTac_DoiTacId",
                        column: x => x.DoiTacId,
                        principalTable: "DoiTac",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HoaDonDoiTac_TepTaiLieu_FileHoaDonId",
                        column: x => x.FileHoaDonId,
                        principalTable: "TepTaiLieu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    MatKhauHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnhDaiDienId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    NguoiDungId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaiKhoan_NguoiDung_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "NguoiDung",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TaiKhoan_TepTaiLieu_AnhDaiDienId",
                        column: x => x.AnhDaiDienId,
                        principalTable: "TepTaiLieu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PhanBoThongBao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThongBaoId = table.Column<int>(type: "int", nullable: false),
                    NguoiDungId = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ReadAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanBoThongBao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhanBoThongBao_NguoiDung_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "NguoiDung",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhanBoThongBao_ThongBao_ThongBaoId",
                        column: x => x.ThongBaoId,
                        principalTable: "ThongBao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tang",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTang = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenTang = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LoaiTangId = table.Column<int>(type: "int", nullable: false),
                    ToaNhaId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tang", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tang_ToaNha_ToaNhaId",
                        column: x => x.ToaNhaId,
                        principalTable: "ToaNha",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TepTaiLieuNguoiDung",
                columns: table => new
                {
                    FilesId = table.Column<int>(type: "int", nullable: false),
                    TaiLieuNguoiDungId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TepTaiLieuNguoiDung", x => new { x.FilesId, x.TaiLieuNguoiDungId });
                    table.ForeignKey(
                        name: "FK_TepTaiLieuNguoiDung_TaiLieuNguoiDung_TaiLieuNguoiDungId",
                        column: x => x.TaiLieuNguoiDungId,
                        principalTable: "TaiLieuNguoiDung",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TepTaiLieuNguoiDung_TepTaiLieu_FilesId",
                        column: x => x.FilesId,
                        principalTable: "TepTaiLieu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BangGia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DichVuId = table.Column<int>(type: "int", nullable: false),
                    TenBangGia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
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
                    table.PrimaryKey("PK_BangGia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BangGia_DichVu_DichVuId",
                        column: x => x.DichVuId,
                        principalTable: "DichVu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhanQuyen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaiKhoanId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanQuyen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhanQuyen_TaiKhoan_TaiKhoanId",
                        column: x => x.TaiKhoanId,
                        principalTable: "TaiKhoan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Token",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TokenHash = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ExpiresDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    TokenType = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    RevokedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ReasonRevoked = table.Column<int>(type: "int", nullable: true),
                    TaiKhoanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Token", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Token_TaiKhoan_TaiKhoanId",
                        column: x => x.TaiKhoanId,
                        principalTable: "TaiKhoan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CanHo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCanHo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenCanHo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DienTich = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoPhongNgu = table.Column<int>(type: "int", nullable: false),
                    SoPhongTam = table.Column<int>(type: "int", nullable: false),
                    LoaiCanHoId = table.Column<int>(type: "int", nullable: false),
                    TinhTrangCanHoId = table.Column<int>(type: "int", nullable: false),
                    TangId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanHo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanHo_Tang_TangId",
                        column: x => x.TangId,
                        principalTable: "Tang",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BangGiaLuyTien",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BangGiaId = table.Column<int>(type: "int", nullable: false),
                    TuMuc = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    DenMuc = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BangGiaLuyTien", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BangGiaLuyTien_BangGia_BangGiaId",
                        column: x => x.BangGiaId,
                        principalTable: "BangGia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiSoTieuThu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CanHoId = table.Column<int>(type: "int", nullable: false),
                    DichVuId = table.Column<int>(type: "int", nullable: false),
                    ChiSoCu = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ChiSoMoi = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Thang = table.Column<int>(type: "int", nullable: false),
                    Nam = table.Column<int>(type: "int", nullable: false),
                    NgayChot = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsLock = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiSoTieuThu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiSoTieuThu_CanHo_CanHoId",
                        column: x => x.CanHoId,
                        principalTable: "CanHo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiSoTieuThu_DichVu_DichVuId",
                        column: x => x.DichVuId,
                        principalTable: "DichVu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DangKyDichVu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CanHoId = table.Column<int>(type: "int", nullable: false),
                    DichVuId = table.Column<int>(type: "int", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    TrangThaiDangKyId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyDichVu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DangKyDichVu_CanHo_CanHoId",
                        column: x => x.CanHoId,
                        principalTable: "CanHo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DangKyDichVu_DichVu_DichVuId",
                        column: x => x.DichVuId,
                        principalTable: "DichVu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhuongTien",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CanHoId = table.Column<int>(type: "int", nullable: false),
                    TenPhuongTien = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LoaiPhuongTienId = table.Column<int>(type: "int", nullable: false),
                    BienSo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MauXe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrangThaiPhuongTienId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuongTien", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhuongTien_CanHo_CanHoId",
                        column: x => x.CanHoId,
                        principalTable: "CanHo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuanHeCuTru",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CanHoId = table.Column<int>(type: "int", nullable: false),
                    NguoiDungId = table.Column<int>(type: "int", nullable: false),
                    LoaiQuanHeCuTruId = table.Column<int>(type: "int", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThaiCuTruId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuanHeCuTru", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuanHeCuTru_CanHo_CanHoId",
                        column: x => x.CanHoId,
                        principalTable: "CanHo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuanHeCuTru_NguoiDung_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "NguoiDung",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YeuCau",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CanHoId = table.Column<int>(type: "int", nullable: false),
                    LoaiYeuCauId = table.Column<int>(type: "int", nullable: false),
                    TrangThaiId = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LyDo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NguoiXuLyId = table.Column<int>(type: "int", nullable: true),
                    NhanVienThucHienId = table.Column<int>(type: "int", nullable: true),
                    NgayXuLy = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LoaiYeuCauCuDan = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    YeuCauQuanHeCuTruId = table.Column<int>(type: "int", nullable: true),
                    YeuCauTen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    YeuCauHo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    YeuCauNgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    YeuCauGioiTinhId = table.Column<int>(type: "int", nullable: true),
                    YeuCauSoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    YeuCauCCCD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    YeuCauDiaChi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    YeuCauLoaiQuanHeId = table.Column<int>(type: "int", nullable: true),
                    YeuCauPhuongTienId = table.Column<int>(type: "int", nullable: true),
                    YeuCauTenPhuongTien = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    YeuCauLoaiPhuongTienId = table.Column<int>(type: "int", nullable: true),
                    YeuCauBienSo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    YeuCauMauXe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCau", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YeuCau_CanHo_CanHoId",
                        column: x => x.CanHoId,
                        principalTable: "CanHo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "ThePhuongTien",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhuongTienId = table.Column<int>(type: "int", nullable: false),
                    MaThe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThaiId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThePhuongTien", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThePhuongTien_PhuongTien_PhuongTienId",
                        column: x => x.PhuongTienId,
                        principalTable: "PhuongTien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "FK_TepYeuCauHinhAnhPhuongTien_YeuCau_YeuCauPhuongTienId",
                        column: x => x.YeuCauPhuongTienId,
                        principalTable: "YeuCau",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YeuCauTaiLieuCuTru",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YeuCauCuTruId = table.Column<int>(type: "int", nullable: false),
                    LoaiGiayToId = table.Column<int>(type: "int", nullable: false),
                    SoGiayTo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayPhatHanh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TaiLieuCuTruId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCauTaiLieuCuTru", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YeuCauTaiLieuCuTru_YeuCau_YeuCauCuTruId",
                        column: x => x.YeuCauCuTruId,
                        principalTable: "YeuCau",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TepYeuCauTaiLieuCuTru",
                columns: table => new
                {
                    FilesId = table.Column<int>(type: "int", nullable: false),
                    YeuCauTaiLieuCuTruId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TepYeuCauTaiLieuCuTru", x => new { x.FilesId, x.YeuCauTaiLieuCuTruId });
                    table.ForeignKey(
                        name: "FK_TepYeuCauTaiLieuCuTru_TepTaiLieu_FilesId",
                        column: x => x.FilesId,
                        principalTable: "TepTaiLieu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TepYeuCauTaiLieuCuTru_YeuCauTaiLieuCuTru_YeuCauTaiLieuCuTruId",
                        column: x => x.YeuCauTaiLieuCuTruId,
                        principalTable: "YeuCauTaiLieuCuTru",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BangGia_DichVuId",
                table: "BangGia",
                column: "DichVuId");

            migrationBuilder.CreateIndex(
                name: "IX_BangGiaLuyTien_BangGiaId",
                table: "BangGiaLuyTien",
                column: "BangGiaId");

            migrationBuilder.CreateIndex(
                name: "IX_CanHo_MaCanHo",
                table: "CanHo",
                column: "MaCanHo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CanHo_TangId",
                table: "CanHo",
                column: "TangId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiSoTieuThu_CanHoId_DichVuId_Thang_Nam",
                table: "ChiSoTieuThu",
                columns: new[] { "CanHoId", "DichVuId", "Thang", "Nam" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChiSoTieuThu_DichVuId",
                table: "ChiSoTieuThu",
                column: "DichVuId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyDichVu_CanHoId",
                table: "DangKyDichVu",
                column: "CanHoId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyDichVu_DichVuId",
                table: "DangKyDichVu",
                column: "DichVuId");

            migrationBuilder.CreateIndex(
                name: "IX_DichVu_DoiTacId",
                table: "DichVu",
                column: "DoiTacId");

            migrationBuilder.CreateIndex(
                name: "IX_DichVu_IconId",
                table: "DichVu",
                column: "IconId");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonDoiTac_DoiTacId",
                table: "HoaDonDoiTac",
                column: "DoiTacId");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonDoiTac_FileHoaDonId",
                table: "HoaDonDoiTac",
                column: "FileHoaDonId");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_CCCD",
                table: "NguoiDung",
                column: "CCCD",
                unique: true,
                filter: "[CCCD] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_SoDienThoai",
                table: "NguoiDung",
                column: "SoDienThoai",
                unique: true,
                filter: "[SoDienThoai] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_MaNhanVien",
                table: "NhanVien",
                column: "MaNhanVien",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_NguoiDungId",
                table: "NhanVien",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_PhanBoThongBao_NguoiDungId",
                table: "PhanBoThongBao",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_PhanBoThongBao_NguoiDungId_IsRead",
                table: "PhanBoThongBao",
                columns: new[] { "NguoiDungId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_PhanBoThongBao_ThongBaoId",
                table: "PhanBoThongBao",
                column: "ThongBaoId");

            migrationBuilder.CreateIndex(
                name: "IX_PhanQuyen_TaiKhoanId",
                table: "PhanQuyen",
                column: "TaiKhoanId");

            migrationBuilder.CreateIndex(
                name: "IX_PhuongTien_BienSo",
                table: "PhuongTien",
                column: "BienSo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhuongTien_CanHoId",
                table: "PhuongTien",
                column: "CanHoId");

            migrationBuilder.CreateIndex(
                name: "IX_QuanHeCuTru_CanHoId_NguoiDungId",
                table: "QuanHeCuTru",
                columns: new[] { "CanHoId", "NguoiDungId" },
                unique: true,
                filter: "[TrangThaiCuTruId] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_QuanHeCuTru_NguoiDungId",
                table: "QuanHeCuTru",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_AnhDaiDienId",
                table: "TaiKhoan",
                column: "AnhDaiDienId");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_Email",
                table: "TaiKhoan",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_NguoiDungId",
                table: "TaiKhoan",
                column: "NguoiDungId",
                unique: true,
                filter: "[NguoiDungId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_TenDangNhap",
                table: "TaiKhoan",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaiLieuNguoiDung_NguoiDungId",
                table: "TaiLieuNguoiDung",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_Tang_ToaNhaId_MaTang",
                table: "Tang",
                columns: new[] { "ToaNhaId", "MaTang" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TepHinhAnhPhuongTien_PhuongTienId",
                table: "TepHinhAnhPhuongTien",
                column: "PhuongTienId");

            migrationBuilder.CreateIndex(
                name: "IX_TepTaiLieuNguoiDung_TaiLieuNguoiDungId",
                table: "TepTaiLieuNguoiDung",
                column: "TaiLieuNguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_TepYeuCauHinhAnhPhuongTien_YeuCauPhuongTienId",
                table: "TepYeuCauHinhAnhPhuongTien",
                column: "YeuCauPhuongTienId");

            migrationBuilder.CreateIndex(
                name: "IX_TepYeuCauTaiLieuCuTru_YeuCauTaiLieuCuTruId",
                table: "TepYeuCauTaiLieuCuTru",
                column: "YeuCauTaiLieuCuTruId");

            migrationBuilder.CreateIndex(
                name: "IX_ThePhuongTien_MaThe",
                table: "ThePhuongTien",
                column: "MaThe",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThePhuongTien_PhuongTienId",
                table: "ThePhuongTien",
                column: "PhuongTienId");

            migrationBuilder.CreateIndex(
                name: "IX_ToaNha_MaToaNha",
                table: "ToaNha",
                column: "MaToaNha",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Token_TaiKhoanId",
                table: "Token",
                column: "TaiKhoanId");

            migrationBuilder.CreateIndex(
                name: "IX_Token_TokenHash",
                table: "Token",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_YeuCau_CanHoId",
                table: "YeuCau",
                column: "CanHoId");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauTaiLieuCuTru_YeuCauCuTruId",
                table: "YeuCauTaiLieuCuTru",
                column: "YeuCauCuTruId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BangGiaLuyTien");

            migrationBuilder.DropTable(
                name: "ChiSoTieuThu");

            migrationBuilder.DropTable(
                name: "DangKyDichVu");

            migrationBuilder.DropTable(
                name: "HoaDonDoiTac");

            migrationBuilder.DropTable(
                name: "NhanVien");

            migrationBuilder.DropTable(
                name: "PhanBoThongBao");

            migrationBuilder.DropTable(
                name: "PhanQuyen");

            migrationBuilder.DropTable(
                name: "QuanHeCuTru");

            migrationBuilder.DropTable(
                name: "TepHinhAnhPhuongTien");

            migrationBuilder.DropTable(
                name: "TepTaiLieuNguoiDung");

            migrationBuilder.DropTable(
                name: "TepYeuCauHinhAnhPhuongTien");

            migrationBuilder.DropTable(
                name: "TepYeuCauTaiLieuCuTru");

            migrationBuilder.DropTable(
                name: "ThePhuongTien");

            migrationBuilder.DropTable(
                name: "Token");

            migrationBuilder.DropTable(
                name: "BangGia");

            migrationBuilder.DropTable(
                name: "ThongBao");

            migrationBuilder.DropTable(
                name: "TaiLieuNguoiDung");

            migrationBuilder.DropTable(
                name: "YeuCauTaiLieuCuTru");

            migrationBuilder.DropTable(
                name: "PhuongTien");

            migrationBuilder.DropTable(
                name: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "DichVu");

            migrationBuilder.DropTable(
                name: "YeuCau");

            migrationBuilder.DropTable(
                name: "NguoiDung");

            migrationBuilder.DropTable(
                name: "DoiTac");

            migrationBuilder.DropTable(
                name: "TepTaiLieu");

            migrationBuilder.DropTable(
                name: "CanHo");

            migrationBuilder.DropTable(
                name: "Tang");

            migrationBuilder.DropTable(
                name: "ToaNha");
        }
    }
}
