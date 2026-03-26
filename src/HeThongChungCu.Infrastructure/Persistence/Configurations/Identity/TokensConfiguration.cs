using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations;

public class TokensConfiguration : IEntityTypeConfiguration<Tokens>
{
    public void Configure(EntityTypeBuilder<Tokens> builder)
    {
        builder.ToTable("Token");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.TokenHash)
            .IsRequired()
            .HasMaxLength(150);
 
        builder.HasIndex(rt => rt.TokenHash).IsUnique();

        builder.Property(rt => rt.ReasonRevoked)
            .HasConversion(
                v => v != null ? v.Value : (int?)null,
                v => v != null ? ReasonRevoked.FromValue(v.Value, null) : null
            );

        builder.Property(rt => rt.TokenType)
            .HasConversion(
                v => v.Value,
                v => TokenType.FromValue(v, null)!
            )
            .HasDefaultValue(TokenType.RefreshToken)
            .IsRequired();

        builder.HasOne(rt => rt.TaiKhoan)
            .WithMany(a => a.Tokens)
            .HasForeignKey(rt => rt.TaiKhoanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
