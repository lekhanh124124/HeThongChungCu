using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class LichBaoTriConfiguration : IEntityTypeConfiguration<LichBaoTri>
{
    public void Configure(EntityTypeBuilder<LichBaoTri> builder)
    {
        builder.ToTable("LichBaoTri");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TanSuatBaoTriId)
            .HasConversion(
                v => v.Value,
                v => TanSuatBaoTri.FromValue(v, null)!)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.HasOne<ThietBi>()
            .WithMany()
            .HasForeignKey(x => x.ThietBiId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<HangMucBaoTri>()
            .WithMany()
            .HasForeignKey(x => x.HangMucBaoTriId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.NgayBaoTriTiepTheo, x.IsActive });
    }
}
