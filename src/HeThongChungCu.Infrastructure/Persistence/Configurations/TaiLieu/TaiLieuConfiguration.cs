using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class TaiLieuConfiguration : IEntityTypeConfiguration<TaiLieu>
{
    public void Configure(EntityTypeBuilder<TaiLieu> builder)
    {
        builder.ToTable("TaiLieu");

        builder.HasKey(x => x.Id);

        builder.HasDiscriminator(x => x.LoaiTaiLieuId)
            .HasValue<TaiLieuNguoiDung>(LoaiTaiLieu.NguoiDung)
            .HasValue<YeuCauTaiLieuCuTru>(LoaiTaiLieu.YeuCauCuTru);

        builder.Property(x => x.LoaiTaiLieuId)
            .HasConversion(
                v => v.Value,
                v => LoaiTaiLieu.FromValue(v, null)!)
            .IsRequired();

        builder.Property(x => x.SoGiayTo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.LoaiGiayToId)
            .HasConversion(
                v => v.Value,
                v => LoaiGiayTo.FromValue(v, null)!)
            .IsRequired();
            
        builder.Property(x => x.NgayPhatHanh);
    }
}
