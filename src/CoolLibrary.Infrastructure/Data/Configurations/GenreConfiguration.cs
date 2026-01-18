using CoolLibrary.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoolLibrary.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Genre entity
/// </summary>
public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        // Primary key
        builder.HasKey(g => g.GenreId);
        
        // Properties
        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(g => g.Description)
            .HasMaxLength(500);
            
        builder.Property(g => g.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(g => g.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
        
        // Indexes
        builder.HasIndex(g => g.Name)
            .IsUnique()
            .HasDatabaseName("IX_Genres_Name");
    }
}