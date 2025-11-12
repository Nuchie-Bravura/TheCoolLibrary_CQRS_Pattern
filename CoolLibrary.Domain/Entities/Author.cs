using CoolLibrary.Domain.Enums;

namespace CoolLibrary.Domain.Entities;

/// <summary>
/// Represents an author of books in the library system
/// </summary>
public class Author
{
    /// <summary>
    /// Unique identifier for the author
    /// </summary>
    public int AuthorId { get; set; }
    
    /// <summary>
    /// Author's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Author's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the normalized version of the full name. we will use this as unique key
    /// </summary>
    public string NormalizedFullName { get; set; } = string.Empty;

    /// <summary>
    /// Biography or description of the author
    /// </summary>
    public string? Biography { get; set; }
    
    /// <summary>
    /// Author's birth date
    /// </summary>
    public DateTime? BirthDate { get; set; }
    
    /// <summary>
    /// Author's nationality
    /// </summary>
    public string? Nationality { get; set; }

    /// <summary>
    /// URL to the author photo
    /// </summary>
    public string? PhotoURL { get; set; }


    /// <summary>
    /// Date when the author record was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date when the author record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    
    /// <summary>
    /// Many-to-many relationship with Books through BookAuthor
    /// </summary>
    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    
    /// <summary>
    /// Full name property for convenience
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    public void NormalizeName()
    {
        NormalizedFullName = $"{FirstName} {LastName}"
            .ToUpperInvariant()
            .Trim()
            .Replace("  ", " ");
    }
}