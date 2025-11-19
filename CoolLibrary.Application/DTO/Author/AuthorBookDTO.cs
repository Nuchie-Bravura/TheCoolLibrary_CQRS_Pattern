namespace CoolLibrary.Application.DTO.Author;

/// <summary>
/// Represents a book within an author's data, simplified for nesting.
/// </summary>
public class AuthorBookDTO
{
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
}
