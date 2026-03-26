using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class QuanHeCuTruConfiguration : IEntityTypeConfiguration<HeThongChungCu.Domain.Entities.QuanHeCuTru>
{
    public void Configure(EntityTypeBuilder<HeThongChungCu.Domain.Entities.QuanHeCuTru> builder)
    {
        builder.ToTable("QuanHeCuTru");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.CanHoId)
            .IsRequired();

        builder.Property(x => x.NguoiDungId)
            .IsRequired();

        builder.Property(q => q.LoaiQuanHeCuTruId)
            .HasConversion(
                v => v.Value,
                v => LoaiQuanHeCuTru.FromValue(v, null)!
            )
            .IsRequired();

        builder.Property(q => q.NgayBatDau)
            .IsRequired();

        builder.Property(q => q.NgayKetThuc);

        builder.Property(q => q.TrangThaiCuTruId)
            .HasConversion(
                v => v.Value,
                v => TrangThaiCuTru.FromValue(v, null)!
            )
            .IsRequired();

        builder.HasOne<CanHo>()
            .WithMany()
            .HasForeignKey(q => q.CanHoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<NguoiDung>()
            .WithMany()
            .HasForeignKey(q => q.NguoiDungId)
            .OnDelete(DeleteBehavior.Cascade);

        // Filtered index to prevent duplicate ACTIVE residency for the same person in the same apartment
        // Assuming TrangThaiCuTru.DangCuTru.Value is 1
        builder.HasIndex(q => new { q.CanHoId, q.NguoiDungId })
            .IsUnique()
            .HasFilter("[TrangThaiCuTruId] = 1");
    }
}
