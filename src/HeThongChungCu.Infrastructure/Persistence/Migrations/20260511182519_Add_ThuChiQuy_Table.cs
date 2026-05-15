using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_ThuChiQuy_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThuChiQuy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaGiaoDich = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LoaiGiaoDichId = table.Column<int>(type: "int", nullable: false),
                    KhoanMucId = table.Column<int>(type: "int", nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NgayGiaoDich = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PhuongThucThanhToanId = table.Column<int>(type: "int", nullable: false),
                    NguoiGiaoDich = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ChungTuGoc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThuChiQuy");
        }
    }
}
