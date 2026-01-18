using CoolLibrary.Domain.Enums;

namespace CoolLibrary.Domain.Entities;

/// <summary>
/// Represents a book loan/checkout in the library system
/// This is the central table for tracking borrowed books
/// </summary>
public class Loan
{
    /// <summary>
    /// Unique identifier for the loan
    /// </summary>
    public int LoanId { get; set; }
    
    /// <summary>
    /// Foreign key to Customer
    /// </summary>
    public int CustomerId { get; set; }
    
    /// <summary>
    /// Foreign key to Book
    /// </summary>
    public int BookId { get; set; }
    
    /// <summary>
    /// Date when the book was loaned out
    /// </summary>
    public DateTime LoanDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date when the book is due to be returned
    /// </summary>
    public DateTime DueDate { get; set; }
    
    /// <summary>
    /// Date when the book was actually returned (null if not returned)
    /// </summary>
    public DateTime? ReturnDate { get; set; }
    
    /// <summary>
    /// Current status of the loan
    /// </summary>
    public LoanStatus Status { get; set; } = LoanStatus.Active;
    
    /// <summary>
    /// Number of times this loan has been renewed
    /// </summary>
    public int RenewalCount { get; set; } = 0;
    
    /// <summary>
    /// Late fee amount calculated based on overdue days
    /// </summary>
    public decimal LateFee { get; set; } = 0;
    
    /// <summary>
    /// Date when the loan record was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date when the loan record was last updated
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
    /// One-to-many relationship with Fines
    /// </summary>
    public virtual ICollection<Fine> Fines { get; set; } = new List<Fine>();
    
    /// <summary>
    /// Indicates if the loan is currently overdue
    /// </summary>
    public bool IsOverdue => ReturnDate == null && DateTime.UtcNow > DueDate;
    
    /// <summary>
    /// Calculates the number of days overdue (0 if not overdue)
    /// </summary>
    public int DaysOverdue => IsOverdue ? (DateTime.UtcNow - DueDate).Days : 0;
    
    /// <summary>
    /// Constructor to set default due date (14 days from loan date)
    /// </summary>
    public Loan()
    {
        DueDate = LoanDate.AddDays(14);
    }
}