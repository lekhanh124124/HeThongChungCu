using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class ChiSoTieuThuConfiguration : IEntityTypeConfiguration<ChiSoTieuThu>
{
    public void Configure(EntityTypeBuilder<ChiSoTieuThu> builder)
    {
        builder.ToTable("ChiSoTieuThus");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.LoaiDichVuId)
            .HasConversion(
                v => v.Value,
                v => LoaiDichVu.FromValue(v, null)!
            )
            .IsRequired();

        builder.Property(c => c.ChiSo)
            .IsRequired();

        builder.Property(c => c.IsLock)
            .IsRequired();

        builder.Property(c => c.Thang)
            .IsRequired();

        builder.Property(c => c.Nam)
            .IsRequired();

        builder.Property(c => c.NgayChot)
            .IsRequired();

        builder.HasOne<CanHo>()
            .WithMany()
            .HasForeignKey(c => c.CanHoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
