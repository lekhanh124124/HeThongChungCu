using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class TepYeuCauSuaChuaConfiguration : IEntityTypeConfiguration<TepYeuCauSuaChua>
{
    public void Configure(EntityTypeBuilder<TepYeuCauSuaChua> builder)
    {
        builder.HasBaseType<TepTaiLieu>();

        builder.Property(x => x.YeuCauSuaChuaId)
            .HasColumnName("YeuCauId");

        builder.HasOne(x => x.YeuCauSuaChua)
            .WithMany(y => y.TepYeuCauSuaChuas)
            .HasForeignKey(x => x.YeuCauSuaChuaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
