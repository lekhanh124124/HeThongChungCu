using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class BieuQuyetCuDanConfiguration : IEntityTypeConfiguration<BieuQuyetCuDan>
{
    public void Configure(EntityTypeBuilder<BieuQuyetCuDan> builder)
    {
        builder.ToTable("BieuQuyetCuDan");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TrongSoBieuQuyet)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.IsOtpVerified)
            .IsRequired();

        // Foreign keys mapping
        builder.HasOne(x => x.KhaoSat)
            .WithMany(y => y.BieuQuyets)
            .HasForeignKey(x => x.KhaoSatId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<CanHo>()
            .WithMany()
            .HasForeignKey(x => x.CanHoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Vote details relationship
        builder.HasMany(x => x.ChiTiets)
            .WithOne(y => y.BieuQuyetCuDan)
            .HasForeignKey(x => x.BieuQuyetCuDanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.ChiTiets)
            .HasField("_chiTiets")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
