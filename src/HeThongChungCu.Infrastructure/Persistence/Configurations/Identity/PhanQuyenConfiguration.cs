using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class PhanQuyenConfiguration : IEntityTypeConfiguration<PhanQuyen>
{
    public void Configure(EntityTypeBuilder<PhanQuyen> builder)
    {
        builder.ToTable("PhanQuyen");

        builder.HasKey(pq => pq.Id);

        builder.Property(pq => pq.RoleId)
            .HasConversion(
                v => v.Value,
                v => Role.FromValue(v, null)!
            )
            .IsRequired();

        builder.HasOne(pq => pq.TaiKhoan)
            .WithMany(a => a.PhanQuyens)
            .HasForeignKey(pq => pq.TaiKhoanId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
