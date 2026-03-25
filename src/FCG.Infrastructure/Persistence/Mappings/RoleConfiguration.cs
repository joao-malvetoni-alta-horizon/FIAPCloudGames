using FCG.Domain.Users.Entities;
using FCG.Domain.Users.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Infrastructure.Persistence.Mappings;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.Property(r => r.Name)
               .HasMaxLength(50)
               .IsRequired();

        builder.HasIndex(r => r.Name).IsUnique();

        builder.Property(r => r.Description).HasMaxLength(200);
        builder.Property(r => r.IsActive).IsRequired();
        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.UpdatedAt);

        // Dados de seed — Guids fixos e determinísticos
        builder.HasData(
            new
            {
                Id = RoleType.User.ToRoleId(),
                Name = RoleType.User.ToRoleName(),
                Description = "Acesso à plataforma e biblioteca de jogos.",
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = (DateTime?)null
            },
            new
            {
                Id = RoleType.Administrator.ToRoleId(),
                Name = RoleType.Administrator.ToRoleName(),
                Description = "Pode cadastrar jogos, administrar usuários e criar promoções.",
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = (DateTime?)null
            }
        );
    }
}
