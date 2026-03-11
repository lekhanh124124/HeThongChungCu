using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTangEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [QuanHeCuTrus]");
            migrationBuilder.Sql("DELETE FROM [ThePhuongTiens]");
            migrationBuilder.Sql("DELETE FROM [PhuongTiens]");
            migrationBuilder.Sql("DELETE FROM [ChiSoTieuThus]");
            migrationBuilder.Sql("DELETE FROM [CanHos]");

            migrationBuilder.RenameColumn(
                name: "Tang",
                table: "CanHos",
                newName: "TangId");

            migrationBuilder.CreateTable(
                name: "Tangs",
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
                    table.PrimaryKey("PK_Tangs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tangs_ToaNhas_ToaNhaId",
                        column: x => x.ToaNhaId,
                        principalTable: "ToaNhas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CanHos_TangId",
                table: "CanHos",
                column: "TangId");

            migrationBuilder.CreateIndex(
                name: "IX_Tangs_ToaNhaId",
                table: "Tangs",
                column: "ToaNhaId");

            migrationBuilder.AddForeignKey(
                name: "FK_CanHos_Tangs_TangId",
                table: "CanHos",
                column: "TangId",
                principalTable: "Tangs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CanHos_Tangs_TangId",
                table: "CanHos");

            migrationBuilder.DropTable(
                name: "Tangs");

            migrationBuilder.DropIndex(
                name: "IX_CanHos_TangId",
                table: "CanHos");

            migrationBuilder.RenameColumn(
                name: "TangId",
                table: "CanHos",
                newName: "Tang");
        }
    }
}
