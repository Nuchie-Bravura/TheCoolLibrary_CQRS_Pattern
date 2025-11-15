using AutoMapper;
using CoolLibrary.Domain.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using CoolLibrary.API.DTOs;
using CoolLibrary.Domain.Entities;
using CoolLibrary.Application.Services.Authors;
using CoolLibrary.Application.DTO.Author;

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
    
    private readonly CreateAuthorService _createAuthorService;
    private readonly GetAllAuthorsService _getAllAuthorsService;
    private readonly DeleteAuthorService _deleteAuthorService;

    private readonly ILogger<AuthorsController> _logger;
    private readonly IMapper _mapper;

    public AuthorsController(CreateAuthorService createAuthorService, GetAllAuthorsService getAllAuthorsService, DeleteAuthorService deleteAuthorService, ILogger<AuthorsController> logger, IMapper mapper)
    {
        _createAuthorService = createAuthorService;
        _getAllAuthorsService = getAllAuthorsService;
        _deleteAuthorService = deleteAuthorService;
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
            var authors = await _getAllAuthorsService.ExecuteAsync();
            var authorDTOs = _mapper.Map<IEnumerable<AuthorDTO>>(authors);
            return Ok(authorDTOs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all authors with books");
            return StatusCode(500, "An error occurred while retrieving authors and their books");
        }
    }



    /// <summary>
    /// Allows Admin User to Create New Author with Photo
    /// </summary>
    /// <remarks>
    /// Returns HTTP link to created autor. HATEOAS can be used to get the created author details.
    /// 
    /// This endpoint accepts multipart/form-data:
    /// - FullName, Biography, Nationality, BirthDate as form fields
    /// - PhotoFile as file upload (optional)
    /// 
    /// </remarks>
    /// <returns>HTTP Link</returns>
    /// <response code="201">Returns the http link of recently created author</response>
    /// <response code="400">Bad request</response>
    /// <response code="500">Internal server error occurred</response>
    [HttpPost("AddNewAuthor")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddNewAuthor([FromForm] CreateAuthorRequestDTO createAuthorRequestDTO)
    {
        try
        {
            // Map DTO a entidad Author
            var authorEntity = _mapper.Map<Author>(createAuthorRequestDTO);

            // Obtener datos del archivo si existe
            Stream? photoStream = null;
            string? photoFileName = null;
            string? photoContentType = null;
            if (createAuthorRequestDTO.PhotoFile != null)
            {
                photoStream = createAuthorRequestDTO.PhotoFile.OpenReadStream();
                photoFileName = createAuthorRequestDTO.PhotoFile.FileName;
                photoContentType = createAuthorRequestDTO.PhotoFile.ContentType;
            }

            var createdAuthor = await _createAuthorService.ExecuteAsync(
                authorEntity,
                photoStream,
                photoFileName,
                photoContentType
            );
            return CreatedAtAction(
                nameof(GetAllAuthorsWithBooks),
                new { id = createdAuthor.AuthorId, version = "1.0" },
                createdAuthor
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating new author");
            return StatusCode(500, "An error occurred while creating the author" + ex.Message);
        }
    }


    /// <summary>
    /// Allows Admin User to Safely Delete an Author
    /// Deletes books and photoURL from azure storage before deleting author from database.
    /// </summary>
    /// <remarks>
    /// 
    /// 
    /// Response Sample:
    ///
    ///     Delete /api/authors/{authorId}
    /// 
    /// </remarks>
    /// <returns>HTTP Link</returns>
    /// <response code="200">Returns OK</response>
    /// <response code="500">Internal server error occurred</response>
    [HttpDelete("{authorId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAuthor(int authorId)
    {
        try
        {
            var result = await _deleteAuthorService.SafeAuthorDeleteAsync(authorId);
            if (!result)
            {
                return NotFound($"Author with ID {authorId} not found.");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting author with ID {authorId}");
            return StatusCode(500, "An error occurred while deleting the author");
        }
    }

}