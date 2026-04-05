using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class DichVuConfiguration : IEntityTypeConfiguration<DichVu>
{
    public void Configure(EntityTypeBuilder<DichVu> builder)
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

        builder.HasOne(x => x.Icon)
            .WithMany()
            .HasForeignKey(x => x.IconId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<DoiTac>()
            .WithMany()
            .HasForeignKey(x => x.DoiTacId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.BangGias)
            .WithOne()
            .HasForeignKey(x => x.DichVuId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Navigation(x => x.BangGias)
            .HasField("_bangGias")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
