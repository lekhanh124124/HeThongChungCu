using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class YeuCauThiCongConfiguration : IEntityTypeConfiguration<YeuCauThiCong>
{
    public void Configure(EntityTypeBuilder<YeuCauThiCong> builder)
    {
        // TPH: Inherits table from YeuCau

        builder.Property(x => x.HangMucThiCong)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.TenDonViThiCong)
            .HasMaxLength(200);

        builder.Property(x => x.NguoiDaiDien)
            .HasMaxLength(100);

        builder.Property(x => x.DuKienBatDau)
            .IsRequired();

        builder.Property(x => x.DuKienKetThuc)
            .IsRequired();

        builder.OwnsOne(e => e.SoDienThoaiDaiDien, sd =>
        {
            sd.Property(p => p.Value)
                .HasColumnName("SoDienThoaiDaiDien")
                .HasMaxLength(15);
        });

        builder.Property(x => x.TienDatCoc)
            .HasPrecision(18, 2);

        builder.Property(x => x.GhiChuThuCoc)
            .HasMaxLength(200);

        builder.Property(x => x.IsDaThuCoc)
            .IsRequired();

        builder.Property(x => x.TienKhauTru)
            .HasPrecision(18, 2);

        builder.Property(x => x.LyDoKhauTru)
            .HasMaxLength(500);

        builder.Property(x => x.IsDaHoanCoc)
            .IsRequired();

        builder.Property(x => x.NgayDuyetSoBo);

        builder.Property(x => x.TrangThaiThiCongId)
            .HasConversion(
                v => v != null ? v.Value : (int?)null,
                v => v == null ? null : TrangThaiThiCong.FromValue(v.Value, null)!)
            .IsRequired(false);

        builder.HasMany(x => x.TepYeuCauThiCongs)
            .WithOne(x => x.YeuCauThiCong)
            .HasForeignKey(x => x.YeuCauThiCongId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.TepYeuCauThiCongs)
            .HasField("_tepYeuCauThiCongs")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Personnel mapping
        builder.HasMany(x => x.NhanSuThiCongs)
            .WithOne()
            .HasForeignKey(x => x.YeuCauId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.NhanSuThiCongs)
            .HasField("_nhanSuThiCongs")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
