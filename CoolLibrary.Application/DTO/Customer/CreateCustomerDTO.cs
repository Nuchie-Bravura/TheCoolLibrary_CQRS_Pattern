using CoolLibrary.Domain.Enums;

namespace CoolLibrary.Application.DTO.Customer;

/// <summary>
/// Data Transfer Object for creating a new Customer.
/// </summary>
public class CreateCustomerDTO
{
    /// <summary>
    /// Customer's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Customer's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Customer's email address (must be unique)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Customer's phone number (optional)
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Customer's street address (optional)
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Customer's city (optional)
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Customer's postal code (optional)
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Maximum number of books allowed (optional, defaults to 5)
    /// </summary>
    public int? MaxBooksAllowed { get; set; }
}
