using Microsoft.AspNetCore.Identity;

namespace CoolLibrary.Domain.Entities;

/// <summary>
/// Extends IdentityUser to add custom properties for the application
/// Represents the authentication/authorization user in the system
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Full name property for convenience
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Date when the user account was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date when the user account was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property (Optional - One-to-One with Customer)
    /// <summary>
    /// Customer profile associated with this user (if the user is a library customer)
    /// Not all users are customers (e.g., Admin users may not have a customer profile)
    /// </summary>
    public virtual Customer? Customer { get; set; }
}
