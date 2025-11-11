using System.ComponentModel.DataAnnotations;

namespace CoolLibrary.Application.DTO;

/// <summary>
/// DTO SEGURO para crear préstamo
/// Solo contiene BookId - CustomerId se obtiene del JWT
/// Previene spoofing attacks
/// </summary>
public class CreateLoanBookOnlyDTO
{
    /// <summary>
    /// ID del libro a pedir prestado
    /// </summary>
    [Required(ErrorMessage = "Book ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Book ID must be a positive number")]
    public int BookId { get; set; }
}
