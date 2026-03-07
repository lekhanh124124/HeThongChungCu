using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddChungCuEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GioiTinhId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ToaNhas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaToaNha = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenToaNha = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoTang = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToaNhas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CanHos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ToaNhaId = table.Column<int>(type: "int", nullable: false),
                    MaCanHo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DienTich = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tang = table.Column<int>(type: "int", nullable: false),
                    SoPhongNgu = table.Column<int>(type: "int", nullable: false),
                    SoPhongTam = table.Column<int>(type: "int", nullable: false),
                    TinhTrangCanHoId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanHos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanHos_ToaNhas_ToaNhaId",
                        column: x => x.ToaNhaId,
                        principalTable: "ToaNhas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuanHeCuTrus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CanHoId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoaiQuanHeCuTruId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_QuanHeCuTrus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuanHeCuTrus_CanHos_CanHoId",
                        column: x => x.CanHoId,
                        principalTable: "CanHos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuanHeCuTrus_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CanHos_MaCanHo",
                table: "CanHos",
                column: "MaCanHo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CanHos_ToaNhaId",
                table: "CanHos",
                column: "ToaNhaId");

            migrationBuilder.CreateIndex(
                name: "IX_QuanHeCuTrus_CanHoId",
                table: "QuanHeCuTrus",
                column: "CanHoId");

            migrationBuilder.CreateIndex(
                name: "IX_QuanHeCuTrus_UserId",
                table: "QuanHeCuTrus",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ToaNhas_MaToaNha",
                table: "ToaNhas",
                column: "MaToaNha",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuanHeCuTrus");

            migrationBuilder.DropTable(
                name: "CanHos");

            migrationBuilder.DropTable(
                name: "ToaNhas");

            migrationBuilder.DropColumn(
                name: "GioiTinhId",
                table: "Users");
        }
    }
}
