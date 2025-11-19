using System;

namespace CoolLibrary.Application.DTO.LoansAndReservations;

public class LoanResponseDTO
{
    public int LoanId { get; set; }
    public int CustomerId { get; set; }
    public int BookId { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
