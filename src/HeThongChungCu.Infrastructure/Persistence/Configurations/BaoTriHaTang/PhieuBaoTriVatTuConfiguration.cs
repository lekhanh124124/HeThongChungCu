using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class PhieuBaoTriVatTuConfiguration : IEntityTypeConfiguration<PhieuBaoTriVatTu>
{
    public void Configure(EntityTypeBuilder<PhieuBaoTriVatTu> builder)
    {
        builder.ToTable("PhieuBaoTriVatTu");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenVatTu)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.DonGia)
            .HasPrecision(18, 2);

        builder.Property(x => x.ThanhTien)
            .HasPrecision(18, 2);
    }
}
