namespace CoolLibrary.Domain.Enums;

/// <summary>
/// Represents the status of a book loan
/// </summary>
public enum LoanStatus
{
    /// <summary>
    /// Loan is currently active
    /// </summary>
    Active = 1,
    
    /// <summary>
    /// Book has been returned
    /// </summary>
    Returned = 2,
    
    /// <summary>
    /// Loan is overdue (past due date)
    /// </summary>
    Overdue = 3,
    
    /// <summary>
    /// Book has been reported as lost
    /// </summary>
    Lost = 4
}