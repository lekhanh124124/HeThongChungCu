using HeThongChungCu.Domain.Entities.PhuongTien;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.PhuongTien;

public class ThePhuongTienConfiguration : IEntityTypeConfiguration<ThePhuongTien>
{
    public void Configure(EntityTypeBuilder<ThePhuongTien> builder)
    {
        builder.ToTable("ThePhuongTiens");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.MaThe)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne<Domain.Entities.PhuongTien.PhuongTien>()
            .WithMany(p => p.ThePhuongTiens)
            .HasForeignKey(t => t.PhuongTienId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
