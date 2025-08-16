using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechChallenge.Domain.Models;

namespace TechChallenge.Data.Configurations;

public sealed class GameConfig : EntityConfig<Game>
{
    protected override void Map(EntityTypeBuilder<Game> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();
        
        builder.Property(x => x.Description)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(x => x.ImageUrl)
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(x => x.Category)
            .IsRequired();

        builder.Property(x => x.ReleaseDate)
            .HasColumnType("DATE")
            .IsRequired();
    }
}