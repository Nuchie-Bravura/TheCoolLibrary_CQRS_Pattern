using System;
using CoolLibrary.Domain.Enums;

namespace CoolLibrary.Application.DTO.Customer;

/// <summary>
/// Data Transfer Object for a Customer.
/// </summary>
public class CustomerDTO
{
    /// <summary>
    /// Unique identifier for the customer.
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// The full name of the customer.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// The customer's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The customer's membership status.
    /// </summary>
    public string MembershipStatus { get; set; } = string.Empty;

    /// <summary>
    /// The date the customer's membership started.
    /// </summary>
    public DateTime MembershipDate { get; set; }
}
