using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.DichVu
{
    public class BangGiaKhungGioConfiguration : IEntityTypeConfiguration<BangGiaKhungGio>
    {
        public void Configure(EntityTypeBuilder<BangGiaKhungGio> builder)
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
