using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class YeuCauPhuongTienConfiguration : IEntityTypeConfiguration<YeuCauPhuongTien>
{
    public void Configure(EntityTypeBuilder<YeuCauPhuongTien> builder)
    {
        builder.ToTable("YeuCauPhuongTien");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.YeuCauTenPhuongTien).HasMaxLength(100).IsRequired();
        builder.Property(e => e.YeuCauBienSo).HasMaxLength(20).IsRequired();
        builder.Property(e => e.YeuCauMauXe).HasMaxLength(50);
        builder.Property(e => e.NoiDung).HasMaxLength(1000);
        builder.Property(e => e.LyDo).HasMaxLength(500);

        builder.Property(x => x.YeuCauLoaiPhuongTienId)
            .HasConversion(
                v => v.Value,
                v => LoaiPhuongTien.FromValue(v, null)!)
            .IsRequired();

        builder.Property(x => x.LoaiYeuCauId)
            .HasConversion(
                v => v.Value,
                v => LoaiYeuCau.FromValue(v, null)!)
            .IsRequired();

        builder.Property(x => x.TrangThaiId)
            .HasConversion(
                v => v.Value,
                v => TrangThaiYeuCau.FromValue(v, null)!)
            .IsRequired();

        builder.HasOne<CanHo>()
            .WithMany()
            .HasForeignKey(x => x.CanHoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.YeuCauHinhAnhPhuongTiens)
            .WithMany()
            .UsingEntity(j => j.ToTable("TepYeuCauHinhAnhPhuongTien"));
    }
}
