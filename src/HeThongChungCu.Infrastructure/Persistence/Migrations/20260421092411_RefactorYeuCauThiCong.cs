using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorYeuCauThiCong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDaHoanCoc",
                table: "YeuCau",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LyDoKhauTru",
                table: "YeuCau",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TienKhauTru",
                table: "YeuCau",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDaHoanCoc",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "LyDoKhauTru",
                table: "YeuCau");

            migrationBuilder.DropColumn(
                name: "TienKhauTru",
                table: "YeuCau");
        }
    }
}
