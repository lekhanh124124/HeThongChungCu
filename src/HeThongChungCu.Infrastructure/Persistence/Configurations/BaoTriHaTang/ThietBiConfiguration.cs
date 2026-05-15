using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class ThietBiConfiguration : IEntityTypeConfiguration<ThietBi>
{
    public void Configure(EntityTypeBuilder<ThietBi> builder)
    {
        builder.ToTable("ThietBi");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.MaThietBi)
            .HasMaxLength(50)
            .IsRequired();
        builder.HasIndex(x => x.MaThietBi).IsUnique();

        builder.Property(x => x.TenThietBi)
            .HasMaxLength(200)
            .IsRequired();
        builder.HasIndex(x => x.TenThietBi);

        builder.Property(x => x.LoaiThietBi)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ViTri)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.GiaTriBanDau)
            .HasPrecision(18, 2);

        builder.Property(x => x.GhiChu)
            .HasMaxLength(1000);

        builder.Property(x => x.TrangThaiThietBiId)
            .HasConversion(
                v => v.Value,
                v => TrangThaiThietBi.FromValue(v, null)!)
            .IsRequired();
        builder.HasIndex(x => x.TrangThaiThietBiId);

        builder.HasOne<ToaNha>()
            .WithMany()
            .HasForeignKey(x => x.ToaNhaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
