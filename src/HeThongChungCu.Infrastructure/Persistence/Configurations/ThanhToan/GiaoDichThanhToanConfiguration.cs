using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class GiaoDichThanhToanConfiguration : IEntityTypeConfiguration<GiaoDichThanhToan>
{
    public void Configure(EntityTypeBuilder<GiaoDichThanhToan> builder)
    {
        builder.ToTable("GiaoDichThanhToan");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SoTien)
            .HasPrecision(18, 2);

        builder.Property(x => x.MaGiaoDich)
            .HasMaxLength(100);

        builder.Property(x => x.NgayGiaoDich)
            .IsRequired();

        builder.Property(x => x.GhiChu)
            .HasMaxLength(500);

        builder.Property(x => x.PhuongThucThanhToanId)
            .HasConversion(
                v => v.Value,
                v => PhuongThucThanhToan.FromValue(v, null)!)
            .IsRequired();

        builder.HasOne<HoaDon>()
            .WithMany()
            .HasForeignKey(x => x.HoaDonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
