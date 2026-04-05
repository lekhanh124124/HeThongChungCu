using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.QuanHeCuTru;

public class TaiLieuNguoiDungConfiguration : IEntityTypeConfiguration<TaiLieuNguoiDung>
{
    public void Configure(EntityTypeBuilder<TaiLieuNguoiDung> builder)
    {
        builder.HasBaseType<TaiLieu>();

        builder.HasOne(x => x.NguoiDung)
            .WithMany(u => u.TaiLieu)
            .HasForeignKey(x => x.NguoiDungId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false);

        builder.HasMany(x => x.Files)
            .WithOne(f => f.TaiLieuNguoiDung)
            .HasForeignKey(f => f.TaiLieuNguoiDungId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Files)
            .HasField("_files")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
