using CoolLibrary.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoolLibrary.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for ApplicationUser entity
/// Configures the relationship between ApplicationUser and Customer
/// </summary>
public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        // FirstName configuration
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        // LastName configuration
        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        // CreatedAt and UpdatedAt
        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(u => u.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // One-to-One relationship with Customer (optional)
        builder.HasOne(u => u.Customer)
            .WithOne(c => c.User)
            .HasForeignKey<Customer>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);  // If user is deleted, customer is also deleted

        // Index on FullName for searching
        builder.HasIndex(u => new { u.LastName, u.FirstName })
            .HasDatabaseName("IX_ApplicationUser_Name");
    }
}
