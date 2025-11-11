using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoolLibrary.Domain.Entities;  
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CoolLibrary.Application.Services;

/// <summary>
/// Service responsible for generating JWT (JSON Web Token) tokens
/// Handles token creation and configuration
/// Now uses ApplicationUser for custom user properties
/// </summary>
public class TokenService
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Constructor - receives configuration to access JWT settings
    /// (Key, Issuer, Audience, ExpirationMinutes)
    /// </summary>
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Generates a JWT token for an authenticated user
    /// </summary>
    /// <param name="user">The authenticated ApplicationUser</param>
    /// <param name="roles">List of roles assigned to the user (optional)</param>
    /// <returns>JWT token as a string</returns>
    public string GenerateJwtToken(ApplicationUser user, IList<string>? roles = null)
    {
        // Step 1: Create claims (user information embedded in the token)
        var claims = new List<Claim>
        {
            // Standard JWT claims
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),           // Subject: User ID
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""), // User's email
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID (unique identifier)
            
            // Custom claims
            new Claim(ClaimTypes.NameIdentifier, user.Id),              // User ID (for [Authorize])
            new Claim(ClaimTypes.Name, user.UserName ?? user.Email ?? ""), // Username
            
            // NEW: Include first name and last name from ApplicationUser
            new Claim(ClaimTypes.GivenName, user.FirstName ?? ""),
            new Claim(ClaimTypes.Surname, user.LastName ?? "")
        };

        // Step 2: Add role claims if any (for role-based authorization)
        if (roles != null && roles.Any())
        {
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role)); // e.g., "Admin", "User", "Librarian"
            }
        }

        // Step 3: Get JWT configuration from appsettings.json or User Secrets
        var jwtKey = _configuration["Jwt:Key"] 
            ?? throw new InvalidOperationException("JWT Key is not configured");
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "CoolLibrary";
        var jwtAudience = _configuration["Jwt:Audience"] ?? "CoolLibraryUsers";
        var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");

        // Step 4: Create the signing key (used to sign and verify the token)
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Step 5: Create the JWT token
        var token = new JwtSecurityToken(
            issuer: jwtIssuer,                              // Who created the token
            audience: jwtAudience,                          // Who can use the token
            claims: claims,                                 // User information
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes), // Expiration time
            signingCredentials: credentials                 // Signature for validation
        );

        // Step 6: Serialize the token to a string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Gets the token expiration time based on configuration
    /// </summary>
    /// <returns>DateTime when the token will expire</returns>
    public DateTime GetTokenExpiration()
    {
        var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");
        return DateTime.UtcNow.AddMinutes(expirationMinutes);
    }
}
