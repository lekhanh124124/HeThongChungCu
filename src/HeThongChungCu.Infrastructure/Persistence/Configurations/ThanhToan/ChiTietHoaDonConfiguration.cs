using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class ChiTietHoaDonConfiguration : IEntityTypeConfiguration<ChiTietHoaDon>
{
    public void Configure(EntityTypeBuilder<ChiTietHoaDon> builder)
    {
        builder.ToTable("ChiTietHoaDon");

        builder.HasKey(x => x.Id);

        // TPH Configuration
        builder.HasDiscriminator(x => x.LoaiChiTietHoaDonId)
            .HasValue<ChiTietHoaDonDichVu>(LoaiChiTietHoaDon.DichVu)
            .HasValue<ChiTietHoaDonTieuThu>(LoaiChiTietHoaDon.TieuThu)
            .HasValue<ChiTietHoaDonSuaChua>(LoaiChiTietHoaDon.SuaChua)
            .HasValue<ChiTietHoaDonThiCong>(LoaiChiTietHoaDon.ThiCong);

        builder.Property(x => x.LoaiChiTietHoaDonId)
            .HasConversion(
                v => v.Value,
                v => LoaiChiTietHoaDon.FromValue(v, null)!)
            .IsRequired();

        builder.Property(x => x.TenMucPhi)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.SoLuong)
            .HasPrecision(18, 3);

        builder.Property(x => x.DonGia)
            .HasPrecision(18, 2);

        builder.Property(x => x.ThanhTien)
            .HasPrecision(18, 2);

        builder.Property(x => x.GhiChu)
            .HasMaxLength(500);

        // Foreign Keys
        builder.HasOne<HoaDon>()
            .WithMany(x => x.ChiTietHoaDons)
            .HasForeignKey(x => x.HoaDonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
