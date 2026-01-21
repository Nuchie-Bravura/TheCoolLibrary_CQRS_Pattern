using AutoMapper;
using CoolLibrary.Application.DTO.Customer;
using CoolLibrary.Domain.Contracts;
using CoolLibrary.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace CoolLibrary.Application.UseCases.Customers.Commands.UpdateCustomer
{
    public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand, CustomerDTO?>
    {
        private readonly ICustomers _customersRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCustomerHandler> _logger;

        public UpdateCustomerHandler(ICustomers customersRepository, IMapper mapper, ILogger<UpdateCustomerHandler> logger)
        {
            _customersRepository = customersRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CustomerDTO?> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customerId = request.CustomerId;
            var patchDoc = request.PatchDoc;
            var modelState = request.ModelState;

            var customerEntity = await _customersRepository.GetByIdAsync(customerId);
            if (customerEntity == null)
            {
                _logger.LogWarning("Update failed: Customer with ID {CustomerId} not found", customerId);
                return null;
            }

            var customerToPatch = _mapper.Map<UpdateCustomerDTO>(customerEntity);
            
            patchDoc.ApplyTo(customerToPatch, modelState);

            if (!modelState.IsValid)
            {
                 _logger.LogWarning("Update failed: Invalid patch data for customer {CustomerId}", customerId);
                 throw new ArgumentException("Invalid state after patch");
            }

            _mapper.Map(customerToPatch, customerEntity);
            await _customersRepository.UpdateAsync(customerEntity);

            _logger.LogInformation("Customer with ID {CustomerId} updated successfully", customerId);

            return _mapper.Map<CustomerDTO>(customerEntity);
        }
    }
}
