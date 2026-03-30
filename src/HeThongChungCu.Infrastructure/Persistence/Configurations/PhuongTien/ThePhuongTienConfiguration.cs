using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class ThePhuongTienConfiguration : IEntityTypeConfiguration<ThePhuongTien>
{
    public void Configure(EntityTypeBuilder<ThePhuongTien> builder)
    {
        builder.ToTable("ThePhuongTien");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.MaThe)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(t => t.MaThe).IsUnique();
        
        builder.Property(x => x.TrangThaiId)
            .HasConversion(
                v => v.Value,
                v => TrangThaiThePhuongTien.FromValue(v, null)!)
            .IsRequired();

        builder.HasOne<PhuongTien>()
            .WithMany(p => p.ThePhuongTiens)
            .HasForeignKey(t => t.PhuongTienId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
