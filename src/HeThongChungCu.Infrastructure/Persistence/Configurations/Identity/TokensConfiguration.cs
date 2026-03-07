using HeThongChungCu.Domain.Entities.Identity;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.Identity;

public class TokensConfiguration : IEntityTypeConfiguration<Tokens>
{
    public void Configure(EntityTypeBuilder<Tokens> builder)
    {
        builder.ToTable("Tokens");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.RefreshToken)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(rt => rt.RefreshToken).IsUnique();

        builder.Property(rt => rt.ReasonRevoked)
            .HasConversion(
                v => v != null ? v.Value : (int?)null,
                v => v != null ? ReasonRevoked.FromValue(v.Value) : null
            );

        builder.Property(rt => rt.TokenType)
            .HasConversion(
                v => v.Value,
                v => TokenType.FromValue(v)
            )
            .HasDefaultValue(TokenType.RefreshToken)
            .IsRequired();

        builder.HasOne(rt => rt.User)
            .WithMany(u => u.Tokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
