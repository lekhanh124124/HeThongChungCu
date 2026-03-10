using HeThongChungCu.Domain.Entities.ChungCu;
using HeThongChungCu.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.ChungCu;

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
            .IsRequired();

        builder.Property(q => q.NgayBatDau)
            .IsRequired();

        builder.Property(q => q.NgayKetThuc);

        builder.Property(q => q.IsKetThuc)
            .IsRequired();

        builder.HasOne<CanHo>()
            .WithMany(c => c.QuanHeCuTrus)
            .HasForeignKey(q => q.CanHoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(q => q.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
