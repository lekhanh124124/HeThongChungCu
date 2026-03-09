using HeThongChungCu.Domain.Entities.ChungCu;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.ChungCu;

public class CanHoHinhAnhConfiguration : IEntityTypeConfiguration<CanHoHinhAnh>
{
    public void Configure(EntityTypeBuilder<CanHoHinhAnh> builder)
    {
        builder.ToTable("CanHoHinhAnhs");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.HinhAnhUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.MoTa)
            .HasMaxLength(255);

        builder.Property(c => c.ThuTu)
            .IsRequired();

        builder.HasOne<CanHo>()
            .WithMany(c => c.HinhAnhs)
            .HasForeignKey(c => c.CanHoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
