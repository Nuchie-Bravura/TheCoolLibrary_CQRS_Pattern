namespace CoolLibrary.Application.DTO.LoansAndReservations;

public class AvailabilityDTO
{
    public int BookId { get; set; }
    public int AvailableCopies { get; set; }
    public bool IsAvailable { get; set; }
}
