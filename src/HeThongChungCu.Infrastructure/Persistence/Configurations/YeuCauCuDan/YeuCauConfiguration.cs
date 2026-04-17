using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class YeuCauConfiguration : IEntityTypeConfiguration<YeuCau>
{
    public void Configure(EntityTypeBuilder<YeuCau> builder)
    {
        builder.ToTable("YeuCau");

        builder.HasKey(e => e.Id);

        builder.HasDiscriminator(x => x.LoaiYeuCauCuDanId)
            .HasValue<YeuCauCuTru>(LoaiYeuCauCuDan.CuTru)
            .HasValue<YeuCauPhuongTien>(LoaiYeuCauCuDan.PhuongTien)
            .HasValue<YeuCauSuaChua>(LoaiYeuCauCuDan.SuaChua)
            .HasValue<YeuCauThiCongNoiThat>(LoaiYeuCauCuDan.ThiCongNoiThat);

        builder.Property(x => x.LoaiYeuCauCuDanId)
            .HasConversion(
                v => v.Value,
                v => LoaiYeuCauCuDan.FromValue(v, null)!)
            .IsRequired();

        builder.Property(e => e.NoiDung).HasMaxLength(1000);
        builder.Property(e => e.LyDo).HasMaxLength(500);

        builder.Property(x => x.TrangThaiId)
            .HasConversion(
                v => v.Value,
                v => TrangThaiYeuCau.FromValue(v, null)!)
            .IsRequired();

        builder.HasOne<CanHo>()
            .WithMany()
            .HasForeignKey(x => x.CanHoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Common audit fields for processed status
        builder.Property(e => e.NguoiXuLyId);
        builder.Property(e => e.NgayXuLy);
    }
}
