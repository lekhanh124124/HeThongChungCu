using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.BangGia;

public class BangGiaLuyTienConfiguration : IEntityTypeConfiguration<BangGiaLuyTien>
{
    public void Configure(EntityTypeBuilder<BangGiaLuyTien> builder)
    {
        builder.ToTable("BangGiaLuyTien");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DonGia)
            .HasColumnType("decimal(18,2)");
    }
}
