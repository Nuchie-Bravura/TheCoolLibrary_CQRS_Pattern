using AutoMapper;
using CoolLibrary.Application.DTO;
using CoolLibrary.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;

namespace CoolLibrary.API.Controllers;

/// <summary>
/// Loan operations management
/// Accessible to authenticated users with User or Admin role
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]  // ← Versioned route
[Produces("application/json")]
[Tags("🔄 Operations - Loans")]
[Authorize(Roles = "User,Admin")]  // ← Both User and Admin can access
[ApiVersion("1.0")]  // ← This controller belongs to API v1.0
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
    /// Requests a new book loan
    /// </summary>
    /// <remarks>
    /// Processes a loan request validating:
    /// - Book availability (must have copies available)
    /// - Customer existence and status
    /// - Customer's borrowing limit
    /// 
    /// Request Sample:
    /// 
    ///     POST /api/loans
    ///     {
    ///         "customerId": 1,
    ///         "bookId": 5,
    ///         "loanDate": "2024-01-15",
    ///         "expectedReturnDate": "2024-02-15"
    ///     }
    /// 
    /// Success Response Sample:
    /// 
    ///     {
    ///         "loanId": 123,
    ///         "customerId": 1,
    ///         "bookId": 5,
    ///         "loanDate": "2024-01-15",
    ///         "expectedReturnDate": "2024-02-15",
    ///         "status": "Active",
    ///         "remainingCopies": 4
    ///     }
    /// 
    /// </remarks>
    /// <param name="request">The loan request details</param>
    /// <returns>Information about the created loan</returns>
    /// <response code="200">Loan created successfully</response>
    /// <response code="400">Invalid request (book unavailable, customer doesn't exist, borrowing limit reached, etc.)</response>
    /// <response code="500">Internal server error occurred</response>
    [HttpPost("RequestLoan")]
    [ProducesResponseType(typeof(LoanResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LoanResponseDTO>> RequestLoan([FromBody] LoanRequestDTO request)
    {
        var (ok, error, loan) = await _service.RequestLoanAsync(request);
        if (!ok)
        {
            return BadRequest(error);
        }
        return Ok(loan);
    }

    /// <summary>
    /// Checks the availability of a specific book
    /// </summary>
    /// <remarks>
    /// Verifies the current availability status of a book including:
    /// - Total number of copies
    /// - Available copies
    /// - Currently loaned copies
    /// 
    /// Usage Example:
    /// 
    ///     GET /api/loans/availability/5
    /// 
    /// Response Sample:
    /// 
    ///     {
    ///         "bookId": 5,
    ///         "bookTitle": "Clean Code",
    ///         "totalCopies": 10,
    ///         "availableCopies": 6,
    ///         "loanedCopies": 4,
    ///         "isAvailable": true
    ///     }
    /// 
    /// </remarks>
    /// <param name="bookId">The unique identifier of the book to check</param>
    /// <returns>Availability information for the specified book</returns>
    /// <response code="200">Returns availability information successfully</response>
    /// <response code="404">Book not found</response>
    /// <response code="500">Internal server error occurred</response>
    [HttpGet("availability/{bookId}")]
    [ProducesResponseType(typeof(AvailabilityDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AvailabilityDTO>> GetAvailability(int bookId)
    {
        var result = await _service.GetAvailabilityAsync(bookId);
        if (result == null) return NotFound();
        return Ok(result);
    }
}
