namespace CoolLibrary.Application.DTO.Authentication;

/// <summary>
/// Data Transfer Object for authentication response
/// Returned to the client after successful login
/// Contains the JWT token and related information
/// </summary>
public class AuthResponseDTO
{
    /// <summary>
    /// JWT (JSON Web Token) - the authentication token
    /// Client must include this in the Authorization header for protected endpoints
    /// Format: "Bearer {token}"
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration timestamp (UTC)
    /// After this time, the token becomes invalid and user must login again
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Email of the authenticated user
    /// Useful for displaying user info in the client application
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Optional: User roles (e.g., "Admin", "User", "Librarian")
    /// Can be used for role-based authorization in the client
    /// </summary>
    public List<string> Roles { get; set; } = new List<string>();
}
