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

        builder.OwnsOne(x => x.ThoiGian, thoiGian =>
        {
            thoiGian.Property(v => v.NgayBatDau)
                .HasColumnName("NgayApDung")
                .IsRequired();

            thoiGian.Property(v => v.NgayKetThuc)
                .HasColumnName("NgayKetThuc");
        });

        builder.OwnsOne(x => x.DonGia, giaTien =>
        {
            giaTien.Property(p => p.SoTien)
                .HasColumnName("DonGia")
                .HasPrecision(18, 2);
            giaTien.Ignore(p => p.LoaiTien);
        });

        builder.Property(x => x.LoaiDinhGiaId)
            .HasConversion(
                v => v.Value,
                v => LoaiDinhGia.FromValue(v, null)!)
            .IsRequired();

        builder.HasMany(x => x.BangGiaLuyTiens)
            .WithOne()
            .HasForeignKey(x => x.BangGiaId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Navigation(x => x.BangGiaLuyTiens)
            .HasField("_bangGiaLuyTiens")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
