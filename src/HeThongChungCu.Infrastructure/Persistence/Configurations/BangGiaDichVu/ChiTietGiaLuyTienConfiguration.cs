using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class ChiTietGiaLuyTienConfiguration : IEntityTypeConfiguration<ChiTietGiaLuyTien>
{
    public void Configure(EntityTypeBuilder<ChiTietGiaLuyTien> builder)
    {
        builder.ToTable("ChiTietGiaLuyTien");

        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.DonGia, giaTien =>
        {
            giaTien.Property(p => p.SoTien)
                .HasColumnName("DonGia")
                .HasPrecision(18, 2);
            giaTien.Ignore(p => p.LoaiTien);
        });

        builder.Property(x => x.TuMuc)
            .HasPrecision(18, 2);

        builder.Property(x => x.DenMuc)
            .HasPrecision(18, 2);
    }
}
