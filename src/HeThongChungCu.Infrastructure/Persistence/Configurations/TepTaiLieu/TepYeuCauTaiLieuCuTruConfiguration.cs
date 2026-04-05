using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class TepYeuCauTaiLieuCuTruConfiguration : IEntityTypeConfiguration<TepYeuCauTaiLieuCuTru>
{
    public void Configure(EntityTypeBuilder<TepYeuCauTaiLieuCuTru> builder)
    {
        builder.HasBaseType<TepTaiLieu>();

        builder.Property(x => x.YeuCauTaiLieuCuTruId)
            .HasColumnName("TaiLieuId");

        builder.HasOne(x => x.YeuCauTaiLieuCuTru)
            .WithMany(y => y.Files)
            .HasForeignKey(x => x.YeuCauTaiLieuCuTruId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
