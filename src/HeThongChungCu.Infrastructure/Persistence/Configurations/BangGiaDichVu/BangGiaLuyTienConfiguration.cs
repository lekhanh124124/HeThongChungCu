using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.DichVu
{
    public class BangGiaLuyTienConfiguration : IEntityTypeConfiguration<BangGiaLuyTien>
    {
        public void Configure(EntityTypeBuilder<BangGiaLuyTien> builder)
        {
            builder.HasMany(x => x.ChiTietGias)
                .WithOne(x => x.BangGia)
                .HasForeignKey(x => x.BangGiaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(x => x.ChiTietGias)
                .HasField("_chiTietGias")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
