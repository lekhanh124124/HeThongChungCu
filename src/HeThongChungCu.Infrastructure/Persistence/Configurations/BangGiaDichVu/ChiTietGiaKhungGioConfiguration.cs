using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class ChiTietGiaKhungGioConfiguration : IEntityTypeConfiguration<ChiTietGiaKhungGio>
{
    public void Configure(EntityTypeBuilder<ChiTietGiaKhungGio> builder)
    {
        builder.ToTable("ChiTietGiaKhungGio");

        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.DonGia, giaTien =>
        {
            giaTien.Property(p => p.SoTien)
                .HasColumnName("DonGia")
                .HasPrecision(18, 2);
            giaTien.Ignore(p => p.LoaiTien);
        });

        builder.HasOne(x => x.KhungGio)
            .WithMany()
            .HasForeignKey(x => x.KhungGioId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(x => x.BangGia)
            .WithMany(x => x.ChiTietGias)
            .HasForeignKey(x => x.BangGiaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
