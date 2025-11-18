using Asp.Versioning;
using CoolLibrary.Application.DTO.Book;
using CoolLibrary.Application.Services.Books;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;


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
    private readonly CreateBookService _createBookService;
    private readonly GetAllBooksService _getAllBooksService;    
    private readonly DeleteBookService _deleteBookService;

    public BooksController(  CreateBookService createBookService, GetAllBooksService getAllBooksService, DeleteBookService deleteBookService)
    {
        _getAllBooksService = getAllBooksService;
        _deleteBookService = deleteBookService;
        _createBookService = createBookService;
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
        var books = await _getAllBooksService.ExecuteAsync();
        return Ok(books);
    }

    /// <summary>
    /// Gets a specific book by ID
    /// </summary>
    /// <param name="id">The book ID</param>
    /// <returns>Book details</returns>
    /// <response code="200">Returns the book successfully</response>
    /// <response code="404">Book not found</response>
    /// <response code="500">Internal server error occurred</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BookDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetById(int bookID)
    {
         var book = await _getAllBooksService.GetByIdAsync(bookID);
         return Ok(book);
    }


    /// <summary>
    /// Allows Admin User to Create New Book , Author(s) need to exists
    /// </summary>
    /// <remarks>
    /// Returns HTTP link to created Book. hateOAS can be used to get the created book details.
    /// 
    /// 
    /// Response Sample:
    /// 
    ///     POST /api/books/AddNewBook
    ///     [
    ///         {
    ///             "Title": "New Book Title",
    ///             "ISBN": "978-3-16-148410-0",
    ///             "Description": "",
    ///             "PageCount": 300,
    ///             "TotalCopies": 5,
    ///             "AvailableCopies": 5,
    ///             "CoverPhotoURL": "",
    ///             "Language": "English",
    ///             "Publisher": "JC",
    ///             "PublicationDate": "2023-01-01", 
    ///             "Authors": [ Id:3]
    ///         }
    ///     ]
    ///
    /// </remarks>
    /// <returns>HTTP Link</returns>
    /// <response code="200">Returns the http link of recently created book</response>
    /// <response code="500">Internal server error occurred</response>
    /// </remarks>
    /// <returns>HTTP Link</returns>
    /// <response code="200">Returns the http link of recently created book<response>
    /// <response code="500">Internal server error occurred</response>

    [HttpPost("AddNewBook")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BookDTO>> CreateNewBookEntry([FromForm] CreateBookRequestDTO createBookRequestDTO)
    {
        var createdBook = await _createBookService.ExecuteAsync(createBookRequestDTO);
        return CreatedAtAction(nameof(GetById), new { id = createdBook.BookId, version = "1.0" }, createdBook);
    }


}
