using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.ThanhToan;

public class ChiTietQuyThuChiConfiguration : IEntityTypeConfiguration<ChiTietQuyThuChi>
{
    public void Configure(EntityTypeBuilder<ChiTietQuyThuChi> builder)
    {
        builder.ToTable("ChiTietQuyThuChi");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SoTien)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.NhomThongKe)
            .HasColumnName("NhomThongKe")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.GhiChu)
            .HasColumnName("GhiChu")
            .HasMaxLength(500);

        builder.HasOne<HeThongChungCu.Domain.Entities.DichVu>()
            .WithMany()
            .HasForeignKey(x => x.DichVuId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relationships
        builder.HasOne<QuyThuChi>()
            .WithMany(x => x.ChiTiets)
            .HasForeignKey(x => x.QuyThuChiId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

