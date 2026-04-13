using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HeThongChungCu.Domain.ValueObjects;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class ToaNhaConfiguration : IEntityTypeConfiguration<ToaNha>
{
    public void Configure(EntityTypeBuilder<ToaNha> builder)
    {
        builder.ToTable("ToaNha");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.MaToaNha)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(t => t.MaToaNha).IsUnique();

        builder.Property(t => t.TenToaNha)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Block)
            .IsRequired()
            .HasMaxLength(1);

        builder.OwnsOne(t => t.DiaChi, da =>
        {
            da.Property(p => p.FullAddress)
                .HasColumnName("DiaChi")
                .IsRequired()
                .HasMaxLength(255);
        });

        builder.Property(t => t.MoTa)
            .HasMaxLength(500);

        builder.Property(t => t.TrangThaiToaNhaId)
            .HasConversion(
                v => v.Value,
                v => TrangThaiToaNha.FromValue(v, null)!
            )
            .IsRequired();

        builder.HasMany(t => t.Tangs)
            .WithOne(t => t.ToaNha)
            .HasForeignKey(t => t.ToaNhaId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
