using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

        builder.Property(x => x.DiaChi)
            .HasMaxLength(500);

        builder.Property(x => x.SoDienThoai)
            .HasMaxLength(20);

        builder.Property(x => x.Email)
            .HasMaxLength(100);

        builder.Property(x => x.TrangThaiHopDongId)
            .HasConversion(
                v => v.Value,
                v => TrangThaiHopDong.FromValue(v, null)!)
            .IsRequired();

        builder.Property(x => x.NgayKyHopDong);
        builder.Property(x => x.NgayHetHan);
    }
}
