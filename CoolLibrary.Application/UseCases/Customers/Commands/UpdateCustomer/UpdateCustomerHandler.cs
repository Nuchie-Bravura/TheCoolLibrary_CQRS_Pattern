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
                return null;
            }

            var customerToPatch = _mapper.Map<UpdateCustomerDTO>(customerEntity);
            
            patchDoc.ApplyTo(customerToPatch, modelState);

            if (!modelState.IsValid)
            {
                 // In a real Clean Architecture, we might throw a ValidationException here.
                 // For now, we will return null or throw depending on preference. 
                 // But since the controller needs the ModelState errors, passing ModelState down is one way.
                 // However, throwing an exception with errors is better. 
                 // Or we could return a result object. 
                 // For simplicity in this migration, we'll assume the controller checks ModelState *after* the command?
                 // No, apply happens here.
                 // If ModelState is invalid, we throw.
                 throw new ArgumentException("Invalid state after patch");
            }

            _mapper.Map(customerToPatch, customerEntity);
            await _customersRepository.UpdateAsync(customerEntity);

            return _mapper.Map<CustomerDTO>(customerEntity);
        }
    }
}
