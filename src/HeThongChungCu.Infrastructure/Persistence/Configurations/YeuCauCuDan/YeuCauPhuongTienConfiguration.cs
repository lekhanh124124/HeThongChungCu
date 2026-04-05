using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class YeuCauPhuongTienConfiguration : IEntityTypeConfiguration<YeuCauPhuongTien>
{
    public void Configure(EntityTypeBuilder<YeuCauPhuongTien> builder)
    {
        // TPH: Inherits table from YeuCau

        builder.Property(e => e.YeuCauTenPhuongTien).HasMaxLength(100);
        builder.Property(e => e.YeuCauBienSo).HasMaxLength(20);
        builder.Property(e => e.YeuCauMauXe).HasMaxLength(50);

        builder.Property(x => x.YeuCauLoaiPhuongTienId)
            .HasConversion(
                v => v.Value,
                v => LoaiPhuongTien.FromValue(v, null)!);

        builder.HasMany(x => x.YeuCauHinhAnhPhuongTiens)
            .WithOne(x => x.YeuCauPhuongTien)
            .HasForeignKey(x => x.YeuCauPhuongTienId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
