using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class ChiTietHoaDonTieuThuConfiguration : IEntityTypeConfiguration<ChiTietHoaDonTieuThu>
{
    public void Configure(EntityTypeBuilder<ChiTietHoaDonTieuThu> builder)
    {
        builder.Property(x => x.ChiSoCu)
            .HasPrecision(18, 2);

        builder.Property(x => x.ChiSoMoi)
            .HasPrecision(18, 2);

        builder.Property(x => x.DichVuId)
            .HasColumnName("DichVuId")
            .IsRequired();

        builder.HasOne<Domain.Entities.DichVu>()
            .WithMany()
            .HasForeignKey(x => x.DichVuId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
