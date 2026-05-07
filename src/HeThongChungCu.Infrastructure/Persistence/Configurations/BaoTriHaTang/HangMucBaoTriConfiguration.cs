using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class HangMucBaoTriConfiguration : IEntityTypeConfiguration<HangMucBaoTri>
{
    public void Configure(EntityTypeBuilder<HangMucBaoTri> builder)
    {
        builder.ToTable("HangMucBaoTri");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.MaHangMuc)
            .HasMaxLength(50)
            .IsRequired();
        builder.HasIndex(x => x.MaHangMuc).IsUnique();

        builder.Property(x => x.TenHangMuc)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.MoTa)
            .HasMaxLength(1000);

        builder.Property(x => x.ChiPhiUocTinh)
            .HasPrecision(18, 2);

        builder.Property(x => x.ChecklistTieuChuan)
            .IsRequired(); // Lưu JSON array
    }
}
