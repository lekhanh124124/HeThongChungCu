using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class QuanHeCuTruConfiguration : IEntityTypeConfiguration<QuanHeCuTru>
{
    public void Configure(EntityTypeBuilder<QuanHeCuTru> builder)
    {
        builder.ToTable("QuanHeCuTrus");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.CanHoId)
            .IsRequired();

        builder.Property(q => q.UserId)
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

        builder.HasOne<User>()
            .WithMany(u => u.QuanHeCuTrus)
            .HasForeignKey(q => q.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
