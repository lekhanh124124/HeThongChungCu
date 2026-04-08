using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class HoaDonDoiTacConfiguration : IEntityTypeConfiguration<HoaDonDoiTac>
{
    public void Configure(EntityTypeBuilder<HoaDonDoiTac> builder)
    {
        builder.ToTable("HoaDonDoiTac");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Thang).IsRequired();
        builder.Property(x => x.Nam).IsRequired();

        builder.OwnsOne(x => x.SoTien, giaTien =>
        {
            giaTien.Property(p => p.SoTien)
                .HasColumnName("SoTien")
                .HasPrecision(18, 2)
                .IsRequired();
            giaTien.Ignore(p => p.LoaiTien);
        });

        builder.Property(x => x.NgayGhiNhan)
            .IsRequired();

        builder.Property(x => x.FileHoaDonId);

        builder.Property(x => x.TrangThaiThanhToanId)
            .HasConversion(
                v => v.Value,
                v => TrangThaiThanhToanDoiTac.FromValue(v, null)!)
            .IsRequired();

        builder.HasOne<HopDongDoiTac>()
            .WithMany()
            .HasForeignKey(x => x.HopDongDoiTacId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.FileHoaDon)
            .WithMany()
            .HasForeignKey(x => x.FileHoaDonId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
