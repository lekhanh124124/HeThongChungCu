using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class YeuCauCuTruConfiguration : IEntityTypeConfiguration<YeuCauCuTru>
{
    public void Configure(EntityTypeBuilder<YeuCauCuTru> builder)
    {
        builder.ToTable("YeuCauCuTru");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.NoiDung).HasMaxLength(1000);
        builder.Property(e => e.LyDo).HasMaxLength(500);

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


        builder.HasOne(x => x.CanHo)
            .WithMany()
            .HasForeignKey(x => x.CanHoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.QuanHeCuTru)
            .WithMany()
            .HasForeignKey(x => x.QuanHeCuTruId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.Documents)
            .WithOne(x => x.YeuCauCuTru)
            .HasForeignKey(x => x.YeuCauCuTruId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.LyDo)
            .HasMaxLength(500);

        builder.Property(e => e.YeuCauHo)
            .HasMaxLength(50);

        builder.Property(e => e.YeuCauTen)
            .HasMaxLength(50);

        builder.Property(e => e.YeuCauSoDienThoai)
            .HasMaxLength(15);
        builder.Property(x => x.YeuCauNgaySinh);
        builder.Property(x => x.YeuCauGioiTinhId);
        builder.Property(x => x.YeuCauLoaiQuanHeId);
    }
}
