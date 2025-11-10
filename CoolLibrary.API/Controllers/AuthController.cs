using Asp.Versioning;
using CoolLibrary.Application.DTO;
using CoolLibrary.Application.Services;
using CoolLibrary.Domain.Entities;
using CoolLibrary.Domain.Enums;
using CoolLibrary.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoolLibrary.API.Controllers;

/// <summary>
/// Authentication controller - handles user registration and login
/// These endpoints are public (do not require authentication)
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[Tags("🔐 Authentication")]
[ApiVersion("1.0")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenService _tokenService;
    private readonly LibraryDbContext _dbContext;  // ← Add DbContext to create Customer
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Constructor - Dependency Injection provides these services
    /// </summary>
    public AuthController(
        UserManager<ApplicationUser> userManager,
        TokenService tokenService,
        LibraryDbContext dbContext,  // ← Inject DbContext
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <remarks>
    /// Creates a new user in the system with the provided credentials.
    /// This endpoint is PUBLIC - no authentication required.
    /// 
    /// **NOW ALSO CREATES A CUSTOMER PROFILE AUTOMATICALLY!**
    /// 
    /// Request Sample:
    /// 
    ///     POST /api/v1/auth/register
    ///     {
    ///         "firstName": "John",
    ///         "lastName": "Doe",
    ///         "email": "john.doe@example.com",
    ///         "password": "MySecurePassword123!",
    ///         "confirmPassword": "MySecurePassword123!",
    ///         "phone": "+1-555-0123",
    ///         "address": "123 Main St",
    ///         "city": "New York",
    ///         "postalCode": "10001"
    ///     }
    /// 
    /// Success Response:
    /// 
    ///     {
    ///         "message": "User and customer profile created successfully",
    ///         "email": "john.doe@example.com",
    ///         "customerId": 5,
    ///         "role": "User"
    ///     }
    /// 
    /// </remarks>
    /// <param name="registerDto">Registration data (email, password, confirmPassword)</param>
    /// <returns>Success message if registration is successful</returns>
    /// <response code="200">User registered successfully</response>
    /// <response code="400">Invalid data or email already exists</response>
    /// <response code="500">Internal server error</response>
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
            // Step 1: Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "User with this email already exists" });
            }

            // Step 2: Create new ApplicationUser with FirstName and LastName
            var newUser = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                EmailConfirmed = true,
                FirstName = registerDto.FirstName,   // ← NEW
                LastName = registerDto.LastName,     // ← NEW
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Step 3: Create user in database (password is automatically hashed)
            var result = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = "User registration failed", errors });
            }

            // Step 4: Assign "User" role
            var roleResult = await _userManager.AddToRoleAsync(newUser, "User");
            
            if (!roleResult.Succeeded)
            {
                _logger.LogWarning("Failed to assign User role to {Email}", registerDto.Email);
            }

            // Step 5: Create Customer profile automatically! ✅
            var customer = new Customer
            {
                UserId = newUser.Id,  // Link to ApplicationUser
                Phone = registerDto.Phone,
                Address = registerDto.Address,
                City = registerDto.City,
                PostalCode = registerDto.PostalCode,
                MembershipDate = DateTime.UtcNow,
                MembershipStatus = MembershipStatus.Active,
                MaxBooksAllowed = 5,  // Default value
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.Customers.Add(customer);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("✅ New user and customer created: {Email} (Customer ID: {CustomerId})", 
                registerDto.Email, customer.CustomerId);

            return Ok(new 
            { 
                message = "User and customer profile created successfully",
                email = newUser.Email,
                customerId = customer.CustomerId,  // ← Return customer ID
                role = "User"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    /// <remarks>
    /// Authenticates a user and returns a JWT token if credentials are valid.
    /// This endpoint is PUBLIC - no authentication required.
    /// The token should be included in subsequent requests using the Authorization header.
    /// 
    /// Request Sample:
    /// 
    ///     POST /api/auth/login
    ///     {
    ///         "email": "john.doe@example.com",
    ///         "password": "MySecurePassword123!"
    ///     }
    /// 
    /// Success Response:
    /// 
    ///     {
    ///         "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    ///         "expiresAt": "2024-01-16T15:30:00Z",
    ///         "email": "john.doe@example.com",
    ///         "roles": ["User"]
    ///     }
    /// 
    /// How to use the token:
    /// 
    ///     Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
    /// 
    /// </remarks>
    /// <param name="loginDto">Login credentials (email and password)</param>
    /// <returns>JWT token and user information</returns>
    /// <response code="200">Login successful, returns JWT token</response>
    /// <response code="401">Invalid credentials</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("login")]
    [AllowAnonymous]  // ← Public endpoint - no JWT required
    [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
    {
        // Step 1: Validate model state
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // Step 2: Find user by email
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                // Don't reveal whether the user exists (security best practice)
                return Unauthorized(new { message = "Invalid email or password" });
            }

            // Step 3: Verify password
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Failed login attempt for user: {Email}", loginDto.Email);
                return Unauthorized(new { message = "Invalid email or password" });
            }

            // Step 4: Get user roles (if any)
            var roles = await _userManager.GetRolesAsync(user);

            // Step 5: Generate JWT token
            var token = _tokenService.GenerateJwtToken(user, roles);
            var expiresAt = _tokenService.GetTokenExpiration();

            _logger.LogInformation("User logged in successfully: {Email}", loginDto.Email);

            // Step 6: Return token and user information
            return Ok(new AuthResponseDTO
            {
                Token = token,
                ExpiresAt = expiresAt,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToList()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login");
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    [HttpPost("renewToken")]
    [Authorize] // ← token needed
    [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RenewToken()
    {
        // het userId from current Token
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "Invalid token" });

        // BD
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Unauthorized(new { message = "User not found" });

        // rol
        var roles = await _userManager.GetRolesAsync(user);

        // generate new token
        var newToken = _tokenService.GenerateJwtToken(user, roles);
        var expiresAt = _tokenService.GetTokenExpiration();

        return Ok(new AuthResponseDTO
        {
            Token = newToken,
            ExpiresAt = expiresAt,
            Email = user.Email ?? "",
            Roles = roles.ToList()
        });
    }

}
