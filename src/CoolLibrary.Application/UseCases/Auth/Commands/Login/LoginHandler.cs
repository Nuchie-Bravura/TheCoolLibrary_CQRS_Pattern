using CoolLibrary.Application.DTO.Authentication;
using CoolLibrary.Application.Services.Token;
using CoolLibrary.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CoolLibrary.Application.UseCases.Auth.Commands.Login;

public record LoginCommand(LoginDTO LoginDto) : IRequest<AuthResponseDTO>;

public class LoginHandler : IRequestHandler<LoginCommand, AuthResponseDTO>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenService _tokenService;
    private readonly ILogger<LoginHandler> _logger;

    public LoginHandler(
        UserManager<ApplicationUser> userManager,
        TokenService tokenService,
        ILogger<LoginHandler> logger)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<AuthResponseDTO> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var loginDto = request.LoginDto;

        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!isPasswordValid)
        {
            _logger.LogWarning("Failed login attempt for user: {Email}", loginDto.Email);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateJwtToken(user, roles);
        var expiresAt = _tokenService.GetTokenExpiration();

        _logger.LogInformation("User logged in successfully: {Email}", loginDto.Email);

        return new AuthResponseDTO
        {
            Token = token,
            ExpiresAt = expiresAt,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToList()
        };
    }
}
