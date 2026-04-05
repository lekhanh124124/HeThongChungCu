using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class YeuCauCuTruConfiguration : IEntityTypeConfiguration<YeuCauCuTru>
{
    public void Configure(EntityTypeBuilder<YeuCauCuTru> builder)
    {
        // TPH: Inherits table from YeuCau

        builder.HasMany(x => x.YeuCauTaiLieuCuTrus)
            .WithOne(x => x.YeuCauCuTru)
            .HasForeignKey(x => x.YeuCauCuTruId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Navigation(x => x.YeuCauTaiLieuCuTrus)
            .HasField("_yeuCauTaiLieuCuTrus")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(e => e.YeuCauHo)
            .HasMaxLength(50);

        builder.Property(e => e.YeuCauTen)
            .HasMaxLength(50);

        builder.OwnsOne(e => e.YeuCauSoDienThoai, sd =>
        {
            sd.Property(p => p.Value)
                .HasColumnName("YeuCauSoDienThoai")
                .HasMaxLength(15);
        });

        builder.Property(e => e.YeuCauCCCD)
            .HasMaxLength(20);

        builder.OwnsOne(e => e.YeuCauDiaChi, da =>
        {
            da.Property(p => p.FullAddress)
                .HasColumnName("YeuCauDiaChi")
                .HasMaxLength(500);
        });
    }
}
