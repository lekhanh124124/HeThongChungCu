using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class ChiTietHoaDonThiCongConfiguration : IEntityTypeConfiguration<ChiTietHoaDonThiCong>
{
    public void Configure(EntityTypeBuilder<ChiTietHoaDonThiCong> builder)
    {
        builder.Property(x => x.YeuCauThiCongId);

        builder.Property(x => x.LoaiChiPhiThiCongId)
            .HasConversion(
                v => v.Value,
                v => LoaiChiPhiThiCong.FromValue(v, null)!)
            .IsRequired();

        // Link to YeuCauThiCong (Optional tracking)
        builder.HasOne<YeuCauThiCong>()
            .WithMany()
            .HasForeignKey(x => x.YeuCauThiCongId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
