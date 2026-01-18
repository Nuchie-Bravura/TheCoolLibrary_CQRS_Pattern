namespace CoolLibrary.Domain.Enums;

/// <summary>
/// Represents the status of a fine
/// </summary>
public enum FineStatus
{
    /// <summary>
    /// Fine is pending payment
    /// </summary>
    Pending = 1,
    
    /// <summary>
    /// Fine has been paid
    /// </summary>
    Paid = 2,
    
    /// <summary>
    /// Fine has been waived by the library
    /// </summary>
    Waived = 3
}