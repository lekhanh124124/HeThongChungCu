using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class PhuongTienConfiguration : IEntityTypeConfiguration<PhuongTien>
{
    public void Configure(EntityTypeBuilder<PhuongTien> builder)
    {
        builder.ToTable("PhuongTiens");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.TenPhuongTien)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.BienSo)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.MauXe)
            .HasMaxLength(50);

        builder.Property(p => p.LoaiPhuongTienId)
            .HasConversion(
                v => v.Value,
                v => LoaiPhuongTien.FromValue(v, null)!
            )
            .IsRequired();

        builder.Property(p => p.TrangThaiPhuongTienId)
            .HasConversion(
                v => v.Value,
                v => TrangThaiPhuongTien.FromValue(v, null)!
            )
            .IsRequired();

        builder.HasOne<CanHo>()
            .WithMany()
            .HasForeignKey(p => p.CanHoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
