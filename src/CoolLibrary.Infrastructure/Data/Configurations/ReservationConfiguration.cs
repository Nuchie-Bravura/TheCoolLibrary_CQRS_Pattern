using CoolLibrary.Domain.Entities;
using CoolLibrary.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoolLibrary.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Reservation entity
/// </summary>
public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        // Table configuration with check constraints
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Reservations_ExpirationDate", "[ExpirationDate] >= [ReservationDate]");
        });

        // Primary key
        builder.HasKey(r => r.ReservationId);
        
        // Properties
        builder.Property(r => r.ReservationDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(r => r.ExpirationDate)
            .IsRequired();
            
        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(ReservationStatus.Pending);
            
        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(r => r.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
        
        // Foreign key relationships
        builder.HasOne(r => r.Customer)
            .WithMany(c => c.Reservations)
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(r => r.Book)
            .WithMany(b => b.Reservations)
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Indexes
        builder.HasIndex(r => r.CustomerId)
            .HasDatabaseName("IX_Reservations_CustomerId");
            
        builder.HasIndex(r => r.BookId)
            .HasDatabaseName("IX_Reservations_BookId");
            
        builder.HasIndex(r => r.Status)
            .HasDatabaseName("IX_Reservations_Status");
            
        builder.HasIndex(r => r.ExpirationDate)
            .HasDatabaseName("IX_Reservations_ExpirationDate");
            
        builder.HasIndex(r => new { r.Status, r.ExpirationDate })
            .HasDatabaseName("IX_Reservations_Status_ExpirationDate");
        
        // Ignore computed properties
        builder.Ignore(r => r.IsExpired);
    }
}