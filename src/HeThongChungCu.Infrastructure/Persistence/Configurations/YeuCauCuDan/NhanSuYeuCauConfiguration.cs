using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class NhanSuYeuCauConfiguration : IEntityTypeConfiguration<NhanSuYeuCau>
{
    public void Configure(EntityTypeBuilder<NhanSuYeuCau> builder)
    {
        builder.ToTable("NhanSuYeuCau");

        builder.HasKey(e => e.Id);

        builder.HasDiscriminator(x => x.LoaiNhanSuId)
            .HasValue<NhanSuThiCong>(LoaiNhanSuYeuCau.ThiCong)
            .HasValue<NhanSuSuaChua>(LoaiNhanSuYeuCau.SuaChua);

        builder.Property(x => x.LoaiNhanSuId)
            .HasConversion(
                v => v.Value,
                v => LoaiNhanSuYeuCau.FromValue(v, null)!)
            .IsRequired();

        builder.Property(e => e.HoTen).HasMaxLength(100);
        builder.Property(e => e.SoCCCD).HasMaxLength(20);
        builder.Property(e => e.VaiTro).HasMaxLength(100);
        builder.Property(e => e.GhiChu).HasMaxLength(500);

        builder.OwnsOne(x => x.SoDienThoai, cb =>
        {
            cb.Property(p => p.Value)
                .HasColumnName("SoDienThoai")
                .HasMaxLength(20);
        });

        builder.HasOne<YeuCau>()
            .WithMany()
            .HasForeignKey(x => x.YeuCauId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<NhanVien>()
            .WithMany()
            .HasForeignKey(x => x.NhanVienId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
