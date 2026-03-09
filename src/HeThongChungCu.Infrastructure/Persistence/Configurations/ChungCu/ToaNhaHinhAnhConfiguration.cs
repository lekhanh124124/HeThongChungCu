using HeThongChungCu.Domain.Entities.ChungCu;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.ChungCu;

public class ToaNhaHinhAnhConfiguration : IEntityTypeConfiguration<ToaNhaHinhAnh>
{
    public void Configure(EntityTypeBuilder<ToaNhaHinhAnh> builder)
    {
        builder.ToTable("ToaNhaHinhAnhs");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.HinhAnhUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.MoTa)
            .HasMaxLength(255);

        builder.Property(t => t.ThuTu)
            .IsRequired();

        builder.HasOne<ToaNha>()
            .WithMany(t => t.HinhAnhs)
            .HasForeignKey(t => t.ToaNhaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
