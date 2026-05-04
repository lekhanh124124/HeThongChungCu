using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class HoaDonConfiguration : IEntityTypeConfiguration<HoaDon>
{
    public void Configure(EntityTypeBuilder<HoaDon> builder)
    {
        builder.ToTable("HoaDon");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MaHoaDon)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.GhiChu)
            .HasMaxLength(500);

        builder.Property(x => x.TongTien)
            .HasPrecision(18, 2);

        builder.Property(x => x.NgayLap)
            .IsRequired();

        builder.Property(x => x.NgayHanThanhToan)
            .IsRequired();

        builder.Property(x => x.NgayTinhLaiCuoi)
            .IsRequired(false);

        builder.OwnsOne(x => x.KyThanhToan, kyBuilder =>
        {
            kyBuilder.Property(k => k.Thang).HasColumnName("Thang").IsRequired();
            kyBuilder.Property(k => k.Nam).HasColumnName("Nam").IsRequired();
        });

        builder.Property(x => x.TrangThaiHoaDonId)
            .HasConversion(
                v => v.Value,
                v => TrangThaiHoaDon.FromValue(v, null)!)
            .IsRequired();

        builder.HasOne<CanHo>()
            .WithMany()
            .HasForeignKey(x => x.CanHoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<DotThanhToan>()
            .WithMany()
            .HasForeignKey(x => x.DotThanhToanId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.ChiTietHoaDons)
            .WithOne()
            .HasForeignKey(x => x.HoaDonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.MaHoaDon).IsUnique();
        builder.HasIndex(x => x.DotThanhToanId);
        builder.HasIndex(x => x.CanHoId);
    }
}
