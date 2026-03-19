using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.HoaDon;

public class ThanhToanConfiguration : IEntityTypeConfiguration<ThanhToan>
{
    public void Configure(EntityTypeBuilder<ThanhToan> builder)
    {
        builder.ToTable("ThanhToans");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.SoTien)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.NgayThanhToan)
            .IsRequired();

        builder.Property(x => x.PhuongThucThanhToanId)
            .HasConversion(
                v => v.Value,
                v => PhuongThucThanhToan.FromValue(v, null)!
            )
            .IsRequired();

        builder.Property(x => x.MaGiaoDich)
            .HasMaxLength(100);

        builder.Property(x => x.NoiDung)
            .HasMaxLength(500);

        builder.HasOne<Domain.Entities.HoaDon>()
            .WithMany(h => h.ThanhToans)
            .HasForeignKey(x => x.HoaDonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
