using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.DichVu
{
    public class BangGiaCoDinhConfiguration : IEntityTypeConfiguration<BangGiaCoDinh>
    {
        public void Configure(EntityTypeBuilder<BangGiaCoDinh> builder)
        {
            builder.OwnsOne(x => x.DonGia, giaTien =>
            {
                giaTien.Property(p => p.SoTien)
                    .HasColumnName("DonGia")
                    .HasPrecision(18, 2);
                giaTien.Ignore(p => p.LoaiTien);
            });
        }
    }
}
