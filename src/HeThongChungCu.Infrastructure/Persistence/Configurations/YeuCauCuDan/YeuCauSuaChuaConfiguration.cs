using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class YeuCauSuaChuaConfiguration : IEntityTypeConfiguration<YeuCauSuaChua>
{
    public void Configure(EntityTypeBuilder<YeuCauSuaChua> builder)
    {
        // TPH: Inherits table from YeuCau

        builder.Property(x => x.PhamViId)
            .HasConversion(v => v.Value, v => PhamViSuaChua.FromValue(v, null)!)
            .IsRequired();

        builder.Property(x => x.LoaiSuCoId)
            .HasConversion(v => v.Value, v => LoaiSuCoKyThuat.FromValue(v, null)!)
            .IsRequired();

        builder.Property(x => x.TrangThaiSuaChuaId)
            .HasConversion(v => v.Value, v => TrangThaiSuaChua.FromValue(v, null)!)
            .IsRequired();

        builder.Property(x => x.MucDoUuTienDeXuatId)
            .HasConversion(v => v.Value, v => MucDoUuTien.FromValue(v, null)!)
            .IsRequired();

        builder.Property(x => x.MucDoUuTienChotId)
            .HasConversion(
                v => v != null ? v.Value : (int?)null,
                v => v != null ? MucDoUuTien.FromValue(v.Value, null) : null);

        builder.Property(x => x.MoTaViTri)
            .HasMaxLength(500);

        builder.Property(x => x.KetQuaXuLy)
            .HasMaxLength(1000);

        builder.Property(x => x.LyDoHuy)
            .HasMaxLength(500);

        builder.Property(x => x.ChiPhiDuKien)
            .HasPrecision(18, 2);

        builder.Property(x => x.ChiPhiThucTe)
            .HasPrecision(18, 2);

        builder.Property(x => x.GhiChuBaoGia)
            .HasMaxLength(1000);

        // Contract mapping
        builder.HasOne<HopDongDoiTac>()
            .WithMany()
            .HasForeignKey(x => x.HopDongDoiTacId)
            .OnDelete(DeleteBehavior.Restrict);

        // Personnel mapping
        builder.HasMany(x => x.NhanSuSuaChuas)
            .WithOne()
            .HasForeignKey(x => x.YeuCauId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.NhanSuSuaChuas)
            .HasField("_nhanSuSuaChuas")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.TepYeuCauSuaChuas)
            .WithOne(x => x.YeuCauSuaChua)
            .HasForeignKey(x => x.YeuCauSuaChuaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.TepYeuCauSuaChuas)
            .HasField("_tepYeuCauSuaChuas")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
