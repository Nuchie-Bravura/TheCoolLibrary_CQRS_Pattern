using CoolLibrary.Application.DTO.LoansAndReservations;
using MediatR;

namespace CoolLibrary.Application.UseCases.Loans.Queries.GetBookAvailability
{
    public record GetBookAvailabilityQuery(int BookId) : IRequest<AvailabilityDTO?>;
}
