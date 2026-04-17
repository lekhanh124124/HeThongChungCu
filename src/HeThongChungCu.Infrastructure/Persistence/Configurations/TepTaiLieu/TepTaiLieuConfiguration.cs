using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class TepTaiLieuConfiguration : IEntityTypeConfiguration<TepTaiLieu>
{
    public void Configure(EntityTypeBuilder<TepTaiLieu> builder)
    {
        builder.ToTable("TepTaiLieu");

        builder.HasKey(x => x.Id);

        builder.HasDiscriminator(x => x.LoaiTepId)
            .HasValue<TepTaiLieu>(LoaiTepTaiLieu.MacDinh)
            .HasValue<TepTaiLieuNguoiDung>(LoaiTepTaiLieu.NguoiDung)
            .HasValue<TepYeuCauTaiLieuCuTru>(LoaiTepTaiLieu.YeuCauCuTru)
            .HasValue<TepYeuCauPhuongTien>(LoaiTepTaiLieu.YeuCauPhuongTien)
            .HasValue<TepYeuCauSuaChua>(LoaiTepTaiLieu.YeuCauSuaChua)
            .HasValue<TepYeuCauThiCongNoiThat>(LoaiTepTaiLieu.YeuCauThiCongNoiThat)
            .HasValue<TepPhuongTien>(LoaiTepTaiLieu.PhuongTien)
            .HasValue<TepHopDongDoiTac>(LoaiTepTaiLieu.HopDongDoiTac);

        builder.Property(x => x.LoaiTepId)
            .HasConversion(
                v => v.Value,
                v => LoaiTepTaiLieu.FromValue(v, null)!)
            .IsRequired();

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.FileUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Size)
            .IsRequired();

        builder.Property(x => x.IsUsed)
            .IsRequired()
            .HasDefaultValue(false);
    }
}
