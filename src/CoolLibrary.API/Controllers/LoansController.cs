using Asp.Versioning;
using CoolLibrary.Application.DTO.LoansAndReservations;
using CoolLibrary.Application.UseCases.Loans.Commands.RequestLoan;
using CoolLibrary.Application.UseCases.Loans.Queries.GetBookAvailability;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoolLibrary.API.Controllers;

/// <summary>
/// Loan operations management
/// Accessible to authenticated users with User or Admin role
/// All business logic and logging is delegated to handlers (CQRS pattern)
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[Tags("🔄 Operations - Loans")]
[Authorize(Roles = "User,Admin")]
[ApiVersion("1.0")]
public class LoansController : ControllerBase
{
    private readonly IMediator _mediator;

    public LoansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// ⚠️ DEPRECATED: Requests a loan (INSECURE - accepts CustomerId from client)
    /// Use /request-secure instead
    /// </summary>
    [HttpPost("RequestLoan")]
    [Obsolete("Use POST /request-secure instead - this endpoint is deprecated")]
    [ProducesResponseType(typeof(LoanResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<LoanResponseDTO> RequestLoan([FromBody] LoanRequestDTO request)
    {
        return BadRequest(new { message = "This endpoint is deprecated and insecure. Please use /request-secure." });
    }

    /// <summary>
    /// ✅ SECURE: Request a book loan (CustomerId from JWT)
    /// </summary>
    /// <remarks>
    /// Creates a new loan for the authenticated user.
    /// CustomerId is extracted from the JWT token (secure).
    /// </remarks>
    /// <param name="request">Book ID to borrow</param>
    /// <returns>Created loan information</returns>
    /// <response code="200">Loan created successfully</response>
    /// <response code="400">Invalid request (book unavailable, borrowing limit reached, etc.)</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Customer profile not found for authenticated user</response>
    [HttpPost("request-secure")]
    [ProducesResponseType(typeof(LoanResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanResponseDTO>> RequestLoanSecure([FromBody] CreateLoanBookOnlyDTO request)
    {
        try
        {
            // 🔐 Extract UserId from JWT token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User ID not found in authentication token" });
            }

            var loan = await _mediator.Send(new RequestLoanCommand(userId, request.BookId));
            
            return Ok(loan);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Checks the availability of a specific book
    /// </summary>
    [HttpGet("availability/{bookId}")]
    [ProducesResponseType(typeof(AvailabilityDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AvailabilityDTO>> GetAvailability(int bookId)
    {
        var result = await _mediator.Send(new GetBookAvailabilityQuery(bookId));
        if (result == null) return NotFound(new { message = "Book not found" });
        return Ok(result);
    }
}
