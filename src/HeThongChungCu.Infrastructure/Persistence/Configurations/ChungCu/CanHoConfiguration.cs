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

        builder.Property(c => c.TenCanHo)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(c => c.MaCanHo).IsUnique();


        builder.Property(c => c.DienTich)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.TangId)
            .IsRequired();

        builder.HasOne(c => c.Tang)
            .WithMany(t => t.CanHos)
            .HasForeignKey(c => c.TangId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(c => c.SoPhongNgu)
            .IsRequired();

        builder.Property(c => c.SoPhongTam)
            .IsRequired();

        builder.Property(c => c.LoaiCanHoId)
            .IsRequired();

        builder.Property(c => c.TinhTrangCanHoId)
            .IsRequired();


        builder.HasMany(c => c.PhuongTiens)
            .WithOne()
            .HasForeignKey(p => p.CanHoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
