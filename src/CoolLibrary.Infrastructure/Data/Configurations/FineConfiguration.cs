using CoolLibrary.Domain.Entities;
using CoolLibrary.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoolLibrary.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Fine entity
/// </summary>
public class FineConfiguration : IEntityTypeConfiguration<Fine>
{
    public void Configure(EntityTypeBuilder<Fine> builder)
    {
        // Table configuration with check constraints
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Fines_Amount", "[Amount] > 0");
            t.HasCheckConstraint("CK_Fines_PaidDate", "[PaidDate] IS NULL OR [PaidDate] >= [IssueDate]");
        });

        // Primary key
        builder.HasKey(f => f.FineId);
        
        // Properties
        builder.Property(f => f.Amount)
            .IsRequired()
            .HasColumnType("decimal(8,2)");
            
        builder.Property(f => f.Reason)
            .IsRequired()
            .HasConversion<int>();
            
        builder.Property(f => f.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(FineStatus.Pending);
            
        builder.Property(f => f.IssueDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(f => f.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(f => f.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
        
        // Foreign key relationships
        builder.HasOne(f => f.Loan)
            .WithMany(l => l.Fines)
            .HasForeignKey(f => f.LoanId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(f => f.Customer)
            .WithMany(c => c.Fines)
            .HasForeignKey(f => f.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Indexes
        builder.HasIndex(f => f.LoanId)
            .HasDatabaseName("IX_Fines_LoanId");
            
        builder.HasIndex(f => f.CustomerId)
            .HasDatabaseName("IX_Fines_CustomerId");
            
        builder.HasIndex(f => f.Status)
            .HasDatabaseName("IX_Fines_Status");
            
        builder.HasIndex(f => f.Reason)
            .HasDatabaseName("IX_Fines_Reason");
            
        builder.HasIndex(f => new { f.Status, f.CustomerId })
            .HasDatabaseName("IX_Fines_Status_CustomerId");
        
        // Ignore computed properties
        builder.Ignore(f => f.IsOutstanding);
    }
}