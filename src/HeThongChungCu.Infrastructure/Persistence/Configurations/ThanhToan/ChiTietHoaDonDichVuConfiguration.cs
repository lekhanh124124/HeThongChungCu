using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class ChiTietHoaDonDichVuConfiguration : IEntityTypeConfiguration<ChiTietHoaDonDichVu>
{
    public void Configure(EntityTypeBuilder<ChiTietHoaDonDichVu> builder)
    {
        builder.Property(x => x.DichVuId)
            .HasColumnName("DichVuId")
            .IsRequired();

        builder.HasOne<Domain.Entities.DichVu>()
            .WithMany()
            .HasForeignKey(x => x.DichVuId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
