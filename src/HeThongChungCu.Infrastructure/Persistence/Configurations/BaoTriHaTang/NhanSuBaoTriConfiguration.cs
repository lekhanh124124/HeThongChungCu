using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class NhanSuBaoTriConfiguration : IEntityTypeConfiguration<NhanSuBaoTri>
{
    public void Configure(EntityTypeBuilder<NhanSuBaoTri> builder)
    {
        builder.ToTable("NhanSuBaoTri");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.HoTen)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.SoCCCD)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.VaiTro)
            .HasMaxLength(100);

        builder.OwnsOne(x => x.SoDienThoai, cb =>
        {
            cb.Property(p => p.Value)
                .HasColumnName("SoDienThoai")
                .HasMaxLength(20);
        });

        builder.HasOne<NhanVien>()
            .WithMany()
            .HasForeignKey(x => x.NhanVienId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
