using CoolLibrary.Domain.Enums;

namespace CoolLibrary.Domain.Entities;

/// <summary>
/// Represents a library customer/member
/// Links to ApplicationUser (Identity) via UserId foreign key
/// Separation: ApplicationUser handles authentication, Customer handles library business logic
/// </summary>
public class Customer
{
    /// <summary>
    /// Unique identifier for the customer
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Foreign key to ApplicationUser (AspNetUsers table)
    /// Links this customer to their authentication account
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to ApplicationUser
    /// Use this to access user's email, name, roles, etc.
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;

    // NOTE: FirstName, LastName, and Email are now in ApplicationUser
    // Access them via: customer.User.FirstName, customer.User.LastName, customer.User.Email

    /// <summary>
    /// Customer's phone number (library-specific contact)
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// Customer's street address
    /// </summary>
    public string? Address { get; set; }
    
    /// <summary>
    /// Customer's city
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// Customer's postal code
    /// </summary>
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Date when customer became a library member
    /// </summary>
    public DateTime MembershipDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Current membership status
    /// </summary>
    public MembershipStatus MembershipStatus { get; set; } = MembershipStatus.Active;
    
    /// <summary>
    /// Maximum number of books this customer can borrow simultaneously
    /// </summary>
    public int MaxBooksAllowed { get; set; } = 5;
    
    /// <summary>
    /// Date when the customer record was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date when the customer record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    
    /// <summary>
    /// One-to-many relationship with Loans
    /// </summary>
    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    
    /// <summary>
    /// One-to-many relationship with Reservations
    /// </summary>
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    
    /// <summary>
    /// One-to-many relationship with Fines
    /// </summary>
    public virtual ICollection<Fine> Fines { get; set; } = new List<Fine>();
    
    // Computed properties
    
    /// <summary>
    /// Full name property (delegates to ApplicationUser)
    /// </summary>
    public string FullName => User?.FullName ?? string.Empty;

    /// <summary>
    /// Email property (delegates to ApplicationUser)
    /// </summary>
    public string Email => User?.Email ?? string.Empty;
    
    /// <summary>
    /// Current number of active loans
    /// </summary>
    public int CurrentLoanCount => Loans?.Count(l => l.Status == LoanStatus.Active || l.Status == LoanStatus.Overdue) ?? 0;
    
    /// <summary>
    /// Indicates if customer can borrow more books
    /// </summary>
    public bool CanBorrowMoreBooks => CurrentLoanCount < MaxBooksAllowed && MembershipStatus == MembershipStatus.Active;
}