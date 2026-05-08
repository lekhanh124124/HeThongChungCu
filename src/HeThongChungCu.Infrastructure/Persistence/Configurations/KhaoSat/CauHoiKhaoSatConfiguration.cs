using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class CauHoiKhaoSatConfiguration : IEntityTypeConfiguration<CauHoiKhaoSat>
{
    public void Configure(EntityTypeBuilder<CauHoiKhaoSat> builder)
    {
        builder.ToTable("CauHoiKhaoSat");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.NoiDungCauHoi)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.IsBatBuoc)
            .IsRequired();

        builder.Property(x => x.IsMultiSelect)
            .IsRequired();

        // KhaoSat back-reference
        builder.HasOne(x => x.KhaoSat)
            .WithMany(y => y.CauHois)
            .HasForeignKey(x => x.KhaoSatId)
            .OnDelete(DeleteBehavior.Cascade);

        // Choices relationship
        builder.HasMany(x => x.LuaChons)
            .WithOne(y => y.CauHoiKhaoSat)
            .HasForeignKey(x => x.CauHoiKhaoSatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.LuaChons)
            .HasField("_luaChons")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
