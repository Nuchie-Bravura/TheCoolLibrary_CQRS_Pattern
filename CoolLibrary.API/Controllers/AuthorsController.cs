using AutoMapper;
using CoolLibrary.Application.DTO;
using CoolLibrary.Domain.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;

namespace CoolLibrary.API.Controllers;

/// <summary>
/// Authors catalog management
/// Accessible to authenticated users with User or Admin role
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]  // ← Versioned route
[Produces("application/json")]
[Tags("✍️ Catalog - Authors")]
[Authorize(Roles = "User,Admin")]  // ← Both User and Admin can access
[ApiVersion("1.0")]  // ← This controller belongs to API v1.0
public class AuthorsController : ControllerBase
{
    private readonly IAuthors _authorsRepository;
    private readonly ILogger<AuthorsController> _logger;
    private readonly IMapper _mapper;

    public AuthorsController(IAuthors authorsRepository, ILogger<AuthorsController> logger, IMapper mapper)
    {
        _authorsRepository = authorsRepository;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets all authors with their published books
    /// </summary>
    /// <remarks>
    /// Returns a complete list of authors including all books they have written.
    /// This endpoint provides detailed information about each author and their literary works.
    /// 
    /// Response Sample:
    /// 
    ///     GET /api/authors/with-books
    ///     [
    ///         {
    ///             "authorId": 1,
    ///             "name": "Robert C. Martin",
    ///             "biography": "Software engineer and author",
    ///             "books": [
    ///                 {
    ///                     "bookId": 1,
    ///                     "title": "Clean Code",
    ///                     "isbn": "978-0132350884"
    ///                 },
    ///                 {
    ///                     "bookId": 2,
    ///                     "title": "Clean Architecture",
    ///                     "isbn": "978-0134494166"
    ///                 }
    ///             ]
    ///         }
    ///     ]
    /// 
    /// </remarks>
    /// <returns>List of all authors with their books</returns>
    /// <response code="200">Returns the list of authors with their books successfully</response>
    /// <response code="500">Internal server error occurred</response>
    [HttpGet("with-books")]
    [ProducesResponseType(typeof(IEnumerable<AuthorDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AuthorDTO>>> GetAllAuthorsWithBooks()
    {
        try
        {
            var authors = await _authorsRepository.GetAllAsync();
            var authorDTOs = _mapper.Map<IEnumerable<AuthorDTO>>(authors);
            return Ok(authorDTOs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all authors with books");
            return StatusCode(500, "An error occurred while retrieving authors and their books");
        }
    }
}