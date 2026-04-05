using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class TaiKhoanConfiguration : IEntityTypeConfiguration<TaiKhoan>
{
    public void Configure(EntityTypeBuilder<TaiKhoan> builder)
    {
        builder.ToTable("TaiKhoan");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.TenDangNhap)
            .IsRequired()
            .HasMaxLength(50);

        builder.OwnsOne(a => a.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(256);
            email.HasIndex(e => e.Value).IsUnique();
        });

        builder.Property(a => a.MatKhauHash)
            .IsRequired();

        builder.Property(a => a.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(a => a.AnhDaiDien)
            .WithMany()
            .HasForeignKey(a => a.AnhDaiDienId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(a => a.TenDangNhap).IsUnique();

        builder.HasOne<NguoiDung>()
            .WithOne()
            .HasForeignKey<TaiKhoan>(a => a.NguoiDungId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
