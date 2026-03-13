using HeThongChungCu.Domain.Entities.Identity;
using HeThongChungCu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeThongChungCu.Infrastructure.Persistence.Configurations.Identity;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.IdCard)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.Dob)
            .IsRequired();

        builder.Property(u => u.GioiTinhId)
            .HasConversion(
                v => v.Value,
                v => GioiTinh.FromValue(v, null)!
            )
            .IsRequired();

        builder.Property(u => u.RoleId)
            .HasConversion(
                v => v.Value,
                v => Role.FromValue(v, null)!
            )
            .IsRequired();

        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);

        // Unique constraints
        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.PhoneNumber).IsUnique();
    }
}
