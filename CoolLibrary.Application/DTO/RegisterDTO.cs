using System.ComponentModel.DataAnnotations;

namespace CoolLibrary.Application.DTO;

/// <summary>
/// Data Transfer Object for user registration
/// Contains the required information to create a new user account AND customer profile
/// </summary>
public class RegisterDTO
{
    /// <summary>
    /// User's first name
    /// </summary>
    [Required(ErrorMessage = "First name is required")]
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's email address - will be used as username for login
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's password - must meet security requirements
    /// (minimum 6 characters, at least one uppercase, one lowercase, one digit)
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Password confirmation - must match the Password field
    /// This prevents typos when creating an account
    /// </summary>
    [Required(ErrorMessage = "Password confirmation is required")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    // Optional fields for customer profile
    
    /// <summary>
    /// User's phone number (optional)
    /// </summary>
    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? Phone { get; set; }

    /// <summary>
    /// User's address (optional)
    /// </summary>
    [StringLength(300, ErrorMessage = "Address cannot exceed 300 characters")]
    public string? Address { get; set; }

    /// <summary>
    /// User's city (optional)
    /// </summary>
    [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    public string? City { get; set; }

    /// <summary>
    /// User's postal code (optional)
    /// </summary>
    [StringLength(20, ErrorMessage = "Postal code cannot exceed 20 characters")]
    public string? PostalCode { get; set; }
}
