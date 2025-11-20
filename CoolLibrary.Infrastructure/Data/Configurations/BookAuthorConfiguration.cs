using CoolLibrary.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoolLibrary.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for BookAuthor junction table
/// </summary>
public class BookAuthorConfiguration : IEntityTypeConfiguration<BookAuthor>
{
    public void Configure(EntityTypeBuilder<BookAuthor> builder)
    {
        // Table configuration with check constraints
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_BookAuthors_AuthorOrder", "[AuthorOrder] > 0");
        });

        // Composite primary key
        builder.HasKey(ba => new { ba.BookId, ba.AuthorId });
        
        // Properties
        builder.Property(ba => ba.AuthorOrder)
            .IsRequired()
            .HasDefaultValue(1);
        
        // Foreign key relationships
        builder.HasOne(ba => ba.Book)
            .WithMany(b => b.BookAuthors)
            .HasForeignKey(ba => ba.BookId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(ba => ba.Author)
            .WithMany(a => a.BookAuthors)
            .HasForeignKey(ba => ba.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indexes
        builder.HasIndex(ba => ba.BookId)
            .HasDatabaseName("IX_BookAuthors_BookId");
            
        builder.HasIndex(ba => ba.AuthorId)
            .HasDatabaseName("IX_BookAuthors_AuthorId");
            
        builder.HasIndex(ba => new { ba.BookId, ba.AuthorOrder })
            .HasDatabaseName("IX_BookAuthors_BookId_AuthorOrder");
    }
}