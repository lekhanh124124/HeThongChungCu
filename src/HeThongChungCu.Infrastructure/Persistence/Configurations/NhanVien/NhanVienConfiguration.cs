using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class NhanVienConfiguration : IEntityTypeConfiguration<NhanVien>
{
    public void Configure(EntityTypeBuilder<NhanVien> builder)
    {
        builder.ToTable("NhanVien");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MaNhanVien)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(x => x.MaNhanVien).IsUnique();

        builder.Property(x => x.NgayVaoLam)
            .IsRequired();

        builder.Property(x => x.GhiChu)
            .HasMaxLength(500);

        builder.Property(x => x.LoaiNhanVienId)
            .HasConversion(
                v => v.Value,
                v => LoaiNhanVien.FromValue(v, null)!
            )
            .IsRequired();

        builder.Property(x => x.TrangThaiNhanVienId)
            .HasConversion(
                v => v.Value,
                v => TrangThaiNhanVien.FromValue(v, null)!
            )
            .IsRequired();

        builder.HasOne<NguoiDung>()
            .WithMany()
            .HasForeignKey(x => x.NguoiDungId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
