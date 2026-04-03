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

        builder.Property(x => x.SoTien)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.NgayGhiNhan)
            .IsRequired();

        builder.Property(x => x.FileHoaDonId);

        builder.Property(x => x.TrangThaiThanhToanId)
            .HasConversion(
                v => v.Value,
                v => TrangThaiThanhToanDoiTac.FromValue(v, null)!)
            .IsRequired();

        builder.HasOne<DoiTac>()
            .WithMany()
            .HasForeignKey(x => x.DoiTacId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<TepTaiLieu>()
            .WithMany()
            .HasForeignKey(x => x.FileHoaDonId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
