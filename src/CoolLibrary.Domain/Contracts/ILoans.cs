using CoolLibrary.Domain.Entities;

namespace CoolLibrary.Domain.Contracts;

/// <summary>
/// Repository contract for managing Loan entities
/// </summary>
public interface ILoans
{
    Task<Loan> CreateAsync(Loan loan);
    Task<Loan?> GetActiveByIdAsync(int loanId);
    Task<int> GetActiveLoanCountForCustomerAsync(int customerId);
    Task<bool> HasActiveLoanForBookAsync(int customerId, int bookId);
    Task<bool> ReturnAsync(int loanId, DateTime returnDate);
    Task<int> SaveChangesAsync();
}
