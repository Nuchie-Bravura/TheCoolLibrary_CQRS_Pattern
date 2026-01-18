using MediatR;

namespace CoolLibrary.Application.UseCases.Customers.Commands.DeleteCustomer
{
    public record DeleteCustomerCommand(int CustomerId) : IRequest<bool>;
}
