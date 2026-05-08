using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class KhaoSatConfiguration : IEntityTypeConfiguration<KhaoSat>
{
    public void Configure(EntityTypeBuilder<KhaoSat> builder)
    {
        builder.ToTable("KhaoSat");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TieuDe)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.MoTa)
            .HasMaxLength(1000);

        builder.Property(x => x.LoaiKhaoSatId)
            .HasConversion(v => v.Value, v => LoaiKhaoSat.FromValue(v, null)!)
            .IsRequired();

        builder.Property(x => x.TrangThaiId)
            .HasConversion(v => v.Value, v => TrangThaiKhaoSat.FromValue(v, null)!)
            .IsRequired();

        builder.Property(x => x.CoCheTinhDiemId)
            .HasConversion(v => v.Value, v => CoCheTinhDiemBauCu.FromValue(v, null)!)
            .IsRequired();

        builder.Property(x => x.NgayBatDau)
            .IsRequired();

        builder.Property(x => x.NgayKetThuc)
            .IsRequired();

        builder.Property(x => x.TyleThamGiaToiThieu)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(x => x.TyLeDongYToiThieu)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(x => x.IsAnDanh)
            .IsRequired();

        // Questions relationship
        builder.HasMany(x => x.CauHois)
            .WithOne(x => x.KhaoSat)
            .HasForeignKey(x => x.KhaoSatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.CauHois)
            .HasField("_cauHois")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Voting list relationship
        builder.HasMany(x => x.BieuQuyets)
            .WithOne(x => x.KhaoSat)
            .HasForeignKey(x => x.KhaoSatId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(x => x.BieuQuyets)
            .HasField("_bieuQuyets")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
