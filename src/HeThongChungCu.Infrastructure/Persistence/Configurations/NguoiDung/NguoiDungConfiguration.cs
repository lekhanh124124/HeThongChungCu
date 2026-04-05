using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class NguoiDungConfiguration : IEntityTypeConfiguration<NguoiDung>
{
    public void Configure(EntityTypeBuilder<NguoiDung> builder)
    {
        builder.ToTable("NguoiDung");

        builder.HasKey(u => u.Id);

        builder.OwnsOne(u => u.DiaChi, da =>
        {
            da.Property(p => p.FullAddress).HasColumnName("DiaChi");
        });

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


        builder.OwnsOne(u => u.SoDienThoai, sd =>
        {
            sd.Property(p => p.Value)
                .HasColumnName("SoDienThoai")
                .HasMaxLength(20);
            sd.HasIndex(p => p.Value)
                .IsUnique()
                .HasFilter("[SoDienThoai] IS NOT NULL");
        });

        builder.HasIndex(u => u.CCCD)
            .IsUnique()
            .HasFilter("[CCCD] IS NOT NULL");

        builder.HasMany(u => u.TaiLieu)
            .WithOne()
            .HasForeignKey(d => d.NguoiDungId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Navigation(u => u.TaiLieu)
            .HasField("_documents")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
