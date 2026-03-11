using HeThongChungCu.Domain.Entities.ChungCu;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.ChungCu;

public class ToaNhaConfiguration : IEntityTypeConfiguration<ToaNha>
{
    public void Configure(EntityTypeBuilder<ToaNha> builder)
    {
        builder.ToTable("ToaNhas");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.MaToaNha)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(t => t.MaToaNha).IsUnique();

        builder.Property(t => t.TenToaNha)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.DiaChi)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(t => t.MoTa)
            .HasMaxLength(500);

        builder.Property(t => t.TrangThaiToaNhaId)
            .IsRequired();

        builder.HasMany(t => t.Tangs)
            .WithOne(t => t.ToaNha)
            .HasForeignKey(t => t.ToaNhaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
