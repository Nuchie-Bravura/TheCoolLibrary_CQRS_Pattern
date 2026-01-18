using CoolLibrary.Application.DTO.Authentication;
using CoolLibrary.Application.Services.Token;
using CoolLibrary.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CoolLibrary.Application.UseCases.Auth.Commands.RenewToken;

public record RenewTokenCommand(string UserId) : IRequest<AuthResponseDTO>;

public class RenewTokenHandler : IRequestHandler<RenewTokenCommand, AuthResponseDTO>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenService _tokenService;

    public RenewTokenHandler(
        UserManager<ApplicationUser> userManager,
        TokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDTO> Handle(RenewTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var newToken = _tokenService.GenerateJwtToken(user, roles);
        var expiresAt = _tokenService.GetTokenExpiration();

        return new AuthResponseDTO
        {
            Token = newToken,
            ExpiresAt = expiresAt,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToList()
        };
    }
}
