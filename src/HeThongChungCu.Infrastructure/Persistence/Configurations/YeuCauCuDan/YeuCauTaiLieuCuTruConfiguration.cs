using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class YeuCauTaiLieuCuTruConfiguration : IEntityTypeConfiguration<YeuCauTaiLieuCuTru>
{
    public void Configure(EntityTypeBuilder<YeuCauTaiLieuCuTru> builder)
    {
        builder.HasBaseType<TaiLieu>();

        builder.Property(x => x.TaiLieuCuTruId);

        builder.HasMany(x => x.Files)
            .WithOne(f => f.YeuCauTaiLieuCuTru)
            .HasForeignKey(f => f.YeuCauTaiLieuCuTruId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Files)
            .HasField("_files")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
