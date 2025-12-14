using CoolLibrary.Domain.Entities;

namespace CoolLibrary.Application.GraphQL.Abstractions;

/// <summary>
/// Abstraction for providing IQueryable access to entities
/// This allows GraphQL queries in Application layer without depending on Infrastructure
/// </summary>
public interface IQueryableProvider
{
    /// <summary>
    /// Get queryable access to Authors
    /// </summary>
    IQueryable<Author> Authors { get; }
    
    /// <summary>
    /// Get queryable access to Books
    /// </summary>
    IQueryable<Book> Books { get; }
    
    /// <summary>
    /// Get queryable access to Customers
    /// </summary>
    IQueryable<Customer> Customers { get; }
    
    /// <summary>
    /// Get queryable access to Genres
    /// </summary>
    IQueryable<Genre> Genres { get; }
    
    /// <summary>
    /// Get queryable access to Loans
    /// </summary>
    IQueryable<Loan> Loans { get; }
}
