namespace CoolLibrary.Domain.Enums;

/// <summary>
/// Represents the status of a book reservation
/// </summary>
public enum ReservationStatus
{
    /// <summary>
    /// Reservation is pending and book is not available
    /// </summary>
    Pending = 1,
    
    /// <summary>
    /// Reservation has been fulfilled and book is available for pickup
    /// </summary>
    Fulfilled = 2,
    
    /// <summary>
    /// Reservation has been cancelled by customer or system
    /// </summary>
    Cancelled = 3,
    
    /// <summary>
    /// Reservation has expired (customer didn't pick up the book)
    /// </summary>
    Expired = 4
}