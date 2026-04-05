using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class TepTaiLieuNguoiDungConfiguration : IEntityTypeConfiguration<TepTaiLieuNguoiDung>
{
    public void Configure(EntityTypeBuilder<TepTaiLieuNguoiDung> builder)
    {
        builder.HasBaseType<TepTaiLieu>();

        builder.Property(x => x.TaiLieuNguoiDungId)
            .HasColumnName("TaiLieuId");

        builder.HasOne(x => x.TaiLieuNguoiDung)
            .WithMany(y => y.Files)
            .HasForeignKey(x => x.TaiLieuNguoiDungId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
