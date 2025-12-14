using CoolLibrary.Application.GraphQL.Abstractions;
using CoolLibrary.Domain.Entities;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace CoolLibrary.Application.GraphQL.Queries;

/// <summary>
/// GraphQL Queries for Genre entity
/// Provides methods to query genres with their books
/// </summary>
[ExtendObjectType("Query")]
public class GenreQueries
{
    /// <summary>
    /// Get all genres with optional filtering, sorting, and projection
    /// Example query:
    /// {
    ///   genres {
    ///     name
    ///     description
    ///     bookGenres {
    ///       book {
    ///         title
    ///         bookAuthors {
    ///           author { firstName lastName }
    ///         }
    ///       }
    ///     }
    ///   }
    /// }
    /// </summary>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Genre> GetGenres([Service] IQueryableProvider provider)
    {
        return provider.Genres
            .Include(g => g.BookGenres)
                .ThenInclude(bg => bg.Book)
                    .ThenInclude(b => b.BookAuthors)
                        .ThenInclude(ba => ba.Author);
    }

    /// <summary>
    /// Get a specific genre by ID
    /// Example query:
    /// {
    ///   genreById(id: 1) {
    ///     name
    ///     description
    ///     bookGenres {
    ///       book { title }
    ///     }
    ///   }
    /// }
    /// </summary>
    [UseProjection]
    [UseFirstOrDefault]
    public IQueryable<Genre> GetGenreById(
        [Service] IQueryableProvider provider, 
        int id)
    {
        return provider.Genres
            .Where(g => g.GenreId == id)
            .Include(g => g.BookGenres)
                .ThenInclude(bg => bg.Book)
                    .ThenInclude(b => b.BookAuthors)
                        .ThenInclude(ba => ba.Author);
    }
}
