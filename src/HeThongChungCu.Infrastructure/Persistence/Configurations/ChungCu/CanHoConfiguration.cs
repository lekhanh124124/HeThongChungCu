using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class CanHoConfiguration : IEntityTypeConfiguration<CanHo>
{
    public void Configure(EntityTypeBuilder<CanHo> builder)
    {
        builder.ToTable("CanHo");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.MaCanHo)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.TenCanHo)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(c => c.MaCanHo).IsUnique();


        builder.OwnsOne(c => c.ThongSo, thongSo =>
        {
            thongSo.Property(t => t.DienTich)
                .HasColumnName("DienTich")
                .HasPrecision(18, 2)
                .IsRequired();

            thongSo.Property(t => t.SoPhongNgu)
                .HasColumnName("SoPhongNgu")
                .IsRequired();

            thongSo.Property(t => t.SoPhongTam)
                .HasColumnName("SoPhongTam")
                .IsRequired();
        });

        builder.Property(c => c.TangId)
            .IsRequired();

        builder.HasOne<Tang>()
            .WithMany()
            .HasForeignKey(c => c.TangId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(c => c.LoaiCanHoId)
            .HasConversion(
                v => v.Value,
                v => LoaiCanHo.FromValue(v, null)!
            )
            .IsRequired();

        builder.Property(c => c.TinhTrangCanHoId)
            .HasConversion(
                v => v.Value,
                v => TrangThaiCanHo.FromValue(v, null)!
            )
            .IsRequired();


    }
}
