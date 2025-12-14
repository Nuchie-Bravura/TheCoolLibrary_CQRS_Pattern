using CoolLibrary.Application.GraphQL.Abstractions;
using CoolLibrary.Domain.Entities;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CoolLibrary.Application.GraphQL.Queries;

/// <summary>
/// GraphQL Queries for Customer entity
/// Provides methods to query customers with their loans and book genres
/// </summary>
[ExtendObjectType("Query")]
public class CustomerQueries
{
    /// <summary>
    /// Get all customers with optional filtering, sorting, and projection
    /// Example query:
    /// {
    ///   customers {
    ///     user {
    ///       firstName
    ///       lastName
    ///     }
    ///     loans {
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
    public IQueryable<Customer> GetCustomers([Service] IQueryableProvider provider)
    {
        return provider.Customers
            .Include(c => c.User)
            .Include(c => c.Loans)
                .ThenInclude(l => l.Book)
                    .ThenInclude(b => b.BookGenres)
                        .ThenInclude(bg => bg.Genre);
    }

    /// <summary>
    /// Get a specific customer by ID with all loans and book genres
    /// Example query:
    /// {
    ///   customerById(id: 1) {
    ///     user {
    ///       firstName
    ///       lastName
    ///       email
    ///     }
    ///     loans {
    ///       loanDate
    ///       returnDate
    ///       status
    ///       book {
    ///         title
    ///         bookGenres {
    ///           genre {
    ///             name
    ///           }
    ///         }
    ///       }
    ///     }
    ///   }
    /// }
    /// </summary>
    [UseProjection]
    [UseFirstOrDefault]
    public IQueryable<Customer> GetCustomerById(
        [Service] IQueryableProvider provider, 
        int id)
    {
        return provider.Customers
            .Where(c => c.CustomerId == id)
            .Include(c => c.User)
            .Include(c => c.Loans)
                .ThenInclude(l => l.Book)
                    .ThenInclude(b => b.BookGenres)
                        .ThenInclude(bg => bg.Genre);
    }

    /// <summary>
    /// Get customers by city
    /// Example query:
    /// {
    ///   customersByCity(city: "New York") {
    ///     user { firstName lastName }
    ///     address
    ///     loans { book { title } }
    ///   }
    /// }
    /// </summary>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Customer> GetCustomersByCity(
        [Service] IQueryableProvider provider,
        string city)
    {
        return provider.Customers
            .Where(c => c.City == city)
            .Include(c => c.User)
            .Include(c => c.Loans)
                .ThenInclude(l => l.Book)
                    .ThenInclude(b => b.BookGenres)
                        .ThenInclude(bg => bg.Genre);
    }

    /// <summary>
    /// Get detailed customer information including loans and book genres by customer ID
    /// Example query:
    /// {
    /// 
    /// 
    ///   customerDetails(id: 5) {
    ///     customerId
    ///     user {
    ///       firstName
    ///       lastName
    ///       email
    ///     }
    ///     phone
    ///     address
    ///     city
    ///     postalCode
    ///     membershipDate
    ///     membershipStatus
    ///     maxBooksAllowed
    ///     loans {
    ///       loanId
    ///       loanDate
    ///       dueDate
    ///       returnDate
    ///       status
    ///       book {
    ///         bookId
    ///         title
    ///         description
    ///         publisher
    ///         isbn
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
    /// 

    // select all above 
    //FROM Customers c
   // INNER JOIN AspNetUsers u ON c.UserId = u.Id
    //LEFT JOIN Loans l ON c.CustomerId = l.CustomerId
    //LEFT JOIN Books b ON l.BookId = b.BookId
    //LEFT JOIN BookGenres bg ON b.BookId = bg.BookId
    //LEFT JOIN Genres g ON bg.GenreId = g.GenreId
    //WHERE c.CustomerId = 5

    [UseProjection]
    [UseFirstOrDefault]
    public IQueryable<Customer> GetCustomerDetails(
        [Service] IQueryableProvider provider,
        int id)
    {
        return provider.Customers
            .Where(c => c.CustomerId == id)
            .Include(c => c.User)
            .Include(c => c.Loans)
                .ThenInclude(l => l.Book)
                    .ThenInclude(b => b.BookGenres)
                        .ThenInclude(bg => bg.Genre);
    }
}
