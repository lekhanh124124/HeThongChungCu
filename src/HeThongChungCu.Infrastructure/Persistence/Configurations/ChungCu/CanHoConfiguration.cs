using HeThongChungCu.Domain.Entities.ChungCu;
using HeThongChungCu.Domain.Entities.PhuongTien;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.ChungCu;

public class CanHoConfiguration : IEntityTypeConfiguration<CanHo>
{
    public void Configure(EntityTypeBuilder<CanHo> builder)
    {
        builder.ToTable("CanHos");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.MaCanHo)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(c => c.MaCanHo).IsUnique();

        builder.Property(c => c.ToaNhaId)
            .IsRequired();

        builder.Property(c => c.DienTich)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.Tang)
            .IsRequired();

        builder.Property(c => c.SoPhongNgu)
            .IsRequired();

        builder.Property(c => c.SoPhongTam)
            .IsRequired();

        builder.Property(c => c.LoaiCanHoId)
            .IsRequired();

        builder.Property(c => c.TinhTrangCanHoId)
            .IsRequired();

        builder.HasOne<ToaNha>()
            .WithMany(t => t.CanHos)
            .HasForeignKey(c => c.ToaNhaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.PhuongTiens)
            .WithOne()
            .HasForeignKey(p => p.CanHoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
