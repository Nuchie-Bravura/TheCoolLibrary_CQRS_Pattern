using CoolLibrary.Application.DTO.Customer;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CoolLibrary.Application.UseCases.Customers.Commands.UpdateCustomer
{
    public record UpdateCustomerCommand(int CustomerId, JsonPatchDocument<UpdateCustomerDTO> PatchDoc, ModelStateDictionary ModelState) : IRequest<CustomerDTO?>;
}
