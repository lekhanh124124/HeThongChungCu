using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.BangGia;

public class BangGiaConfiguration : IEntityTypeConfiguration<Domain.Entities.BangGia>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.BangGia> builder)
    {
        builder.ToTable("BangGias");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenBangGia)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.LoaiDinhGiaId)
            .HasConversion(
                v => v.Value,
                v => LoaiDinhGia.FromValue(v, null)!
            )
            .IsRequired();

        builder.Property(x => x.DonGia)
            .HasColumnType("decimal(18,2)");

        builder.HasMany(x => x.BangGiaLuyTiens)
            .WithOne()
            .HasForeignKey(x => x.BangGiaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
