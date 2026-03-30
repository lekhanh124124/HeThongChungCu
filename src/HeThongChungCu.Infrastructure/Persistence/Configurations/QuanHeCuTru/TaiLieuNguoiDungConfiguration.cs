using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.QuanHeCuTru;

public class TaiLieuNguoiDungConfiguration : IEntityTypeConfiguration<TaiLieuNguoiDung>
{
    public void Configure(EntityTypeBuilder<TaiLieuNguoiDung> builder)
    {
        builder.ToTable("TaiLieuNguoiDung");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SoGiayTo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.LoaiGiayToId)
            .HasConversion(
                v => v.Value,
                v => HeThongChungCu.Domain.Enums.LoaiGiayTo.FromValue(v, null)!)
            .IsRequired();

        builder.HasOne(x => x.NguoiDung)
            .WithMany(u => u.TaiLieu)
            .HasForeignKey(x => x.NguoiDungId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder.HasMany(x => x.Files)
            .WithMany()
            .UsingEntity(j => j.ToTable("TepTaiLieuNguoiDung"));

        builder.Navigation(x => x.Files)
            .HasField("_files")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
