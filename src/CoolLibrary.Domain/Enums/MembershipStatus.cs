namespace CoolLibrary.Domain.Enums;

/// <summary>
/// Represents the membership status of a library customer
/// </summary>
public enum MembershipStatus
{
    /// <summary>
    /// Active membership - can borrow books
    /// </summary>
    Active = 1,
    
    /// <summary>
    /// Suspended membership - cannot borrow books temporarily
    /// </summary>
    Suspended = 2,
    
    /// <summary>
    /// Expired membership - needs renewal
    /// </summary>
    Expired = 3
}