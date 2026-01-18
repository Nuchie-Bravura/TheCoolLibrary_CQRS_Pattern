using CoolLibrary.Domain.Enums;

namespace CoolLibrary.Domain.Entities;

/// <summary>
/// Represents a book reservation when the book is not currently available
/// </summary>
public class Reservation
{
    /// <summary>
    /// Unique identifier for the reservation
    /// </summary>
    public int ReservationId { get; set; }
    
    /// <summary>
    /// Foreign key to Customer
    /// </summary>
    public int CustomerId { get; set; }
    
    /// <summary>
    /// Foreign key to Book
    /// </summary>
    public int BookId { get; set; }
    
    /// <summary>
    /// Date when the reservation was made
    /// </summary>
    public DateTime ReservationDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date when the reservation expires if not fulfilled
    /// </summary>
    public DateTime ExpirationDate { get; set; }
    
    /// <summary>
    /// Current status of the reservation
    /// </summary>
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    
    /// <summary>
    /// Date when the reservation record was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date when the reservation record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    
    /// <summary>
    /// Navigation property to Customer
    /// </summary>
    public virtual Customer Customer { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to Book
    /// </summary>
    public virtual Book Book { get; set; } = null!;
    
    /// <summary>
    /// Indicates if the reservation has expired
    /// </summary>
    public bool IsExpired => DateTime.UtcNow > ExpirationDate;
    
    /// <summary>
    /// Constructor to set default expiration date (7 days from reservation date)
    /// </summary>
    public Reservation()
    {
        ExpirationDate = ReservationDate.AddDays(7);
    }
}