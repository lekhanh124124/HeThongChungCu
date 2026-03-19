using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.HoaDon;

public class LaiChamTraConfiguration : IEntityTypeConfiguration<LaiChamTra>
{
    public void Configure(EntityTypeBuilder<LaiChamTra> builder)
    {
        builder.ToTable("LaiChamTras");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.SoTienGoc)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.LaiSuat)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.SoTienLai)
            .HasColumnType("decimal(18,2)");
    }
}
