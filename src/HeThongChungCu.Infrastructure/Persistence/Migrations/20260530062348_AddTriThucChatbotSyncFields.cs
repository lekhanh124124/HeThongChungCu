using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTriThucChatbotSyncFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TriThucChatbot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TieuDe = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DanhMuc = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ThuTuHienThi = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsSynced = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastSyncedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriThucChatbot", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TriThucChatbot_DanhMuc",
                table: "TriThucChatbot",
                column: "DanhMuc");

            migrationBuilder.CreateIndex(
                name: "IX_TriThucChatbot_DanhMuc_ThuTuHienThi",
                table: "TriThucChatbot",
                columns: new[] { "DanhMuc", "ThuTuHienThi" });

            migrationBuilder.CreateIndex(
                name: "IX_TriThucChatbot_IsActive",
                table: "TriThucChatbot",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TriThucChatbot_IsSynced",
                table: "TriThucChatbot",
                column: "IsSynced");

            migrationBuilder.CreateIndex(
                name: "IX_TriThucChatbot_IsSynced_IsDeleted",
                table: "TriThucChatbot",
                columns: new[] { "IsSynced", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TriThucChatbot");
        }
    }
}
