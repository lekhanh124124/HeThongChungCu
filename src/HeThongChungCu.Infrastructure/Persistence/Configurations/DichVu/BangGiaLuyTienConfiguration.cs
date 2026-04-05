using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class BangGiaLuyTienConfiguration : IEntityTypeConfiguration<BangGiaLuyTien>
{
    public void Configure(EntityTypeBuilder<BangGiaLuyTien> builder)
    {
        builder.ToTable("BangGiaLuyTien");

        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.DonGia, giaTien =>
        {
            giaTien.Property(p => p.SoTien)
                .HasColumnName("DonGia")
                .HasPrecision(18, 2);
            giaTien.Ignore(p => p.LoaiTien);
        });

        builder.Property(x => x.TuMuc)
            .HasPrecision(18, 4);

        builder.Property(x => x.DenMuc)
            .HasPrecision(18, 4);
    }
}
