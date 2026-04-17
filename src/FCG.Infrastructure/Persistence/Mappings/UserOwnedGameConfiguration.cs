using FCG.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Infrastructure.Persistence.Mappings;

public class UserOwnedGameConfiguration : IEntityTypeConfiguration<UserOwnedGame>
{
    public void Configure(EntityTypeBuilder<UserOwnedGame> builder)
    {
        builder.ToTable("UserGameLibrary");
        builder.HasKey(ownedGame => ownedGame.Id);
        builder.Property(ownedGame => ownedGame.Id).ValueGeneratedNever();

        builder.Property(ownedGame => ownedGame.UserId).IsRequired();
        builder.Property(ownedGame => ownedGame.GameId).IsRequired();
        builder.Property(ownedGame => ownedGame.AcquiredAt).IsRequired();
        builder.Property(ownedGame => ownedGame.PricePaid).HasPrecision(18, 2).IsRequired();
        builder.Property(ownedGame => ownedGame.CreatedAt).IsRequired();
        builder.Property(ownedGame => ownedGame.UpdatedAt);

        // Um usuário pode possuir cada jogo apenas uma vez.
        builder.HasIndex(ownedGame => new { ownedGame.UserId, ownedGame.GameId }).IsUnique();

        builder.HasOne(ownedGame => ownedGame.User)
               .WithMany(user => user.OwnedGames)
               .HasForeignKey(ownedGame => ownedGame.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ownedGame => ownedGame.Game)
               .WithMany()
               .HasForeignKey(ownedGame => ownedGame.GameId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
