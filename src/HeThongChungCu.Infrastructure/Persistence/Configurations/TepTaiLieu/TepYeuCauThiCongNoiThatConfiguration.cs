using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class TepYeuCauThiCongNoiThatConfiguration : IEntityTypeConfiguration<TepYeuCauThiCongNoiThat>
{
    public void Configure(EntityTypeBuilder<TepYeuCauThiCongNoiThat> builder)
    {
        builder.HasBaseType<TepTaiLieu>();

        builder.Property(x => x.YeuCauThiCongNoiThatId)
            .HasColumnName("YeuCauId");

        builder.HasOne(x => x.YeuCauThiCongNoiThat)
            .WithMany(y => y.TepYeuCauThiCongNoiThats)
            .HasForeignKey(x => x.YeuCauThiCongNoiThatId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
