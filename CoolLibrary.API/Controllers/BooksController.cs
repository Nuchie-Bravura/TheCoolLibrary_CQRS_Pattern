using Asp.Versioning;
using AutoMapper;
using CoolLibrary.Application.DTO;
using CoolLibrary.Domain.Contracts;
using CoolLibrary.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    private readonly IAuthors _authorsRepository;

    public BooksController(IBooks booksRepository, ILogger<BooksController> logger, IMapper mapper, IAuthors authorsRepository)
    {
        _booksRepository = booksRepository;
        _logger = logger;
        _mapper = mapper;
        _authorsRepository = authorsRepository;
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

    /// <summary>
    /// Gets a specific book by ID
    /// </summary>
    /// <param name="id">The book's unique identifier</param>
    /// <returns>Book details</returns>
    /// <response code="200">Returns the book successfully</response>
    /// <response code="404">Book not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BookDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDTO>> GetById(int id)
    {
        try
        {
            var book = await _booksRepository.GetByIdAsync(id);
            
            if (book == null)
            {
                return NotFound(new { message = $"Book with ID {id} not found" });
            }

            var bookDTO = _mapper.Map<BookDTO>(book);
            return Ok(bookDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving book {BookId}", id);
            return StatusCode(500, "An error occurred while retrieving the book");
        }
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
    public async Task<ActionResult<BookDTO>> CreateNewBookEntry([FromBody] CreateBookRequestDTO createBookRequestDTO)
    {
        // ✅ Validate ISBN uniqueness
        var existingBookByISBN = await _booksRepository.GetByISBNAsync(createBookRequestDTO.ISBN);
        if (existingBookByISBN != null)
        {
            return Conflict(new 
            { 
                message = $"A book with ISBN '{createBookRequestDTO.ISBN}' already exists",
                existingBookId = existingBookByISBN.BookId,
                existingBookTitle = existingBookByISBN.Title
            });
        }

        // Validate that all authors exist
        if (createBookRequestDTO.Authors == null || !createBookRequestDTO.Authors.Any())
        {
            return BadRequest(new { message = "At least one author is required" });
        }

        foreach (var authorId in createBookRequestDTO.Authors)
        {
            var existingAuthor = await _authorsRepository.GetByIdAsync(authorId);
            if (existingAuthor == null)
            {
                return BadRequest(new { message = $"Author with ID {authorId} does not exist" });
            }
        }

        try
        {
            // Map DTO to Book entity
            var bookEntity = _mapper.Map<CoolLibrary.Domain.Entities.Book>(createBookRequestDTO);
            
            // Create BookAuthor n-n relations
            var authorOrder = 1;
            bookEntity.BookAuthors = createBookRequestDTO.Authors.Select(authorId => new CoolLibrary.Domain.Entities.BookAuthor
            {
                AuthorId = authorId,
                Book = bookEntity,  
                AuthorOrder = authorOrder++
            }).ToList();

            // Insert book with authors
            var createdBook = await _booksRepository.InsertAsync(bookEntity);
            
            // Reload book with relations
            createdBook = await _booksRepository.GetByIdAsync(createdBook.BookId);
            
            var response = _mapper.Map<CreateBookResponseDTO>(createdBook);

            _logger.LogInformation(
                "✅ Book created successfully: ID={BookId}, Title={Title}, ISBN={ISBN}, Authors={AuthorCount}",
                createdBook!.BookId,
                createdBook.Title,
                createdBook.ISBN,
                createdBook.BookAuthors.Count);

            return CreatedAtAction(
                nameof(GetById), 
                new { id = createdBook.BookId }, 
                response
            );
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex) 
            when (ex.InnerException?.Message.Contains("IX_Books_ISBN") == true)
        {
            // ISBN duplicate constraint violation (fallback if validation missed it)
            _logger.LogWarning(ex, "Duplicate ISBN constraint violation: {ISBN}", createBookRequestDTO.ISBN);
            return Conflict(new { message = $"A book with ISBN '{createBookRequestDTO.ISBN}' already exists" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating new book");
            
            var innerMessage = ex.InnerException?.Message ?? "No inner exception";
            _logger.LogError("Inner exception: {InnerMessage}", innerMessage);
            
            return StatusCode(500, new 
            { 
                message = "An error occurred while creating the book", 
                error = ex.Message,
                innerError = innerMessage
            });
        }
    }
}
