using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class ChiTietHoaDonSuaChuaConfiguration : IEntityTypeConfiguration<ChiTietHoaDonSuaChua>
{
    public void Configure(EntityTypeBuilder<ChiTietHoaDonSuaChua> builder)
    {
        builder.Property(x => x.YeuCauSuaChuaId)
            .HasColumnName("YeuCauId");
        
        // Link to YeuCauSuaChua (Optional tracking)
        builder.HasOne<YeuCauSuaChua>()
            .WithMany()
            .HasForeignKey(x => x.YeuCauSuaChuaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
