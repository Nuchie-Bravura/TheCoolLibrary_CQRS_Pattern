using CoolLibrary.Application.DTO.LoansAndReservations;
using MediatR;

namespace CoolLibrary.Application.UseCases.Loans.Commands.RequestLoan
{
    public record RequestLoanCommand(string UserId, int BookId) : IRequest<LoanResponseDTO>;
}
