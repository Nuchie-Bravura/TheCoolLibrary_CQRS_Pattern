using CoolLibrary.Domain.Entities;

namespace CoolLibrary.Domain.Contracts;

/// <summary>
/// Repository contract for managing Book entities
/// </summary>
public interface IBooks
{
    /// <summary>
    /// Gets all books
    /// </summary>
    /// <returns>Collection of all books</returns>
    Task<IEnumerable<Book>> GetAllAsync();

    /// <summary>
    /// Gets a book by its unique identifier
    /// </summary>
    /// <param name="bookId">The book's unique identifier</param>
    /// <returns>The book if found, null otherwise</returns>
    Task<Book?> GetByIdAsync(int bookId);

    /// <summary>
    /// Gets books by title
    /// </summary>
    /// <param name="title">The title to search for</param>
    /// <returns>Collection of books matching the title</returns>
    Task<IEnumerable<Book>> GetByTitleAsync(string title);

    /// <summary>
    /// Gets a book by its ISBN
    /// </summary>
    /// <param name="isbn">The ISBN to search for</param>
    /// <returns>The book if found, null otherwise</returns>
    Task<Book?> GetByISBNAsync(string isbn);

    /// <summary>
    /// Gets books by author
    /// </summary>
    /// <param name="authorId">The author's unique identifier</param>
    /// <returns>Collection of books by the author</returns>
    Task<IEnumerable<Book>> GetByAuthorAsync(int authorId);

    /// <summary>
    /// Gets books by genre
    /// </summary>
    /// <param name="genreId">The genre's unique identifier</param>
    /// <returns>Collection of books in the genre</returns>
    Task<IEnumerable<Book>> GetByGenreAsync(int genreId);

    /// <summary>
    /// Gets books by language
    /// </summary>
    /// <param name="language">The language to filter by</param>
    /// <returns>Collection of books in the specified language</returns>
    Task<IEnumerable<Book>> GetByLanguageAsync(string language);

    /// <summary>
    /// Gets books by publisher
    /// </summary>
    /// <param name="publisher">The publisher to filter by</param>
    /// <returns>Collection of books by the publisher</returns>
    Task<IEnumerable<Book>> GetByPublisherAsync(string publisher);

    /// <summary>
    /// Gets available books (books with AvailableCopies > 0)
    /// </summary>
    /// <returns>Collection of available books</returns>
    Task<IEnumerable<Book>> GetAvailableBooksAsync();

    /// <summary>
    /// Inserts a new book
    /// </summary>
    /// <param name="book">The book to insert</param>
    /// <returns>The inserted book with generated ID</returns>
    Task<Book> InsertAsync(Book book);

    /// <summary>
    /// Updates an existing book completely
    /// </summary>
    /// <param name="book">The book with updated information</param>
    /// <returns>The updated book</returns>
    Task<Book> UpdateAsync(Book book);

    /// <summary>
    /// Partially updates a book (patch)
    /// </summary>
    /// <param name="bookId">The book's unique identifier</param>
    /// <param name="updates">Dictionary of property names and values to update</param>
    /// <returns>The updated book if found, null otherwise</returns>
    Task<Book?> PatchAsync(int bookId, Dictionary<string, object> updates);

    /// <summary>
    /// Deletes a book by its unique identifier
    /// </summary>
    /// <param name="bookId">The book's unique identifier</param>
    /// <returns>True if deleted successfully, false otherwise</returns>
    Task<bool> DeleteAsync(int bookId);

    /// <summary>
    /// Checks if a book exists
    /// </summary>
    /// <param name="bookId">The book's unique identifier</param>
    /// <returns>True if the book exists, false otherwise</returns>
    Task<bool> ExistsAsync(int bookId);

    /// <summary>
    /// Updates the available copies count for a book
    /// </summary>
    /// <param name="bookId">The book's unique identifier</param>
    /// <param name="availableCopies">The new available copies count</param>
    /// <returns>True if updated successfully, false otherwise</returns>
    Task<bool> UpdateAvailableCopiesAsync(int bookId, int availableCopies);

    /// <summary>
    /// Saves all pending changes
    /// </summary>
    /// <returns>The number of affected records</returns>
    Task<int> SaveChangesAsync();
}
