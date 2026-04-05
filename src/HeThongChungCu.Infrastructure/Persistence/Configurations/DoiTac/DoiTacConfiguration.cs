using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class DoiTacConfiguration : IEntityTypeConfiguration<DoiTac>
{
    public void Configure(EntityTypeBuilder<DoiTac> builder)
    {
        builder.ToTable("DoiTac");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenDoiTac)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.TenCongTy)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.NguoiDaiDien)
            .HasMaxLength(100);

        builder.Property(x => x.SoGiayPhepKD)
            .HasMaxLength(50);

        builder.Property(x => x.MaSoThue)
            .HasMaxLength(50);

        builder.OwnsOne(x => x.DiaChi, da =>
        {
            da.Property(p => p.FullAddress)
                .HasColumnName("DiaChi")
                .HasMaxLength(500);
        });

        builder.OwnsOne(x => x.SoDienThoai, sd =>
        {
            sd.Property(p => p.Value)
                .HasColumnName("SoDienThoai")
                .HasMaxLength(20);
        });

        builder.OwnsOne(x => x.Email, em =>
        {
            em.Property(p => p.Value)
                .HasColumnName("Email")
                .HasMaxLength(100);
        });

        builder.Property(x => x.TrangThaiHopDongId)
            .HasConversion(
                v => v.Value,
                v => TrangThaiHopDong.FromValue(v, null)!)
            .IsRequired();

        builder.Property(x => x.NgayKyHopDong);
        builder.Property(x => x.NgayHetHan);
    }
}
