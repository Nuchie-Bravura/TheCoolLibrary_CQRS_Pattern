using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using System.Security.Claims;
using CoolLibrary.Application.Services.LoansAndReservations;
using CoolLibrary.Application.DTO.LoansAndReservations;

namespace CoolLibrary.API.Controllers;

/// <summary>
/// Loan operations management
/// Accessible to authenticated users with User or Admin role
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[Tags("🔄 Operations - Loans")]
[Authorize(Roles = "User,Admin")]
[ApiVersion("1.0")]
public class LoansController : ControllerBase
{
    private readonly LoanRequestService _service;
    private readonly ILogger<LoansController> _logger;

    public LoansController(LoanRequestService service, ILogger<LoansController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// ⚠️ DEPRECATED: Requests a loan (INSECURE - accepts CustomerId from client)
    /// Use /request-secure instead
    /// </summary>
    [HttpPost("RequestLoan")]
    [Obsolete("Use POST /request-secure instead - this endpoint is deprecated")]
    [ProducesResponseType(typeof(LoanResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoanResponseDTO>> RequestLoan([FromBody] LoanRequestDTO request)
    {
        _logger.LogWarning("⚠️  Using deprecated endpoint RequestLoan");
        var (ok, error, loan) = await _service.RequestLoanAsync(request);
        if (!ok)
        {
            return BadRequest(new { message = error });
        }
        return Ok(loan);
    }

    /// <summary>
    /// ✅ SECURE: Request a book loan (CustomerId from JWT)
    /// </summary>
    /// <remarks>
    /// Creates a new loan for the authenticated user.
    /// CustomerId is extracted from the JWT token (secure).
    /// 
    /// Request Sample:
    /// 
    ///     POST /api/v1/loans/request-secure
    ///     Headers:
    ///         Authorization: Bearer eyJhbGc...
    ///     Body:
    ///     {
    ///         "bookId": 5
    ///     }
    /// 
    /// Success Response:
    /// 
    ///     {
    ///         "loanId": 123,
    ///         "customerId": 1,
    ///         "bookId": 5,
    ///         "loanDate": "2024-01-15T10:30:00Z",
    ///         "dueDate": "2024-01-29T10:30:00Z",
    ///         "status": "Active"
    ///     }
    /// 
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
            // 🔐 EXTRACT UserId FROM JWT TOKEN (NOT from client request)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("⚠️  No user ID found in JWT token");
                return Unauthorized(new { message = "User ID not found in authentication token" });
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            
            _logger.LogInformation(
                "📖 Loan request - UserId: {UserId}, Email: {Email}, BookId: {BookId}",
                userId, userEmail, request.BookId);

            // ✅ Call secure service method (uses UserId from JWT)
            var (ok, error, loan) = await _service.RequestLoanSecureAsync(userId, request.BookId);
            
            if (!ok)
            {
                _logger.LogWarning(
                    "❌ Loan request failed - UserId: {UserId}, BookId: {BookId}, Error: {Error}",
                    userId, request.BookId, error);
                    
                return BadRequest(new { message = error });
            }

            _logger.LogInformation(
                "✅ Loan created successfully - LoanId: {LoanId}, UserId: {UserId}, BookId: {BookId}",
                loan!.LoanId, userId, request.BookId);

            return Ok(loan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Unexpected error creating loan");
            return StatusCode(500, new { message = "An error occurred while processing the loan request" });
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
        var result = await _service.GetAvailabilityAsync(bookId);
        if (result == null) return NotFound(new { message = "Book not found" });
        return Ok(result);
    }
}
