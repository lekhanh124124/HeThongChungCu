using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class ChiTietBieuQuyetConfiguration : IEntityTypeConfiguration<ChiTietBieuQuyet>
{
    public void Configure(EntityTypeBuilder<ChiTietBieuQuyet> builder)
    {
        builder.ToTable("ChiTietBieuQuyet");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.NoiDungTraLoiTuDo)
            .HasMaxLength(1000);

        // Foreign keys mapping
        builder.HasOne(x => x.BieuQuyetCuDan)
            .WithMany(y => y.ChiTiets)
            .HasForeignKey(x => x.BieuQuyetCuDanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.LuaChonKhaoSat)
            .WithMany()
            .HasForeignKey(x => x.LuaChonKhaoSatId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
