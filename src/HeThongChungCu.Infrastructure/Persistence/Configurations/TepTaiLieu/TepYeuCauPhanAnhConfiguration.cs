using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class TepYeuCauPhanAnhConfiguration : IEntityTypeConfiguration<TepYeuCauPhanAnh>
{
    public void Configure(EntityTypeBuilder<TepYeuCauPhanAnh> builder)
    {
        builder.HasBaseType<TepTaiLieu>();

        builder.Property(x => x.YeuCauPhanAnhId)
            .HasColumnName("YeuCauId");

        builder.HasOne(x => x.YeuCauPhanAnh)
            .WithMany(y => y.TepYeuCauPhanAnhs)
            .HasForeignKey(x => x.YeuCauPhanAnhId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
