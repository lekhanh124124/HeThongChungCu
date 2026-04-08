using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class DichVuConfiguration : IEntityTypeConfiguration<HeThongChungCu.Domain.Entities.DichVu>
{
    public void Configure(EntityTypeBuilder<HeThongChungCu.Domain.Entities.DichVu> builder)
    {
        builder.ToTable("DichVu");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MaDichVu)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.TenDichVu)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.DonViTinh)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.MoTa)
            .HasMaxLength(1000);

        builder.Property(x => x.LoaiDichVuId)
            .HasConversion(
                v => v.Value,
                v => LoaiDichVu.FromValue(v, null)!)
            .IsRequired();

        builder.Property(x => x.TrangThaiId)
            .HasConversion(
                v => v.Value,
                v => TrangThaiDichVu.FromValue(v, null)!)
            .IsRequired();

        builder.HasOne(x => x.Icon)
            .WithMany()
            .HasForeignKey(x => x.IconId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.BangGias)
            .WithOne(x => x.DichVu)
            .HasForeignKey(x => x.DichVuId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Navigation(x => x.BangGias)
            .HasField("_bangGias")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.KhungGios)
            .WithOne(x => x.DichVu)
            .HasForeignKey(x => x.DichVuId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.KhungGios)
            .HasField("_khungGios")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
