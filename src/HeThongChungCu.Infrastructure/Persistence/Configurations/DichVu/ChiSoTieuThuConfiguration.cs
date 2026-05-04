using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class ChiSoTieuThuConfiguration : IEntityTypeConfiguration<ChiSoTieuThu>
{
    public void Configure(EntityTypeBuilder<ChiSoTieuThu> builder)
    {
        builder.ToTable("ChiSoTieuThu");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CanHoId)
            .IsRequired();

        builder.Property(x => x.DichVuId)
            .IsRequired();

        builder.Property(x => x.ChiSoCu)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(x => x.ChiSoMoi)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(x => x.Thang)
            .IsRequired();

        builder.Property(x => x.Nam)
            .IsRequired();

        builder.Property(x => x.NgayGhiNhan)
            .IsRequired();

        builder.Property(x => x.TrangThaiChiSoId)
            .HasConversion(
                v => v.Value,
                v => TrangThaiChiSo.FromValue(v, null)!)
            .IsRequired();

        builder.Property(x => x.AnhDongHoId);

        builder.Property(x => x.GhiChu)
            .HasMaxLength(500);

        builder.Property(x => x.MaTraCuu)
            .HasMaxLength(100);

        builder.HasIndex(x => x.MaTraCuu)
            .IsUnique()
            .HasFilter("[MaTraCuu] IS NOT NULL");

        // Relationships
        builder.HasOne<CanHo>()
            .WithMany()
            .HasForeignKey(x => x.CanHoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<HeThongChungCu.Domain.Entities.DichVu>()
            .WithMany()
            .HasForeignKey(x => x.DichVuId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<HoaDon>()
            .WithMany()
            .HasForeignKey(x => x.HoaDonId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.AnhDongHo)
            .WithMany()
            .HasForeignKey(x => x.AnhDongHoId)
            .OnDelete(DeleteBehavior.SetNull);

        // Unique index to prevent duplicate readings for the same month/service/apartment
        builder.HasIndex(x => new { x.CanHoId, x.DichVuId, x.Thang, x.Nam })
            .IsUnique();
    }
}
