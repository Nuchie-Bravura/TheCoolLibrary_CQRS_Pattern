namespace CoolLibrary.Domain.Entities;

/// <summary>
/// Junction table for many-to-many relationship between Books and Authors
/// </summary>
public class BookAuthor
{
    /// <summary>
    /// Foreign key to Book
    /// </summary>
    public int BookId { get; set; }
    
    /// <summary>
    /// Foreign key to Author
    /// </summary>
    public int AuthorId { get; set; }
    
    /// <summary>
    /// Order of this author for the book (for co-authors)
    /// Lower numbers indicate higher priority/primary authors
    /// </summary>
    public int AuthorOrder { get; set; } = 1;
    
    // Navigation properties
    
    /// <summary>
    /// Navigation property to Book
    /// </summary>
    public virtual Book Book { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to Author
    /// </summary>
    public virtual Author Author { get; set; } = null!;
}