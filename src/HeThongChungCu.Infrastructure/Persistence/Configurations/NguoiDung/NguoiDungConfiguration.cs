using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class NguoiDungConfiguration : IEntityTypeConfiguration<NguoiDung>
{
    public void Configure(EntityTypeBuilder<NguoiDung> builder)
    {
        builder.ToTable("NguoiDung");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Ten)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Ho)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.NgaySinh)
            .IsRequired();

        builder.Property(u => u.GioiTinhId)
            .HasConversion(
                v => v.Value,
                v => GioiTinh.FromValue(v, null)!
            )
            .IsRequired();


        builder.HasIndex(u => u.SoDienThoai)
            .IsUnique()
            .HasFilter("[SoDienThoai] IS NOT NULL");

        builder.HasIndex(u => u.CCCD)
            .IsUnique()
            .HasFilter("[CCCD] IS NOT NULL");

        builder.HasMany(u => u.TaiLieu)
            .WithOne()
            .HasForeignKey(d => d.NguoiDungId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(u => u.TaiLieu)
            .HasField("_documents")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
