using Asp.Versioning;
using CoolLibrary.Application.DTO.Authentication;
using CoolLibrary.Application.UseCases.Auth.Queries.Login;
using CoolLibrary.Application.UseCases.Auth.Commands.Register;
using CoolLibrary.Application.UseCases.Auth.Commands.RenewToken;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoolLibrary.API.Controllers;

/// <summary>
/// Authentication controller - handles user registration and login
/// These endpoints are public (do not require authentication)
/// All business logic and logging is delegated to handlers (CQRS pattern)
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[Tags("🔐 Authentication")]
[ApiVersion("1.0")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registers a new user and creates their customer profile
    /// </summary>
    /// <param name="registerDto">Registration data including personal information</param>
    /// <returns>Registration confirmation with user details</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _mediator.Send(new RegisterCommand(registerDto));
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    /// <param name="loginDto">Login credentials (email and password)</param>
    /// <returns>JWT token and user information</returns>
    /// <remarks>
    /// This is a Query operation because it only validates credentials and reads data.
    /// It doesn't modify any state in the system.
    /// </remarks>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _mediator.Send(new LoginQuery(loginDto));
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Renews an existing JWT token for an authenticated user
    /// </summary>
    /// <returns>New JWT token with updated expiration</returns>
    [HttpPost("renewToken")]
    [Authorize]
    [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RenewToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "Invalid token" });

        try
        {
            var result = await _mediator.Send(new RenewTokenCommand(userId));
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
