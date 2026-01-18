namespace CoolLibrary.Domain.Entities;

/// <summary>
/// Junction table for many-to-many relationship between Books and Genres
/// </summary>
public class BookGenre
{
    /// <summary>
    /// Foreign key to Book
    /// </summary>
    public int BookId { get; set; }
    
    /// <summary>
    /// Foreign key to Genre
    /// </summary>
    public int GenreId { get; set; }
    
    // Navigation properties
    
    /// <summary>
    /// Navigation property to Book
    /// </summary>
    public virtual Book Book { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to Genre
    /// </summary>
    public virtual Genre Genre { get; set; } = null!;
}