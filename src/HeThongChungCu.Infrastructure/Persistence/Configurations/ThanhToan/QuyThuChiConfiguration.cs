using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.ThanhToan;

public class QuyThuChiConfiguration : IEntityTypeConfiguration<QuyThuChi>
{
    public void Configure(EntityTypeBuilder<QuyThuChi> builder)
    {
        builder.ToTable("QuyThuChi");

        builder.Property(x => x.LoaiGiaoDichId)
            .HasConversion(
                x => x.Value,
                x => LoaiThuChi.FromValue(x, null)!)
            .IsRequired()
            .HasColumnName("LoaiGiaoDichId");

        // TPH has been removed, map directly as a single concrete class

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MaGiaoDich)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.TongSoTien)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.NgayGiaoDich)
            .IsRequired();

        builder.Property(x => x.PhuongThucThanhToanId)
            .HasConversion(
                x => x.Value,
                x => PhuongThucThanhToan.FromValue(x, null)!)
            .IsRequired();

        builder.Property(x => x.NguoiGiaoDich)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ChungTuGoc)
            .HasMaxLength(200);

        builder.HasIndex(x => x.MaGiaoDich).IsUnique();
        builder.HasIndex(x => x.NgayGiaoDich);
        builder.HasIndex("LoaiGiaoDichId"); // Index for the discriminator
    }
}
