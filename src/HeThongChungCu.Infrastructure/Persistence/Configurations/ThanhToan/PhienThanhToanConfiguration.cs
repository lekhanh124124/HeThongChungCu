using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class PhienThanhToanConfiguration : IEntityTypeConfiguration<PhienThanhToan>
{
    public void Configure(EntityTypeBuilder<PhienThanhToan> builder)
    {
        builder.ToTable("PhienThanhToan");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MaThanhToan)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.MaThanhToan)
            .IsUnique();

        builder.Property(x => x.SoTien)
            .HasPrecision(18, 2);

        builder.Property(x => x.ChiTietHoaDonIds)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.TrangThaiThanhToanId)
            .HasConversion(
                v => v,
                v => TrangThaiThanhToan.FromValue(v, null)!.Value)
            .IsRequired();
            
        builder.HasOne<HoaDon>()
            .WithMany()
            .HasForeignKey(x => x.HoaDonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
