using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class TepYeuCauPhuongTienConfiguration : IEntityTypeConfiguration<TepYeuCauPhuongTien>
{
    public void Configure(EntityTypeBuilder<TepYeuCauPhuongTien> builder)
    {
        builder.HasBaseType<TepTaiLieu>();

        builder.Property(x => x.YeuCauPhuongTienId)
            .HasColumnName("YeuCauId");

        builder.HasOne(x => x.YeuCauPhuongTien)
            .WithMany(y => y.YeuCauHinhAnhPhuongTiens)
            .HasForeignKey(x => x.YeuCauPhuongTienId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
