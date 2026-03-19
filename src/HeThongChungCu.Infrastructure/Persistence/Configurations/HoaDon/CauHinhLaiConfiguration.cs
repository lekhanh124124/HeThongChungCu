using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.HoaDon;

public class CauHinhLaiConfiguration : IEntityTypeConfiguration<CauHinhLai>
{
    public void Configure(EntityTypeBuilder<CauHinhLai> builder)
    {
        builder.ToTable("CauHinhLais");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.MaCauHinh)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.LaiSuatThang)
            .HasColumnType("decimal(18,2)");

        builder.HasIndex(x => x.MaCauHinh).IsUnique();
    }
}
