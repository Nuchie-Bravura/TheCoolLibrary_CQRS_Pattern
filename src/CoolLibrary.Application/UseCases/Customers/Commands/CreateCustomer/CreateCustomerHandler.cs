using AutoMapper;
using CoolLibrary.Application.DTO.Customer;
using CoolLibrary.Domain.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoolLibrary.Application.UseCases.Customers.Commands.CreateCustomer
{
    public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, CustomerDTO>
    {
        private readonly ICustomers _customersRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateCustomerHandler> _logger;

        public CreateCustomerHandler(ICustomers customersRepository, IMapper mapper, ILogger<CreateCustomerHandler> logger)
        {
            _customersRepository = customersRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CustomerDTO> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var createCustomerDto = request.CreateCustomerDTO;
            try
            {
                var emailExists = await _customersRepository.EmailExistsAsync(createCustomerDto.Email);
                if (emailExists)
                {
                    throw new ArgumentException($"A customer with email '{createCustomerDto.Email}' already exists.");
                }

                var customer = _mapper.Map<CoolLibrary.Domain.Entities.Customer>(createCustomerDto);
                var createdCustomer = await _customersRepository.InsertAsync(customer);
                return _mapper.Map<CustomerDTO>(createdCustomer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                throw;
            }
        }
    }
}
