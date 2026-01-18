using CoolLibrary.Application.DTO.Customer;
using MediatR;

namespace CoolLibrary.Application.UseCases.Customers.Queries.GetAllCustomers
{
    public record GetAllCustomersQuery : IRequest<IEnumerable<CustomerDTO>>;
}
