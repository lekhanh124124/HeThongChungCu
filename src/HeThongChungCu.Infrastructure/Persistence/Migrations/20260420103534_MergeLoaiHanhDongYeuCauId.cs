using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongChungCu.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MergeLoaiHanhDongYeuCauId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YeuCauPhuongTien_LoaiHanhDongYeuCauId",
                table: "YeuCau");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "YeuCauPhuongTien_LoaiHanhDongYeuCauId",
                table: "YeuCau",
                type: "int",
                nullable: true);
        }
    }
}
