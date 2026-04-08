using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class ChiTietGiaLoaiCanHoConfiguration : IEntityTypeConfiguration<ChiTietGiaLoaiCanHo>
{
    public void Configure(EntityTypeBuilder<ChiTietGiaLoaiCanHo> builder)
    {
        builder.ToTable("ChiTietGiaLoaiCanHo");

        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.DonGia, giaTien =>
        {
            giaTien.Property(p => p.SoTien)
                .HasColumnName("DonGia")
                .HasPrecision(18, 2);
            giaTien.Ignore(p => p.LoaiTien);
        });

        builder.Property(x => x.LoaiCanHoId)
            .HasConversion(
                v => v == null ? (int?)null : v.Value,
                v => v == null ? null : LoaiCanHo.FromValue(v.Value, null)
            );

        builder.HasOne(x => x.BangGia)
            .WithMany(x => x.ChiTietGias)
            .HasForeignKey(x => x.BangGiaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
