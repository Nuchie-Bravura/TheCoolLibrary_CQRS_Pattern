using CoolLibrary.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoolLibrary.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Book entity
/// </summary>
public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        // Primary key
        builder.HasKey(b => b.BookId);
        
        // Properties
        builder.Property(b => b.ISBN)
            .IsRequired()
            .HasMaxLength(20);
            
        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(300);
            
        builder.Property(b => b.Description)
            .HasMaxLength(2000);

        builder.Property(b => b.CoverPhotoURL)
            .IsUnicode(false)
            .IsRequired(false)
            .HasDefaultValue(null);

        builder.Property(b => b.Publisher)
            .HasMaxLength(200);
            
        builder.Property(b => b.Language)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("English");
            
        builder.Property(b => b.AvailableCopies)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(b => b.TotalCopies)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(b => b.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(b => b.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
        
        // Indexes
        builder.HasIndex(b => b.ISBN)
            .IsUnique()
            .HasDatabaseName("IX_Books_ISBN");
            
        builder.HasIndex(b => b.Title)
            .HasDatabaseName("IX_Books_Title");
            
        builder.HasIndex(b => b.AvailableCopies)
            .HasDatabaseName("IX_Books_AvailableCopies");
        
        // Ignore computed properties
        builder.Ignore(b => b.IsValidCopyCount);
        builder.Ignore(b => b.IsAvailable);
        
        // Check constraints
        builder.HasCheckConstraint("CK_Books_CopyCount", "[AvailableCopies] <= [TotalCopies]");
        builder.HasCheckConstraint("CK_Books_PositiveCopies", "[AvailableCopies] >= 0 AND [TotalCopies] >= 0");
    }
}