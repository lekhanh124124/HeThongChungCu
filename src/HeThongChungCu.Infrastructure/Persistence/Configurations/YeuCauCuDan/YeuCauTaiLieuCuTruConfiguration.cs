using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class YeuCauTaiLieuCuTruConfiguration : IEntityTypeConfiguration<YeuCauTaiLieuCuTru>
{
    public void Configure(EntityTypeBuilder<YeuCauTaiLieuCuTru> builder)
    {
        builder.ToTable("YeuCauTaiLieuCuTru");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SoGiayTo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.LoaiGiayToId)
            .HasConversion(
                v => v.Value,
                v => LoaiGiayTo.FromValue(v, null)!)
            .IsRequired();

        builder.HasMany(x => x.Files)
            .WithMany()
            .UsingEntity(j => j.ToTable("TepYeuCauTaiLieuCuTru"));
    }
}
