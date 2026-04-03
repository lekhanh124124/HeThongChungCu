using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class BangGiaConfiguration : IEntityTypeConfiguration<BangGia>
{
    public void Configure(EntityTypeBuilder<BangGia> builder)
    {
        builder.ToTable("BangGia");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenBangGia)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.DonGia)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.LoaiDinhGiaId)
            .HasConversion(
                v => v.Value,
                v => LoaiDinhGia.FromValue(v, null)!)
            .IsRequired();

        builder.HasMany(x => x.BangGiaLuyTiens)
            .WithOne()
            .HasForeignKey(x => x.BangGiaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.BangGiaLuyTiens)
            .HasField("_bangGiaLuyTiens")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
