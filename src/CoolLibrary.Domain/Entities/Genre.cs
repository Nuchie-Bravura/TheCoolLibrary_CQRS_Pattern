namespace CoolLibrary.Domain.Entities;

/// <summary>
/// Represents a genre/category for books in the library system
/// </summary>
public class Genre
{
    /// <summary>
    /// Unique identifier for the genre
    /// </summary>
    public int GenreId { get; set; }
    
    /// <summary>
    /// Name of the genre
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the genre
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Date when the genre record was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date when the genre record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    
    /// <summary>
    /// Many-to-many relationship with Books through BookGenre
    /// </summary>
    public virtual ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
}