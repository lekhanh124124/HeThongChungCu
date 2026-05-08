using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class TraLoiPhanAnhConfiguration : IEntityTypeConfiguration<TraLoiPhanAnh>
{
    public void Configure(EntityTypeBuilder<TraLoiPhanAnh> builder)
    {
        builder.ToTable("TraLoiPhanAnh");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.NoiDung)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.IsNhanVien)
            .IsRequired();

        builder.HasOne(x => x.YeuCauPhanAnh)
            .WithMany(y => y.TraLoiPhanAnhs)
            .HasForeignKey(x => x.YeuCauPhanAnhId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
