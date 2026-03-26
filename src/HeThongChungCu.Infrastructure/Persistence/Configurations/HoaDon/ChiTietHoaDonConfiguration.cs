using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.HoaDon;

public class ChiTietHoaDonConfiguration : IEntityTypeConfiguration<ChiTietHoaDon>
{
    public void Configure(EntityTypeBuilder<ChiTietHoaDon> builder)
    {
        builder.ToTable("ChiTietHoaDon");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.LoaiChiTietId)
            .HasConversion(
                v => v.Value,
                v => LoaiChiTietHoaDon.FromValue(v, null)!
            )
            .IsRequired();

        builder.Property(x => x.DonGia)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.ThanhTien)
            .HasColumnType("decimal(18,2)");
    }
}
