using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class TepPhuongTienConfiguration : IEntityTypeConfiguration<TepPhuongTien>
{
    public void Configure(EntityTypeBuilder<TepPhuongTien> builder)
    {
        builder.HasBaseType<TepTaiLieu>();

        builder.HasOne(x => x.PhuongTien)
            .WithMany(p => p.HinhAnhPhuongTiens)
            .HasForeignKey(x => x.PhuongTienId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
