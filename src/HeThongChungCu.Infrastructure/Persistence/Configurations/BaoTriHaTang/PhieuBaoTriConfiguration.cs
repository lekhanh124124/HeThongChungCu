using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class PhieuBaoTriConfiguration : IEntityTypeConfiguration<PhieuBaoTri>
{
    public void Configure(EntityTypeBuilder<PhieuBaoTri> builder)
    {
        builder.ToTable("PhieuBaoTri");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.MaPhieu)
            .HasMaxLength(50)
            .IsRequired();
        builder.HasIndex(x => x.MaPhieu).IsUnique();

        builder.Property(x => x.TrangThaiPhieuBaoTriId)
            .HasConversion(
                v => v.Value,
                v => TrangThaiPhieuBaoTri.FromValue(v, null)!)
            .IsRequired();
        builder.HasIndex(x => x.TrangThaiPhieuBaoTriId);

        builder.Property(x => x.ChiPhiThucTe)
            .HasPrecision(18, 2);

        builder.Property(x => x.GhiChuXuLy)
            .HasMaxLength(2000);

        builder.Property(x => x.LyDoHuy)
            .HasMaxLength(500);

        builder.HasOne<ThietBi>()
            .WithMany()
            .HasForeignKey(x => x.ThietBiId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<HangMucBaoTri>()
            .WithMany()
            .HasForeignKey(x => x.HangMucBaoTriId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<LichBaoTri>()
            .WithMany()
            .HasForeignKey(x => x.LichBaoTriId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<HopDongDoiTac>()
            .WithMany()
            .HasForeignKey(x => x.HopDongDoiTacId)
            .OnDelete(DeleteBehavior.Restrict);

        // One-to-many child checklists
        builder.HasMany(x => x.Checklists)
            .WithOne()
            .HasForeignKey(x => x.PhieuBaoTriId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Checklists)
            .HasField("_checklists")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // One-to-many child materials
        builder.HasMany(x => x.VatTus)
            .WithOne()
            .HasForeignKey(x => x.PhieuBaoTriId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.VatTus)
            .HasField("_vatTus")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // One-to-many child staff
        builder.HasMany(x => x.NhanSuBaoTris)
            .WithOne()
            .HasForeignKey(x => x.PhieuBaoTriId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.NhanSuBaoTris)
            .HasField("_nhanSuBaoTris")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => x.NgayDuKien);
    }
}
