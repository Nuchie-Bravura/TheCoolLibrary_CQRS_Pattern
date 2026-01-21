using CoolLibrary.Application.DTO.Authentication;
using CoolLibrary.Application.Services.Token;
using CoolLibrary.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CoolLibrary.Application.UseCases.Auth.Commands.RenewToken;

public record RenewTokenCommand(string UserId) : IRequest<AuthResponseDTO>;

/// <summary>
/// Handler for renewing JWT tokens for authenticated users
/// </summary>
public class RenewTokenHandler : IRequestHandler<RenewTokenCommand, AuthResponseDTO>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenService _tokenService;
    private readonly ILogger<RenewTokenHandler> _logger;

    public RenewTokenHandler(
        UserManager<ApplicationUser> userManager,
        TokenService tokenService,
        ILogger<RenewTokenHandler> logger)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<AuthResponseDTO> Handle(RenewTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            _logger.LogWarning("Token renewal failed: User not found (ID: {UserId})", request.UserId);
            throw new UnauthorizedAccessException("User not found");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var newToken = _tokenService.GenerateJwtToken(user, roles);
        var expiresAt = _tokenService.GetTokenExpiration();

        _logger.LogInformation("Token renewed successfully for user: {Email}", user.Email);

        return new AuthResponseDTO
        {
            Token = newToken,
            ExpiresAt = expiresAt,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToList()
        };
    }
}
