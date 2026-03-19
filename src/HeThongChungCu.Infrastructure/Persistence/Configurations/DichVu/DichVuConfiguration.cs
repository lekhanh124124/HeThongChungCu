using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.DichVu;

public class DichVuConfiguration : IEntityTypeConfiguration<Domain.Entities.DichVu>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.DichVu> builder)
    {
        builder.ToTable("DichVus");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.MaDichVu)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.TenDichVu)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.DonViTinh)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.MaDichVu).IsUnique();
    }
}
