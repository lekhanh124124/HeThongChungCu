using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class PhieuBaoTriChecklistConfiguration : IEntityTypeConfiguration<PhieuBaoTriChecklist>
{
    public void Configure(EntityTypeBuilder<PhieuBaoTriChecklist> builder)
    {
        builder.ToTable("PhieuBaoTriChecklist");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.NoiDungChecklist)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.GhiChuThucTe)
            .HasMaxLength(250);

        builder.Property(x => x.AnhMinhHoaId);

        builder.HasOne(x => x.AnhMinhHoa)
            .WithMany()
            .HasForeignKey(x => x.AnhMinhHoaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
