using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.ThongBao;

public class ThongBaoConfiguration : IEntityTypeConfiguration<HeThongChungCu.Domain.Entities.ThongBao>
{
    public void Configure(EntityTypeBuilder<HeThongChungCu.Domain.Entities.ThongBao> builder)
    {
        builder.ToTable("ThongBao");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TieuDe)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.NoiDung)
            .IsRequired();

        builder.Property(x => x.LoaiThongBao)
            .HasConversion(
                v => v.Value,
                v => HeThongChungCu.Domain.Enums.LoaiThongBao.FromValue(v, null)!)
            .IsRequired();

        builder.Property(x => x.ReferenceId)
            .HasMaxLength(100);

        builder.Property(x => x.Metadata)
            .IsUnicode(false);

        builder.HasMany(x => x.PhanBoThongBaos)
            .WithOne()
            .HasForeignKey(x => x.ThongBaoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
