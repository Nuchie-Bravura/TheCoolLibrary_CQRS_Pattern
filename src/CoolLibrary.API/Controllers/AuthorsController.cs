using Asp.Versioning;
using CoolLibrary.Application.DTO.Author;
using CoolLibrary.Application.UseCases.Authors.Commands.CreateAuthor;
using CoolLibrary.Application.UseCases.Authors.Commands.DeleteAuthor;
using CoolLibrary.Application.UseCases.Authors.Queries.GetAllAuthors;
using MediatR;
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
    private readonly IMediator _mediator;

    public AuthorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("with-books")]
    public async Task<IActionResult> GetAllAuthorsWithBooks()
    {
        var authors = await _mediator.Send(new GetAllAuthorsQuery());
        return Ok(authors);
    }

    [HttpPost("AddNewAuthor")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddNewAuthor([FromForm] CreateAuthorRequestDTO createAuthorRequestDTO)
    {
        var createdAuthor = await _mediator.Send(new CreateAuthorCommand(createAuthorRequestDTO));
        return CreatedAtAction(nameof(GetAllAuthorsWithBooks), new { id = createdAuthor.AuthorId, version = "1.0" }, createdAuthor);
    }

    [HttpDelete("{authorId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAuthor(int authorId)
    {
        var result = await _mediator.Send(new DeleteAuthorCommand(authorId));
        if (!result) return NotFound($"Author with ID {authorId} not found.");
        return NoContent();
    }
}
