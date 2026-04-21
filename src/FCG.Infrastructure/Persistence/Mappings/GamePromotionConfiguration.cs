using FCG.Domain.Games.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Infrastructure.Persistence.Mappings;

public class GamePromotionConfiguration : IEntityTypeConfiguration<GamePromotion>
{
    public void Configure(EntityTypeBuilder<GamePromotion> builder)
    {
        builder.ToTable("GamePromotions");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.GameId).IsRequired();

        builder.Property(p => p.DiscountType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.DiscountValue)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.StartDate).IsRequired();
        builder.Property(p => p.EndDate).IsRequired();
        builder.Property(p => p.IsActive).IsRequired();
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt);

        builder.HasOne(p => p.Game)
            .WithMany()
            .HasForeignKey(p => p.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => new { p.GameId, p.StartDate, p.EndDate });
    }
}