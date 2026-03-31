using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.ThongBao;

public class PhanBoThongBaoConfiguration : IEntityTypeConfiguration<PhanBoThongBao>
{
    public void Configure(EntityTypeBuilder<PhanBoThongBao> builder)
    {
        builder.ToTable("PhanBoThongBao");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ThongBaoId)
            .IsRequired();

        builder.Property(x => x.NguoiDungId)
            .IsRequired();

        builder.Property(x => x.IsRead)
            .HasDefaultValue(false);

        builder.HasIndex(x => x.NguoiDungId);
        builder.HasIndex(x => new { x.NguoiDungId, x.IsRead });
    }
}
