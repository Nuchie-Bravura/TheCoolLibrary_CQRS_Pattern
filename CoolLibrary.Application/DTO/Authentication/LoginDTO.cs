using System.ComponentModel.DataAnnotations;

namespace CoolLibrary.Application.DTO.Authentication;

/// <summary>
/// Data Transfer Object for user login
/// Contains credentials required to authenticate an existing user
/// </summary>
public class LoginDTO
{
    /// <summary>
    /// User's email address used as username
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's password for authentication
    /// Note: No ConfirmPassword needed here since the account already exists
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}
