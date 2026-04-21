using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class TepYeuCauThiCongConfiguration : IEntityTypeConfiguration<TepYeuCauThiCong>
{
    public void Configure(EntityTypeBuilder<TepYeuCauThiCong> builder)
    {
        builder.HasBaseType<TepTaiLieu>();

        builder.Property(x => x.YeuCauThiCongId)
            .HasColumnName("YeuCauId");

        builder.HasOne(x => x.YeuCauThiCong)
            .WithMany(y => y.TepYeuCauThiCongs)
            .HasForeignKey(x => x.YeuCauThiCongId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
