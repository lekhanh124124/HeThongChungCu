using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class BangGiaConfiguration : IEntityTypeConfiguration<BangGia>
{
    public void Configure(EntityTypeBuilder<BangGia> builder)
    {
        builder.ToTable("BangGia");

        builder.HasKey(x => x.Id);

        builder.HasDiscriminator(x => x.LoaiDinhGiaId)
            .HasValue<BangGiaCoDinh>(LoaiDinhGia.CoDinh)
            .HasValue<BangGiaLuyTien>(LoaiDinhGia.LuyTien)
            .HasValue<BangGiaKhungGio>(LoaiDinhGia.TheoKhungGio)
            .HasValue<BangGiaLoaiCanHo>(LoaiDinhGia.TheoDienTich);

        builder.Property(x => x.TenBangGia)
            .HasMaxLength(100)
            .IsRequired();

        builder.OwnsOne(x => x.ThoiGian, thoiGian =>
        {
            thoiGian.Property(v => v.NgayBatDau)
                .HasColumnName("NgayApDung")
                .IsRequired();

            thoiGian.Property(v => v.NgayKetThuc)
                .HasColumnName("NgayKetThuc");
        });

        builder.Property(x => x.LoaiDinhGiaId)
            .HasConversion(
                v => v.Value,
                v => LoaiDinhGia.FromValue(v, null)!)
            .IsRequired();
            
        // Individual Subclass configurations are handled in their respective configuration classes
    }
}