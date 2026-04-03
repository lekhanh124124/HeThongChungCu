using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class ChiSoTieuThuConfiguration : IEntityTypeConfiguration<ChiSoTieuThu>
{
    public void Configure(EntityTypeBuilder<ChiSoTieuThu> builder)
    {
        builder.ToTable("ChiSoTieuThu");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CanHoId)
            .IsRequired();

        builder.Property(x => x.DichVuId)
            .IsRequired();

        builder.Property(x => x.ChiSoCu)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(x => x.ChiSoMoi)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(x => x.Thang)
            .IsRequired();

        builder.Property(x => x.Nam)
            .IsRequired();

        builder.Property(x => x.NgayChot)
            .IsRequired();

        builder.Property(x => x.IsLock)
            .IsRequired()
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne<CanHo>()
            .WithMany()
            .HasForeignKey(x => x.CanHoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<DichVu>()
            .WithMany()
            .HasForeignKey(x => x.DichVuId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique index to prevent duplicate readings for the same month/service/apartment
        builder.HasIndex(x => new { x.CanHoId, x.DichVuId, x.Thang, x.Nam })
            .IsUnique();
    }
}
