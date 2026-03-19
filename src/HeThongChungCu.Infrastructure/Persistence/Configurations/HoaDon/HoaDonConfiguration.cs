using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.HoaDon;

public class HoaDonConfiguration : IEntityTypeConfiguration<Domain.Entities.HoaDon>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.HoaDon> builder)
    {
        builder.ToTable("HoaDons");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.MaHoaDon)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.TrangThaiHoaDonId)
            .HasConversion(
                v => v.Value,
                v => TrangThaiHoaDon.FromValue(v, null)!
            )
            .IsRequired();

        builder.HasMany(x => x.ChiTietHoaDons)
            .WithOne()
            .HasForeignKey(x => x.HoaDonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.ThanhToans)
            .WithOne()
            .HasForeignKey(x => x.HoaDonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.LaiChamTras)
            .WithOne()
            .HasForeignKey(x => x.HoaDonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.MaHoaDon).IsUnique();
    }
}
