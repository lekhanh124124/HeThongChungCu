using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.DichVu;

public class DangKyDichVuConfiguration : IEntityTypeConfiguration<DangKyDichVu>
{
    public void Configure(EntityTypeBuilder<DangKyDichVu> builder)
    {
        builder.ToTable("DangKyDichVu");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.NgayBatDau)
            .IsRequired();

        builder.Property(x => x.SoLuong)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne<CanHo>()
            .WithMany()
            .HasForeignKey(x => x.CanHoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Domain.Entities.DichVu>()
            .WithMany()
            .HasForeignKey(x => x.DichVuId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
