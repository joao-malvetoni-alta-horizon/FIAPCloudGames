using FCG.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Infrastructure.Persistence.Mappings;

public class UserGameLibraryConfiguration : IEntityTypeConfiguration<UserGameLibrary>
{
    public void Configure(EntityTypeBuilder<UserGameLibrary> builder)
    {
        builder.ToTable("UserGameLibrary");
        builder.HasKey(ugl => ugl.Id);
        builder.Property(ugl => ugl.Id).ValueGeneratedNever();

        builder.Property(ugl => ugl.UserId).IsRequired();
        builder.Property(ugl => ugl.GameId).IsRequired();
        builder.Property(ugl => ugl.AcquiredAt).IsRequired();
        builder.Property(ugl => ugl.PricePaid).HasPrecision(18, 2).IsRequired();
        builder.Property(ugl => ugl.CreatedAt).IsRequired();
        builder.Property(ugl => ugl.UpdatedAt);

        // Um usuário pode possuir cada jogo apenas uma vez
        builder.HasIndex(ugl => new { ugl.UserId, ugl.GameId }).IsUnique();

        builder.HasOne(ugl => ugl.User)
               .WithMany(u => u.GameLibrary)
               .HasForeignKey(ugl => ugl.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ugl => ugl.Game)
               .WithMany()
               .HasForeignKey(ugl => ugl.GameId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
