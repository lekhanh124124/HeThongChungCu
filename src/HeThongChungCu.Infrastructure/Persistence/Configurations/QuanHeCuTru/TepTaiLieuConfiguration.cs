using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.QuanHeCuTru;

public class TepTaiLieuConfiguration : IEntityTypeConfiguration<TepTaiLieu>
{
    public void Configure(EntityTypeBuilder<TepTaiLieu> builder)
    {
        builder.ToTable("TepTaiLieu");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.FileUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Size)
            .IsRequired();

        builder.Property(x => x.IsUsed)
            .IsRequired()
            .HasDefaultValue(false);
    }
}
