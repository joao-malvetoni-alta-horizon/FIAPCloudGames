using FCG.Domain.Users.Entities;
using FCG.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Infrastructure.Persistence.Mappings;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedNever();

        builder.Property(u => u.Name)
               .HasMaxLength(150)
               .IsRequired();

        builder.Property(u => u.Email)
               .HasColumnName("Email")
               .HasMaxLength(320)
               .IsRequired()
               .HasConversion(
                   email => email.Address,
                   raw => Email.FromStorage(raw));

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash)
               .HasMaxLength(500)
               .IsRequired();

        builder.Property(u => u.RoleId).IsRequired();
        builder.Property(u => u.IsActive).IsRequired();
        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.UpdatedAt);

        builder.HasOne(u => u.Role)
               .WithMany(r => r.Users)
               .HasForeignKey(u => u.RoleId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.GameLibrary)
               .WithOne(gl => gl.User)
               .HasForeignKey(gl => gl.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
