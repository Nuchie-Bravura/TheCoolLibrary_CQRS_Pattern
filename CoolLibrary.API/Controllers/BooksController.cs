using AutoMapper;
using CoolLibrary.Application.DTO;
using CoolLibrary.Domain.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;

namespace CoolLibrary.API.Controllers;

/// <summary>
/// Books catalog management
/// Accessible to authenticated users with User or Admin role
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]  // ← Versioned route
[Produces("application/json")]
[Tags("📚 Catalog - Books")]
[Authorize(Roles = "User,Admin")]  // ← Both User and Admin can access
[ApiVersion("1.0")]  // ← This controller belongs to API v1.0
public class BooksController : ControllerBase
{
    private readonly IBooks _booksRepository;
    private readonly ILogger<BooksController> _logger;
    private readonly IMapper _mapper;

    public BooksController(IBooks booksRepository, ILogger<BooksController> logger, IMapper mapper)
    {
        _booksRepository = booksRepository;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets all books from the library catalog
    /// </summary>
    /// <remarks>
    /// Returns all books available in the library system including their availability status.
    /// 
    /// Response Sample:
    /// 
    ///     GET /api/books
    ///     [
    ///         {
    ///             "bookId": 1,
    ///             "title": "Clean Code",
    ///             "isbn": "978-0132350884",
    ///             "publishedDate": "2008-08-01",
    ///             "availableCopies": 5,
    ///             "totalCopies": 10
    ///         }
    ///     ]
    /// 
    /// </remarks>
    /// <returns>List of all books in the catalog</returns>
    /// <response code="200">Returns the list of books successfully</response>
    /// <response code="500">Internal server error occurred</response>
    [HttpGet("ListBooks")]
    [ProducesResponseType(typeof(IEnumerable<BookDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<BookDTO>>> GetAll()
    {
        try
        {
            var books = await _booksRepository.GetAllAsync();
            var bookDTOs = _mapper.Map<IEnumerable<BookDTO>>(books);
            return Ok(bookDTOs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all books");
            return StatusCode(500, "An error occurred while retrieving books");
        }
    }
}
