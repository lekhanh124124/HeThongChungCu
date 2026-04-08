using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class DangKyDichVuConfiguration : IEntityTypeConfiguration<DangKyDichVu>
{
    public void Configure(EntityTypeBuilder<DangKyDichVu> builder)
    {
        builder.ToTable("DangKyDichVu");

        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.ThoiGian, thoiGian =>
        {
            thoiGian.Property(v => v.NgayBatDau)
                .HasColumnName("NgayBatDau")
                .IsRequired();

            thoiGian.Property(v => v.NgayKetThuc)
                .HasColumnName("NgayKetThuc");
        });

        builder.Property(x => x.TrangThaiDangKyId)
            .HasConversion(
                v => v.Value,
                v => TrangThaiDangKy.FromValue(v, null)!)
            .IsRequired();

        builder.HasOne<CanHo>()
            .WithMany()
            .HasForeignKey(x => x.CanHoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<HeThongChungCu.Domain.Entities.DichVu>()
            .WithMany()
            .HasForeignKey(x => x.DichVuId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
