using CoolLibrary.Application.GraphQL.Abstractions;
using CoolLibrary.Domain.Entities;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace CoolLibrary.Application.GraphQL.Queries;

/// <summary>
/// GraphQL Queries for Author entity
/// Provides methods to query authors with their books and genres
/// </summary>
[ExtendObjectType("Query")]
public class AuthorQueries
{
    /// <summary>
    /// Get all authors with optional filtering, sorting, and projection
    /// Example query:
    /// {
    ///   authors {
    ///     firstName
    ///     lastName
    ///     bookAuthors {
    ///       book {
    ///         title
    ///         bookGenres {
    ///           genre { name }
    ///         }
    ///       }
    ///     }
    ///   }
    /// }
    /// </summary>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Author> GetAuthors([Service] IQueryableProvider provider)
    {
        return provider.Authors
            .Include(a => a.BookAuthors)
                .ThenInclude(ba => ba.Book)
                    .ThenInclude(b => b.BookGenres)
                        .ThenInclude(bg => bg.Genre);
    }

    /// <summary>
    /// Get a specific author by ID with all related data
    /// Example query:
    /// {
    ///   authorById(id: 1) {
    ///     firstName
    ///     lastName
    ///     biography
    ///     bookAuthors {
    ///       book {
    ///         title
    ///         bookGenres {
    ///           genre {
    ///             name
    ///             description
    ///           }
    ///         }
    ///       }
    ///     }
    ///   }
    /// }
    /// </summary>
    [UseProjection]
    [UseFirstOrDefault]
    public IQueryable<Author> GetAuthorById(
        [Service] IQueryableProvider provider, 
        int id)
    {
        return provider.Authors
            .Where(a => a.AuthorId == id)
            .Include(a => a.BookAuthors)
                .ThenInclude(ba => ba.Book)
                    .ThenInclude(b => b.BookGenres)
                        .ThenInclude(bg => bg.Genre);
    }

    /// <summary>
    /// Get authors by nationality
    /// Example query:
    /// {
    ///   authorsByNationality(nationality: "British") {
    ///     firstName
    ///     lastName
    ///     bookAuthors {
    ///       book { title }
    ///     }
    ///   }
    /// }
    /// </summary>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Author> GetAuthorsByNationality(
        [Service] IQueryableProvider provider,
        string nationality)
    {
        return provider.Authors
            .Where(a => a.Nationality == nationality)
            .Include(a => a.BookAuthors)
                .ThenInclude(ba => ba.Book)
                    .ThenInclude(b => b.BookGenres)
                        .ThenInclude(bg => bg.Genre);
    }
}
