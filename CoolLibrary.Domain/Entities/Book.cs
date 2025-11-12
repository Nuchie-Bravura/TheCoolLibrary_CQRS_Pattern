namespace CoolLibrary.Domain.Entities;

/// <summary>
/// Represents a book in the library system
/// </summary>
public class Book
{
    /// <summary>
    /// Unique identifier for the book
    /// </summary>
    public int BookId { get; set; }
    
    /// <summary>
    /// International Standard Book Number
    /// </summary>
    public string ISBN { get; set; } = string.Empty;
    
    /// <summary>
    /// Title of the book
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Description or summary of the book
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// URL to the book cover photo
    /// </summary>
    public string? CoverPhotoURL { get; set; }


    /// <summary>
    /// Date when the book was published
    /// </summary>
    public DateTime? PublicationDate { get; set; }
    
    /// <summary>
    /// Publisher of the book
    /// </summary>
    public string? Publisher { get; set; }
    
    /// <summary>
    /// Number of pages in the book
    /// </summary>
    public int? PageCount { get; set; }
    
    /// <summary>
    /// Language of the book
    /// </summary>
    public string Language { get; set; } = "English";
    
    /// <summary>
    /// Number of copies currently available for loan
    /// </summary>
    public int AvailableCopies { get; set; }
    
    /// <summary>
    /// Total number of copies owned by the library
    /// </summary>
    public int TotalCopies { get; set; }
    
    /// <summary>
    /// Date when the book record was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date when the book record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    
    /// <summary>
    /// Many-to-many relationship with Authors through BookAuthor
    /// </summary>
    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    
    /// <summary>
    /// Many-to-many relationship with Genres through BookGenre
    /// </summary>
    public virtual ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
    
    /// <summary>
    /// One-to-many relationship with Loans
    /// </summary>
    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    
    /// <summary>
    /// One-to-many relationship with Reservations
    /// </summary>
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    
    /// <summary>
    /// Business rule validation: AvailableCopies cannot exceed TotalCopies
    /// </summary>
    public bool IsValidCopyCount => AvailableCopies <= TotalCopies;
    
    /// <summary>
    /// Indicates if the book is available for loan
    /// </summary>
    public bool IsAvailable => AvailableCopies > 0;
}