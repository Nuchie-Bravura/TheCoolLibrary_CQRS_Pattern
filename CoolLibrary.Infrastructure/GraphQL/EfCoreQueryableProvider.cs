using CoolLibrary.Application.GraphQL.Abstractions;
using CoolLibrary.Domain.Entities;
using CoolLibrary.Infrastructure.Data;

namespace CoolLibrary.Infrastructure.GraphQL;

/// <summary>
/// Implementation of IQueryableProvider using Entity Framework Core DbContext
/// Provides IQueryable access to entities for GraphQL queries
/// </summary>
public class EfCoreQueryableProvider : IQueryableProvider
{
    private readonly LibraryDbContext _context;

    public EfCoreQueryableProvider(LibraryDbContext context)
    {
        _context = context;
    }

    public IQueryable<Author> Authors => _context.Authors;
    
    public IQueryable<Book> Books => _context.Books;
    
    public IQueryable<Customer> Customers => _context.Customers;
    
    public IQueryable<Genre> Genres => _context.Genres;
    
    public IQueryable<Loan> Loans => _context.Loans;
}
