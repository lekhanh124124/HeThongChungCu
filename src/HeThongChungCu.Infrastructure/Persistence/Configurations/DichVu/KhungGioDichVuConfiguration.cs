using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.DichVu;

public class KhungGioDichVuConfiguration : IEntityTypeConfiguration<KhungGioDichVu>
{
    public void Configure(EntityTypeBuilder<KhungGioDichVu> builder)
    {
        builder.ToTable("KhungGioDichVu");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenKhungGio)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.GioBatDau)
            .IsRequired();

        builder.Property(x => x.GioKetThuc)
            .IsRequired();

        builder.HasOne(x => x.DichVu)
            .WithMany(x => x.KhungGios)
            .HasForeignKey(x => x.DichVuId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.Property(x => x.NgayTrongTuan)
            .HasConversion(
                v => v == null ? (int?)null : v.Value,
                v => v == null ? null : NgayTrongTuan.FromValue(v.Value, null))
            .IsRequired(false);
    }
}
