using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class YeuCauThiCongNoiThatConfiguration : IEntityTypeConfiguration<YeuCauThiCongNoiThat>
{
    public void Configure(EntityTypeBuilder<YeuCauThiCongNoiThat> builder)
    {
        // TPH: Inherits table from YeuCau

        builder.Property(x => x.HangMucThiCong)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.TenDonViThiCong)
            .HasMaxLength(200);

        builder.Property(x => x.NguoiDaiDien)
            .HasMaxLength(100);

        builder.OwnsOne(e => e.SoDienThoaiDaiDien, sd =>
        {
            sd.Property(p => p.Value)
                .HasColumnName("SoDienThoaiDaiDien")
                .HasMaxLength(15);
        });

        builder.Property(x => x.TienDatCoc)
            .HasPrecision(18, 2);

        builder.Property(x => x.GhiChuThuCoc)
            .HasMaxLength(200);

        builder.Property(x => x.IsDaThuCoc)
            .IsRequired();

        builder.Property(x => x.NgayDuyetSoBo);

        builder.Property(x => x.TrangThaiThiCongId)
            .HasConversion(v => v.Value, v => TrangThaiThiCong.FromValue(v, null)!)
            .IsRequired();

        builder.HasMany(x => x.TepYeuCauThiCongNoiThats)
            .WithOne(x => x.YeuCauThiCongNoiThat)
            .HasForeignKey(x => x.YeuCauThiCongNoiThatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.TepYeuCauThiCongNoiThats)
            .HasField("_tepYeuCauThiCongNoiThats")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Personnel mapping
        builder.HasMany(x => x.NhanSuThiCongs)
            .WithOne()
            .HasForeignKey(x => x.YeuCauId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.NhanSuThiCongs)
            .HasField("_nhanSuThiCongs")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
