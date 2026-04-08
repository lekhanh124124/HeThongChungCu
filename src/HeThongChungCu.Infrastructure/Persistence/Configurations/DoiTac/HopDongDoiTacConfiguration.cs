using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class HopDongDoiTacConfiguration : IEntityTypeConfiguration<HopDongDoiTac>
{
    public void Configure(EntityTypeBuilder<HopDongDoiTac> builder)
    {
        builder.ToTable("HopDongDoiTac");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SoHopDong)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.NgayKy)
            .IsRequired();

        builder.Property(x => x.NgayHetHan)
            .IsRequired();

        builder.OwnsOne(x => x.GiaTriHopDong, giaTien =>
        {
            giaTien.Property(p => p.SoTien)
                .HasColumnName("GiaTriHopDong")
                .HasPrecision(18, 2)
                .IsRequired();
            giaTien.Ignore(p => p.LoaiTien);
        });

        builder.Property(x => x.NoiDung)
            .HasMaxLength(1000);

        builder.HasOne<HeThongChungCu.Domain.Entities.DichVu>()
            .WithMany()
            .HasForeignKey(x => x.DichVuId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.TrangThaiHopDongId)
            .HasConversion(
                v => v.Value,
                v => TrangThaiHopDong.FromValue(v, null)!)
            .IsRequired();

        builder.HasOne(x => x.DoiTac)
            .WithMany(x => x.HopDongs)
            .HasForeignKey(x => x.DoiTacId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.TepHopDongs)
            .WithOne(x => x.HopDongDoiTac)
            .HasForeignKey(x => x.HopDongDoiTacId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
