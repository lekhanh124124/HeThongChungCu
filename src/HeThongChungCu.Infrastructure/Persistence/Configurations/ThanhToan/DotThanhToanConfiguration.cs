using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class DotThanhToanConfiguration : IEntityTypeConfiguration<DotThanhToan>
{
    public void Configure(EntityTypeBuilder<DotThanhToan> builder)
    {
        builder.ToTable("DotThanhToan");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenDot)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.GhiChu)
            .HasMaxLength(500);

        builder.OwnsOne(x => x.KyThanhToan, kyBuilder =>
        {
            kyBuilder.Property(k => k.Thang).HasColumnName("Thang").IsRequired();
            kyBuilder.Property(k => k.Nam).HasColumnName("Nam").IsRequired();
        });

        builder.Property(x => x.TrangThaiDotThanhToanId)
            .HasConversion(
                v => v.Value,
                v => TrangThaiDotThanhToan.FromValue(v, null)!)
            .IsRequired();

        builder.Property(x => x.NgayPhatHanh);
    }
}
