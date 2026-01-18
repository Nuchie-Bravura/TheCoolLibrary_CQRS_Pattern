using CoolLibrary.Application.DTO.Customer;
using MediatR;

namespace CoolLibrary.Application.UseCases.Customers.Commands.CreateCustomer
{
    public record CreateCustomerCommand(CreateCustomerDTO CreateCustomerDTO) : IRequest<CustomerDTO>;
}
