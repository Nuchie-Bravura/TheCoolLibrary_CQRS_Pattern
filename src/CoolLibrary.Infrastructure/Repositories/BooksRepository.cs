using CoolLibrary.Domain.Contracts;
using CoolLibrary.Domain.Entities;
using CoolLibrary.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CoolLibrary.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing Book entities
/// </summary>
public class BooksRepository : IBooks
{
    private readonly LibraryDbContext _context;

    public BooksRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _context.Books
            .Include(b => b.BookAuthors)
            .ThenInclude(ba => ba.Author)
            .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(int bookId)
    {
        return await _context.Books
            .Include(b => b.BookAuthors)
            .ThenInclude(ba => ba.Author)
            .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .Include(b => b.Loans)
            .Include(b => b.Reservations)
            .FirstOrDefaultAsync(b => b.BookId == bookId);
    }

    public async Task<IEnumerable<Book>> GetByTitleAsync(string title)
    {
        return await _context.Books
            .Where(b => b.Title.Contains(title))
            .Include(b => b.BookAuthors)
            .ThenInclude(ba => ba.Author)
            .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .ToListAsync();
    }

    public async Task<Book?> GetByISBNAsync(string isbn)
    {
        return await _context.Books
            .Include(b => b.BookAuthors)
            .ThenInclude(ba => ba.Author)
            .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .FirstOrDefaultAsync(b => b.ISBN == isbn);
    }


    /**
     * SELECT 
         b.[BookId], b.[ISBN], b.[Title], b.[Description], 
         b.[Publisher], b.[PageCount], b.[Language],
         b.[AvailableCopies], b.[TotalCopies], b.[CreatedAt], b.[UpdatedAt],
         ba.[BookId], ba.[AuthorId], ba.[AuthorOrder],
         a.[AuthorId], a.[Biography], a.[Nationality], a.[CreatedAt], a.[UpdatedAt],
         bg.[BookId], bg.[GenreId],
         g.[GenreId], g.[Description], g.[CreatedAt], g.[UpdatedAt]
         FROM [Books] AS b
         INNER JOIN [BookAuthors] AS ba ON b.[BookId] = ba.[BookId]
         INNER JOIN [Authors] AS a ON ba.[AuthorId] = a.[AuthorId]
         LEFT JOIN [BookGenres] AS bg ON b.[BookId] = bg.[BookId]
         LEFT JOIN [Genres] AS g ON bg.[GenreId] = g.[GenreId]WHERE EXISTS (
            SELECT 1
            FROM [BookAuthors] AS ba2
            WHERE ba2.[BookId] = b.[BookId] 
              AND ba2.[AuthorId] = @authorId
        )
        ORDER BY b.[BookId], ba.[AuthorId], bg.[GenreId] 
    */
    public async Task<IEnumerable<Book>> GetByAuthorAsync(int authorId)
    {
        return await _context.Books
            .Include(b => b.BookAuthors)
            .ThenInclude(ba => ba.Author)
            .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .Where(b => b.BookAuthors.Any(ba => ba.AuthorId == authorId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetByGenreAsync(int genreId)
    {
        return await _context.Books
            .Include(b => b.BookAuthors)
            .ThenInclude(ba => ba.Author)
            .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .Where(b => b.BookGenres.Any(bg => bg.GenreId == genreId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetByLanguageAsync(string language)
    {
        return await _context.Books
            .Where(b => b.Language == language)
            .Include(b => b.BookAuthors)
            .ThenInclude(ba => ba.Author)
            .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetByPublisherAsync(string publisher)
    {
        return await _context.Books
            .Where(b => b.Publisher != null && b.Publisher.Contains(publisher))
            .Include(b => b.BookAuthors)
            .ThenInclude(ba => ba.Author)
            .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetAvailableBooksAsync()
    {
        return await _context.Books
            .Where(b => b.AvailableCopies > 0)
            .Include(b => b.BookAuthors)
            .ThenInclude(ba => ba.Author)
            .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .ToListAsync();
    }

    public async Task<Book> InsertAsync(Book book)
    {
        book.CreatedAt = DateTime.UtcNow;
        book.UpdatedAt = DateTime.UtcNow;
        
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();
        
        return book;
    }

    public async Task<Book> UpdateAsync(Book book)
    {
        book.UpdatedAt = DateTime.UtcNow;
        
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
        
        return book;
    }

    public async Task<Book?> PatchAsync(int bookId, Dictionary<string, object> updates)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book == null)
            return null;

        foreach (var update in updates)
        {
            var property = typeof(Book).GetProperty(update.Key);
            if (property != null && property.CanWrite)
            {
                property.SetValue(book, Convert.ChangeType(update.Value, property.PropertyType));
            }
        }

        book.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        
        return book;
    }

    public async Task<bool> DeleteAsync(int bookId)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book == null)
            return false;

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> ExistsAsync(int bookId)
    {
        return await _context.Books.AnyAsync(b => b.BookId == bookId);
    }

    public async Task<bool> UpdateAvailableCopiesAsync(int bookId, int availableCopies)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book == null)
            return false;

        book.AvailableCopies = availableCopies;
        book.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
