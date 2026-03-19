using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.ChiSoTieuThu;

public class ChiSoTieuThuConfiguration : IEntityTypeConfiguration<Domain.Entities.ChiSoTieuThu>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.ChiSoTieuThu> builder)
    {
        builder.ToTable("ChiSoTieuThus");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Thang).IsRequired();
        builder.Property(x => x.Nam).IsRequired();
        builder.Property(x => x.NgayChot).IsRequired();

        builder.HasOne<CanHo>()
            .WithMany()
            .HasForeignKey(x => x.CanHoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<HeThongChungCu.Domain.Entities.DichVu>()
            .WithMany()
            .HasForeignKey(x => x.DichVuId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
