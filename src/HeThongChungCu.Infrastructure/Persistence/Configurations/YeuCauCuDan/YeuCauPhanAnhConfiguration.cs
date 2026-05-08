using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class YeuCauPhanAnhConfiguration : IEntityTypeConfiguration<YeuCauPhanAnh>
{
    public void Configure(EntityTypeBuilder<YeuCauPhanAnh> builder)
    {
        // TPH: Inherits table from YeuCau
        builder.HasBaseType<YeuCau>();

        builder.Property(x => x.LoaiPhanAnhId)
            .HasConversion(v => v.Value, v => LoaiPhanAnh.FromValue(v, null)!)
            .IsRequired();

        builder.Property(x => x.TrangThaiPhanAnhId)
            .HasConversion(v => v.Value, v => TrangThaiPhanAnh.FromValue(v, null)!)
            .IsRequired();

        builder.Property(x => x.TieuDe)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.DiemDanhGia);
        builder.Property(x => x.NhanXetDanhGia).HasMaxLength(500);
        builder.Property(x => x.NgayDanhGia);

        // Chat replies relationship
        builder.HasMany(x => x.TraLoiPhanAnhs)
            .WithOne(y => y.YeuCauPhanAnh)
            .HasForeignKey(x => x.YeuCauPhanAnhId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.TraLoiPhanAnhs)
            .HasField("_traLoiPhanAnhs")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Files relationship
        builder.HasMany(x => x.TepYeuCauPhanAnhs)
            .WithOne(x => x.YeuCauPhanAnh)
            .HasForeignKey(x => x.YeuCauPhanAnhId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.TepYeuCauPhanAnhs)
            .HasField("_tepYeuCauPhanAnhs")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
