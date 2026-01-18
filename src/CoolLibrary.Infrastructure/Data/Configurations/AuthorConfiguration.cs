using CoolLibrary.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoolLibrary.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Author entity
/// </summary>
public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        // Primary key
        builder.HasKey(a => a.AuthorId);
        
        // Properties
        builder.Property(a => a.FirstName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(a => a.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.NormalizedFullName)
            .IsRequired()
            .HasMaxLength(201); 


        builder.Property(a => a.Biography)
            .HasMaxLength(2000);
            
        builder.Property(a => a.Nationality)
            .HasMaxLength(100);

        builder.Property(a => a.PhotoURL)
            .IsUnicode(false)
            .IsRequired(false)
            .HasDefaultValue(null);


        builder.Property(a => a.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(a => a.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
        
        // Indexes
        builder.HasIndex(a => new { a.LastName, a.FirstName })
            .HasDatabaseName("IX_Authors_Name");
        
        // Ignore computed properties
        builder.Ignore(a => a.FullName);
    }
}