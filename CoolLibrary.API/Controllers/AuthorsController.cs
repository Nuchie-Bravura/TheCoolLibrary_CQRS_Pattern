using Asp.Versioning;
using CoolLibrary.Application.DTO.Author;
using CoolLibrary.Application.Services.Authors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[Tags("✍️ Catalog - Authors")]
[Authorize(Roles = "User,Admin")]
[ApiVersion("1.0")]
public class AuthorsController : ControllerBase
{
    private readonly CreateAuthorService _createAuthorService;
    private readonly GetAllAuthorsService _getAllAuthorsService;
    private readonly DeleteAuthorService _deleteAuthorService;

    public AuthorsController(
        CreateAuthorService createAuthorService,
        GetAllAuthorsService getAllAuthorsService,
        DeleteAuthorService deleteAuthorService)
    {
        _createAuthorService = createAuthorService;
        _getAllAuthorsService = getAllAuthorsService;
        _deleteAuthorService = deleteAuthorService;
    }

    [HttpGet("with-books")]
    public async Task<IActionResult> GetAllAuthorsWithBooks()
    {
        var authors = await _getAllAuthorsService.ExecuteAsync();
        return Ok(authors);
    }

    [HttpPost("AddNewAuthor")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddNewAuthor([FromForm] CreateAuthorRequestDTO createAuthorRequestDTO)
    {
        var createdAuthor = await _createAuthorService.ExecuteAsync(createAuthorRequestDTO);
        return CreatedAtAction(nameof(GetAllAuthorsWithBooks), new { id = createdAuthor.AuthorId, version = "1.0" }, createdAuthor);
    }

    [HttpDelete("{authorId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAuthor(int authorId)
    {
        var result = await _deleteAuthorService.SafeAuthorDeleteAsync(authorId);
        if (!result) return NotFound($"Author with ID {authorId} not found.");
        return NoContent();
    }
}
