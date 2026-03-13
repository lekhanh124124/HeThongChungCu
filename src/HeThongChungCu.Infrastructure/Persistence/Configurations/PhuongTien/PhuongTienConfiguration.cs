using HeThongChungCu.Domain.Entities.ChungCu;
using HeThongChungCu.Domain.Entities.PhuongTien;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.PhuongTien;

public class PhuongTienConfiguration : IEntityTypeConfiguration<Domain.Entities.PhuongTien.PhuongTien>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.PhuongTien.PhuongTien> builder)
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

        builder.HasOne<CanHo>()
            .WithMany(c => c.PhuongTiens)
            .HasForeignKey(p => p.CanHoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
