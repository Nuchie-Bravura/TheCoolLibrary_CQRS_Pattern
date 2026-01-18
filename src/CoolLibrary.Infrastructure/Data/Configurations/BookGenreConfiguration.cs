using CoolLibrary.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoolLibrary.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for BookGenre junction table
/// </summary>
public class BookGenreConfiguration : IEntityTypeConfiguration<BookGenre>
{
    public void Configure(EntityTypeBuilder<BookGenre> builder)
    {
        // Composite primary key
        builder.HasKey(bg => new { bg.BookId, bg.GenreId });
        
        // Foreign key relationships
        builder.HasOne(bg => bg.Book)
            .WithMany(b => b.BookGenres)
            .HasForeignKey(bg => bg.BookId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(bg => bg.Genre)
            .WithMany(g => g.BookGenres)
            .HasForeignKey(bg => bg.GenreId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indexes
        builder.HasIndex(bg => bg.BookId)
            .HasDatabaseName("IX_BookGenres_BookId");
            
        builder.HasIndex(bg => bg.GenreId)
            .HasDatabaseName("IX_BookGenres_GenreId");
    }
}