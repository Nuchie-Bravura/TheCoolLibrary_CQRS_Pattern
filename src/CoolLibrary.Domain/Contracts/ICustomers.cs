using CoolLibrary.Domain.Entities;
using CoolLibrary.Domain.Enums;

namespace CoolLibrary.Domain.Contracts;

/// <summary>
/// Repository contract for managing Customer entities
/// </summary>
public interface ICustomers
{
    /// <summary>
    /// Gets all customers
    /// </summary>
    /// <returns>Collection of all customers</returns>
    Task<IEnumerable<Customer>> GetAllAsync();

    /// <summary>
    /// Gets a customer by their unique identifier
    /// </summary>
    /// <param name="customerId">The customer's unique identifier</param>
    /// <returns>The customer if found, null otherwise</returns>
    Task<Customer?> GetByIdAsync(int customerId);

    /// <summary>
    /// Gets customers by name (first name, last name, or full name)
    /// </summary>
    /// <param name="name">The name to search for</param>
    /// <returns>Collection of customers matching the name</returns>
    Task<IEnumerable<Customer>> GetByNameAsync(string name);

    /// <summary>
    /// Gets a customer by their email address
    /// </summary>
    /// <param name="email">The email to search for</param>
    /// <returns>The customer if found, null otherwise</returns>
    Task<Customer?> GetByEmailAsync(string email);

    /// <summary>
    /// Gets a customer by their ApplicationUser ID (from JWT token)
    /// Used for secure operations where UserId comes from authentication
    /// </summary>
    /// <param name="userId">The ApplicationUser ID (GUID from AspNetUsers)</param>
    /// <returns>The customer if found, null otherwise</returns>
    Task<Customer?> GetCustomerByUserIdAsync(string userId);

    /// <summary>
    /// Gets customers by membership status
    /// </summary>
    /// <param name="status">The membership status to filter by</param>
    /// <returns>Collection of customers with matching status</returns>
    Task<IEnumerable<Customer>> GetByMembershipStatusAsync(MembershipStatus status);

    /// <summary>
    /// Gets customers by city
    /// </summary>
    /// <param name="city">The city to filter by</param>
    /// <returns>Collection of customers in the specified city</returns>
    Task<IEnumerable<Customer>> GetByCityAsync(string city);

    /// <summary>
    /// Gets customers who have active loans
    /// </summary>
    /// <returns>Collection of customers with active loans</returns>
    Task<IEnumerable<Customer>> GetCustomersWithActiveLoansAsync();

    /// <summary>
    /// Gets customers who have unpaid fines
    /// </summary>
    /// <returns>Collection of customers with unpaid fines</returns>
    Task<IEnumerable<Customer>> GetCustomersWithUnpaidFinesAsync();

    /// <summary>
    /// Gets customers who have overdue loans
    /// </summary>
    /// <returns>Collection of customers with overdue loans</returns>
    Task<IEnumerable<Customer>> GetCustomersWithOverdueLoansAsync();

    /// <summary>
    /// Inserts a new customer
    /// </summary>
    /// <param name="customer">The customer to insert</param>
    /// <returns>The inserted customer with generated ID</returns>
    Task<Customer> InsertAsync(Customer customer);

    /// <summary>
    /// Updates an existing customer completely
    /// </summary>
    /// <param name="customer">The customer with updated information</param>
    /// <returns>The updated customer</returns>
    Task<Customer> UpdateAsync(Customer customer);

    /// <summary>
    /// Partially updates a customer (patch)
    /// </summary>
    /// <param name="customerId">The customer's unique identifier</param>
    /// <param name="updates">Dictionary of property names and values to update</param>
    /// <returns>The updated customer if found, null otherwise</returns>
    Task<Customer?> PatchAsync(int customerId, Dictionary<string, object> updates);

    /// <summary>
    /// Deletes a customer by their unique identifier
    /// </summary>
    /// <param name="customerId">The customer's unique identifier</param>
    /// <returns>True if deleted successfully, false otherwise</returns>
    Task<bool> DeleteAsync(int customerId);

    /// <summary>
    /// Checks if a customer exists
    /// </summary>
    /// <param name="customerId">The customer's unique identifier</param>
    /// <returns>True if the customer exists, false otherwise</returns>
    Task<bool> ExistsAsync(int customerId);

    /// <summary>
    /// Checks if an email is already registered
    /// </summary>
    /// <param name="email">The email to check</param>
    /// <returns>True if the email exists, false otherwise</returns>
    Task<bool> EmailExistsAsync(string email);

    /// <summary>
    /// Updates the membership status of a customer
    /// </summary>
    /// <param name="customerId">The customer's unique identifier</param>
    /// <param name="status">The new membership status</param>
    /// <returns>True if updated successfully, false otherwise</returns>
    Task<bool> UpdateMembershipStatusAsync(int customerId, MembershipStatus status);

    /// <summary>
    /// Gets all loans for a specific customer
    /// </summary>
    /// <param name="customerId">The customer's unique identifier</param>
    /// <returns>Collection of loans for the customer</returns>
    Task<IEnumerable<Loan>> GetCustomerLoansAsync(int customerId);

    /// <summary>
    /// Gets all reservations for a specific customer
    /// </summary>
    /// <param name="customerId">The customer's unique identifier</param>
    /// <returns>Collection of reservations for the customer</returns>
    Task<IEnumerable<Reservation>> GetCustomerReservationsAsync(int customerId);

    /// <summary>
    /// Gets all fines for a specific customer
    /// </summary>
    /// <param name="customerId">The customer's unique identifier</param>
    /// <returns>Collection of fines for the customer</returns>
    Task<IEnumerable<Fine>> GetCustomerFinesAsync(int customerId);

    /// <summary>
    /// Saves all pending changes
    /// </summary>
    /// <returns>The number of affected records</returns>
    Task<int> SaveChangesAsync();
}
