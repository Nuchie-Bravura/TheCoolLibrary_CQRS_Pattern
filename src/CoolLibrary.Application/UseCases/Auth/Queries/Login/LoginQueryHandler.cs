using CoolLibrary.Application.DTO.Authentication;
using CoolLibrary.Application.Services.Token;
using CoolLibrary.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CoolLibrary.Application.UseCases.Auth.Queries.Login;

/// <summary>
/// Handler for LoginQuery - validates credentials and generates JWT token
/// This is a Query handler because it only reads data (user validation)
/// </summary>
public class LoginQueryHandler : IRequestHandler<LoginQuery, AuthResponseDTO>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenService _tokenService;
    private readonly ILogger<LoginQueryHandler> _logger;

    public LoginQueryHandler(
        UserManager<ApplicationUser> userManager,
        TokenService tokenService,
        ILogger<LoginQueryHandler> logger)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<AuthResponseDTO> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var loginDto = request.LoginDto;

        // Step 1: Find user by email
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Step 2: Validate password
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!isPasswordValid)
        {
            _logger.LogWarning("Failed login attempt for user: {Email}", loginDto.Email);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Step 3: Get user roles
        var roles = await _userManager.GetRolesAsync(user);

        // Step 4: Generate JWT token
        var token = _tokenService.GenerateJwtToken(user, roles);
        var expiresAt = _tokenService.GetTokenExpiration();

        _logger.LogInformation("User logged in successfully: {Email}", loginDto.Email);

        // Step 5: Return authentication response
        return new AuthResponseDTO
        {
            Token = token,
            ExpiresAt = expiresAt,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToList()
        };
    }
}
