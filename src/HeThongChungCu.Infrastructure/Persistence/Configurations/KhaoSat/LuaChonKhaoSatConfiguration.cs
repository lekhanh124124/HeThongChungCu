using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class LuaChonKhaoSatConfiguration : IEntityTypeConfiguration<LuaChonKhaoSat>
{
    public void Configure(EntityTypeBuilder<LuaChonKhaoSat> builder)
    {
        builder.ToTable("LuaChonKhaoSat");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.NoiDungLuaChon)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.IsUngVienBQT)
            .IsRequired();

        builder.Property(x => x.TieuSuUngVien)
            .HasMaxLength(1000);

        builder.Property(x => x.UngVienId)
            .IsRequired(false);

        // Back navigation to CauHoiKhaoSat
        builder.HasOne(x => x.CauHoiKhaoSat)
            .WithMany(y => y.LuaChons)
            .HasForeignKey(x => x.CauHoiKhaoSatId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
