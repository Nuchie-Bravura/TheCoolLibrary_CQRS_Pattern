using CoolLibrary.Domain.Enums;

namespace CoolLibrary.Domain.Entities;

/// <summary>
/// Represents a fine/penalty imposed on a customer
/// </summary>
public class Fine
{
    /// <summary>
    /// Unique identifier for the fine
    /// </summary>
    public int FineId { get; set; }
    
    /// <summary>
    /// Foreign key to Loan (nullable for fines not related to specific loans)
    /// </summary>
    public int? LoanId { get; set; }
    
    /// <summary>
    /// Foreign key to Customer
    /// </summary>
    public int CustomerId { get; set; }
    
    /// <summary>
    /// Amount of the fine in currency
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Reason for the fine
    /// </summary>
    public FineReason Reason { get; set; }
    
    /// <summary>
    /// Current status of the fine
    /// </summary>
    public FineStatus Status { get; set; } = FineStatus.Pending;
    
    /// <summary>
    /// Date when the fine was issued
    /// </summary>
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date when the fine was paid (null if not paid)
    /// </summary>
    public DateTime? PaidDate { get; set; }
    
    /// <summary>
    /// Date when the fine record was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date when the fine record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    
    /// <summary>
    /// Navigation property to Loan (optional)
    /// </summary>
    public virtual Loan? Loan { get; set; }
    
    /// <summary>
    /// Navigation property to Customer
    /// </summary>
    public virtual Customer Customer { get; set; } = null!;
    
    /// <summary>
    /// Indicates if the fine is currently outstanding
    /// </summary>
    public bool IsOutstanding => Status == FineStatus.Pending;
}