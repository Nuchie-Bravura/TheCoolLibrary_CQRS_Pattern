namespace CoolLibrary.Domain.Enums;

/// <summary>
/// Represents the reason for a fine
/// </summary>
public enum FineReason
{
    /// <summary>
    /// Fine for returning a book late
    /// </summary>
    LateReturn = 1,
    
    /// <summary>
    /// Fine for a lost book
    /// </summary>
    LostBook = 2,
    
    /// <summary>
    /// Fine for returning a damaged book
    /// </summary>
    DamagedBook = 3
}