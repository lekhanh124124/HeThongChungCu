using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BangGia",
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
                    table.PrimaryKey("PK_BangGia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CauHinhLai",
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
                    table.PrimaryKey("PK_CauHinhLai", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DichVu",
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
                    table.PrimaryKey("PK_DichVu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HoaDon",
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
                    table.PrimaryKey("PK_HoaDon", x => x.Id);
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
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                name: "BangGiaLuyTien",
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
                    table.PrimaryKey("PK_BangGiaLuyTien", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BangGiaLuyTien_BangGia_BangGiaId",
                        column: x => x.BangGiaId,
                        principalTable: "BangGia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietHoaDon",
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
                    table.PrimaryKey("PK_ChiTietHoaDon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietHoaDon_HoaDon_HoaDonId",
                        column: x => x.HoaDonId,
                        principalTable: "HoaDon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LaiChamTra",
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
                    table.PrimaryKey("PK_LaiChamTra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaiChamTra_HoaDon_HoaDonId",
                        column: x => x.HoaDonId,
                        principalTable: "HoaDon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThanhToan",
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
                    table.PrimaryKey("PK_ThanhToan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThanhToan_HoaDon_HoaDonId",
                        column: x => x.HoaDonId,
                        principalTable: "HoaDon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "ChiSoTieuThu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CanHoId = table.Column<int>(type: "int", nullable: false),
                    DichVuId = table.Column<int>(type: "int", nullable: false),
                    ChiSoCu = table.Column<double>(type: "float", nullable: false),
                    ChiSoMoi = table.Column<double>(type: "float", nullable: false),
                    Thang = table.Column<int>(type: "int", nullable: false),
                    Nam = table.Column<int>(type: "int", nullable: false),
                    NgayChot = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsLock = table.Column<bool>(type: "bit", nullable: false),
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
                    SoLuong = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
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
                name: "ThePhuongTien",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhuongTienId = table.Column<int>(type: "int", nullable: false),
                    MaThe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
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
                name: "YeuCauCuTru",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CanHoId = table.Column<int>(type: "int", nullable: false),
                    QuanHeCuTruId = table.Column<int>(type: "int", nullable: true),
                    LoaiYeuCauId = table.Column<int>(type: "int", nullable: false),
                    TrangThaiId = table.Column<int>(type: "int", nullable: false),
                    LyDo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NoiDung = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NguoiXuLyId = table.Column<int>(type: "int", nullable: true),
                    NgayXuLy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    YeuCauTen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    YeuCauHo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    YeuCauNgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    YeuCauGioiTinhId = table.Column<int>(type: "int", nullable: true),
                    YeuCauSoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    YeuCauLoaiQuanHeId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCauCuTru", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YeuCauCuTru_CanHo_CanHoId",
                        column: x => x.CanHoId,
                        principalTable: "CanHo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_YeuCauCuTru_QuanHeCuTru_QuanHeCuTruId",
                        column: x => x.QuanHeCuTruId,
                        principalTable: "QuanHeCuTru",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
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
                    NgayPhatHanh = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCauTaiLieuCuTru", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YeuCauTaiLieuCuTru_YeuCauCuTru_YeuCauCuTruId",
                        column: x => x.YeuCauCuTruId,
                        principalTable: "YeuCauCuTru",
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
                name: "IX_CauHinhLai_MaCauHinh",
                table: "CauHinhLai",
                column: "MaCauHinh",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChiSoTieuThu_CanHoId",
                table: "ChiSoTieuThu",
                column: "CanHoId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiSoTieuThu_DichVuId",
                table: "ChiSoTieuThu",
                column: "DichVuId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoaDon_HoaDonId",
                table: "ChiTietHoaDon",
                column: "HoaDonId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyDichVu_CanHoId",
                table: "DangKyDichVu",
                column: "CanHoId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyDichVu_DichVuId",
                table: "DangKyDichVu",
                column: "DichVuId");

            migrationBuilder.CreateIndex(
                name: "IX_DichVu_MaDichVu",
                table: "DichVu",
                column: "MaDichVu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaHoaDon",
                table: "HoaDon",
                column: "MaHoaDon",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LaiChamTra_HoaDonId",
                table: "LaiChamTra",
                column: "HoaDonId");

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
                name: "IX_TepTaiLieuNguoiDung_TaiLieuNguoiDungId",
                table: "TepTaiLieuNguoiDung",
                column: "TaiLieuNguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_TepYeuCauTaiLieuCuTru_YeuCauTaiLieuCuTruId",
                table: "TepYeuCauTaiLieuCuTru",
                column: "YeuCauTaiLieuCuTruId");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToan_HoaDonId",
                table: "ThanhToan",
                column: "HoaDonId");

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
                name: "IX_YeuCauCuTru_CanHoId",
                table: "YeuCauCuTru",
                column: "CanHoId");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauCuTru_QuanHeCuTruId",
                table: "YeuCauCuTru",
                column: "QuanHeCuTruId");

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
                name: "CauHinhLai");

            migrationBuilder.DropTable(
                name: "ChiSoTieuThu");

            migrationBuilder.DropTable(
                name: "ChiTietHoaDon");

            migrationBuilder.DropTable(
                name: "DangKyDichVu");

            migrationBuilder.DropTable(
                name: "LaiChamTra");

            migrationBuilder.DropTable(
                name: "PhanQuyen");

            migrationBuilder.DropTable(
                name: "TepTaiLieuNguoiDung");

            migrationBuilder.DropTable(
                name: "TepYeuCauTaiLieuCuTru");

            migrationBuilder.DropTable(
                name: "ThanhToan");

            migrationBuilder.DropTable(
                name: "ThePhuongTien");

            migrationBuilder.DropTable(
                name: "Token");

            migrationBuilder.DropTable(
                name: "BangGia");

            migrationBuilder.DropTable(
                name: "DichVu");

            migrationBuilder.DropTable(
                name: "TaiLieuNguoiDung");

            migrationBuilder.DropTable(
                name: "YeuCauTaiLieuCuTru");

            migrationBuilder.DropTable(
                name: "HoaDon");

            migrationBuilder.DropTable(
                name: "PhuongTien");

            migrationBuilder.DropTable(
                name: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "YeuCauCuTru");

            migrationBuilder.DropTable(
                name: "TepTaiLieu");

            migrationBuilder.DropTable(
                name: "QuanHeCuTru");

            migrationBuilder.DropTable(
                name: "CanHo");

            migrationBuilder.DropTable(
                name: "NguoiDung");

            migrationBuilder.DropTable(
                name: "Tang");

            migrationBuilder.DropTable(
                name: "ToaNha");
        }
    }
}
