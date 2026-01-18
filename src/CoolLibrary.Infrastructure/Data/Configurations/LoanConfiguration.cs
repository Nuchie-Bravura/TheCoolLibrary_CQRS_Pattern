using CoolLibrary.Domain.Entities;
using CoolLibrary.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoolLibrary.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Loan entity
/// </summary>
public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        // Table configuration with check constraints
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Loans_RenewalCount", "[RenewalCount] >= 0");
            t.HasCheckConstraint("CK_Loans_LateFee", "[LateFee] >= 0");
            t.HasCheckConstraint("CK_Loans_ReturnDate", "[ReturnDate] IS NULL OR [ReturnDate] >= [LoanDate]");
        });

        // Primary key
        builder.HasKey(l => l.LoanId);
        
        // Properties
        builder.Property(l => l.LoanDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(l => l.DueDate)
            .IsRequired();
            
        builder.Property(l => l.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(LoanStatus.Active);
            
        builder.Property(l => l.RenewalCount)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(l => l.LateFee)
            .IsRequired()
            .HasColumnType("decimal(8,2)")
            .HasDefaultValue(0);
            
        builder.Property(l => l.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(l => l.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
        
        // Foreign key relationships
        builder.HasOne(l => l.Customer)
            .WithMany(c => c.Loans)
            .HasForeignKey(l => l.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(l => l.Book)
            .WithMany(b => b.Loans)
            .HasForeignKey(l => l.BookId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Indexes
        builder.HasIndex(l => l.CustomerId)
            .HasDatabaseName("IX_Loans_CustomerId");
            
        builder.HasIndex(l => l.BookId)
            .HasDatabaseName("IX_Loans_BookId");
            
        builder.HasIndex(l => l.Status)
            .HasDatabaseName("IX_Loans_Status");
            
        builder.HasIndex(l => l.DueDate)
            .HasDatabaseName("IX_Loans_DueDate");
            
        builder.HasIndex(l => new { l.Status, l.DueDate })
            .HasDatabaseName("IX_Loans_Status_DueDate");
        
        // Ignore computed properties
        builder.Ignore(l => l.IsOverdue);
        builder.Ignore(l => l.DaysOverdue);
    }
}