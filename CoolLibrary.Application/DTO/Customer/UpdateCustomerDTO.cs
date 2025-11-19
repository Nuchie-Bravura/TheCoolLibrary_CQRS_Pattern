using System.ComponentModel.DataAnnotations;

namespace CoolLibrary.Application.DTO.Customer;

public class UpdateCustomerDTO
{
    [StringLength(100, ErrorMessage = "First name cannot be longer than 100 characters.")]
    public string? FirstName { get; set; }

    [StringLength(100, ErrorMessage = "Last name cannot be longer than 100 characters.")]
    public string? LastName { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [StringLength(200, ErrorMessage = "Email cannot be longer than 200 characters.")]
    public string? Email { get; set; }

    [StringLength(20, ErrorMessage = "Phone number cannot be longer than 20 characters.")]
    public string? Phone { get; set; }

    [StringLength(300, ErrorMessage = "Address cannot be longer than 300 characters.")]
    public string? Address { get; set; }

    [StringLength(100, ErrorMessage = "City cannot be longer than 100 characters.")]
    public string? City { get; set; }

    [StringLength(20, ErrorMessage = "Postal code cannot be longer than 20 characters.")]
    public string? PostalCode { get; set; }

    [Range(1, 100, ErrorMessage = "Max books allowed must be between 1 and 100.")]
    public int? MaxBooksAllowed { get; set; }
}
