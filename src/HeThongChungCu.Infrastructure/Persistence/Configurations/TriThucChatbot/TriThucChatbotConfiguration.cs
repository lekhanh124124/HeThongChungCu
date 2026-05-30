using HeThongChungCu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class TriThucChatbotConfiguration : IEntityTypeConfiguration<TriThucChatbot>
{
    public void Configure(EntityTypeBuilder<TriThucChatbot> builder)
    {
        builder.ToTable("TriThucChatbot");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TieuDe)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.NoiDung)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(x => x.DanhMuc)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ThuTuHienThi)
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(false)
            .IsRequired();

        // ─── Sync Fields ─────────────────────────────────────────────────
        builder.Property(x => x.IsSynced)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.LastSyncedAt);

        // ─── Audit Fields ───────────────────────────────────────────────
        builder.Property(x => x.CreatedBy)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.ModifiedBy);

        builder.Property(x => x.ModifiedAt);

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.DeletedAt);

        // ─── Indexes ────────────────────────────────────────────────────
        builder.HasIndex(x => x.DanhMuc);
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.IsSynced); // nhanh khi query bản ghi chưa sync
        builder.HasIndex(x => new { x.DanhMuc, x.ThuTuHienThi });
        builder.HasIndex(x => new { x.IsSynced, x.IsDeleted }); // sync job query
    }
}
