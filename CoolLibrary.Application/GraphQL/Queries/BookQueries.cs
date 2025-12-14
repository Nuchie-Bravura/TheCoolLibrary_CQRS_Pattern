using CoolLibrary.Application.GraphQL.Abstractions;
using CoolLibrary.Domain.Entities;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace CoolLibrary.Application.GraphQL.Queries;

/// <summary>
/// GraphQL Queries for Book entity
/// Provides methods to query books with their authors and genres
/// </summary>
[ExtendObjectType("Query")]
public class BookQueries
{
    /// <summary>
    /// Get all books with optional filtering, sorting, and projection
    /// Example query:
    /// {
    ///   books {
    ///     title
    ///     isbn
    ///     bookAuthors {
    ///       author { firstName lastName }
    ///     }
    ///     bookGenres {
    ///       genre { name }
    ///     }
    ///   }
    /// }
    /// </summary>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Book> GetBooks([Service] IQueryableProvider provider)
    {
        return provider.Books
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .Include(b => b.BookGenres)
                .ThenInclude(bg => bg.Genre);
    }

    /// <summary>
    /// Get a specific book by ID
    /// Example query:
    /// {
    ///   bookById(id: 1) {
    ///     title
    ///     description
    ///     availableCopies
    ///     bookAuthors {
    ///       author { firstName lastName }
    ///     }
    ///     bookGenres {
    ///       genre { name description }
    ///     }
    ///   }
    /// }
    /// </summary>
    [UseProjection]
    [UseFirstOrDefault]
    public IQueryable<Book> GetBookById(
        [Service] IQueryableProvider provider, 
        int id)
    {
        return provider.Books
            .Where(b => b.BookId == id)
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .Include(b => b.BookGenres)
                .ThenInclude(bg => bg.Genre);
    }

    /// <summary>
    /// Get available books (with copies available)
    /// Example query:
    /// {
    ///   availableBooks {
    ///     title
    ///     availableCopies
    ///     totalCopies
    ///   }
    /// }
    /// </summary>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Book> GetAvailableBooks([Service] IQueryableProvider provider)
    {
        return provider.Books
            .Where(b => b.AvailableCopies > 0)
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .Include(b => b.BookGenres)
                .ThenInclude(bg => bg.Genre);
    }
}
