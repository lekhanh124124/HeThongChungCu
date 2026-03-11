using HeThongChungCu.Domain.Entities.ChungCu;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.ChungCu;

public class TangConfiguration : IEntityTypeConfiguration<Tang>
{
    public void Configure(EntityTypeBuilder<Tang> builder)
    {
        builder.ToTable("Tangs");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.MaTang)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(t => t.TenTang)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.LoaiTangId)
            .IsRequired();

        builder.HasOne(t => t.ToaNha)
            .WithMany(tn => tn.Tangs)
            .HasForeignKey(t => t.ToaNhaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.CanHos)
            .WithOne(ch => ch.Tang)
            .HasForeignKey(ch => ch.TangId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
