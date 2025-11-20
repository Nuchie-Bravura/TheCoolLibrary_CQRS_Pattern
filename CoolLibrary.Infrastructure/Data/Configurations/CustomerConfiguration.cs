using CoolLibrary.Domain.Entities;
using CoolLibrary.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoolLibrary.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Customer entity
/// Configures Customer as a separate business entity linked to ApplicationUser
/// </summary>
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        // Table configuration with check constraints
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Customers_MaxBooksAllowed", "[MaxBooksAllowed] > 0");
        });

        // Primary key
        builder.HasKey(c => c.CustomerId);

        // Foreign key to ApplicationUser
        builder.Property(c => c.UserId)
            .IsRequired()
            .HasMaxLength(450);  // Standard length for AspNetUsers.Id

        // NOTE: FirstName, LastName, and Email are now in ApplicationUser
        // They are no longer configured here to avoid duplication
        
        builder.Property(c => c.Phone)
            .HasMaxLength(20);
            
        builder.Property(c => c.Address)
            .HasMaxLength(300);
            
        builder.Property(c => c.City)
            .HasMaxLength(100);
            
        builder.Property(c => c.PostalCode)
            .HasMaxLength(20);
            
        builder.Property(c => c.MembershipDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(c => c.MembershipStatus)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(MembershipStatus.Active);
            
        builder.Property(c => c.MaxBooksAllowed)
            .IsRequired()
            .HasDefaultValue(5);
            
        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(c => c.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
        
        // Indexes
        builder.HasIndex(c => c.UserId)
            .IsUnique()  // One customer per user
            .HasDatabaseName("IX_Customers_UserId");
            
        builder.HasIndex(c => c.MembershipStatus)
            .HasDatabaseName("IX_Customers_MembershipStatus");
        
        // Relationship with ApplicationUser (configured in ApplicationUserConfiguration)
        // One-to-One: Customer.User -> ApplicationUser
        builder.HasOne(c => c.User)
            .WithOne(u => u.Customer)
            .HasForeignKey<Customer>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore computed properties (not stored in database)
        builder.Ignore(c => c.FullName);
        builder.Ignore(c => c.Email);
        builder.Ignore(c => c.CurrentLoanCount);
        builder.Ignore(c => c.CanBorrowMoreBooks);
    }
}