using CoolLibrary.Domain.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoolLibrary.Application.UseCases.Customers.Commands.DeleteCustomer
{
    public class DeleteCustomerHandler : IRequestHandler<DeleteCustomerCommand, bool>
    {
        private readonly ICustomers _customersRepository;
        private readonly ILogger<DeleteCustomerHandler> _logger;

        public DeleteCustomerHandler(ICustomers customersRepository, ILogger<DeleteCustomerHandler> logger)
        {
            _customersRepository = customersRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            var customerId = request.CustomerId;
            try
            {
                var deleted = await _customersRepository.DeleteAsync(customerId);
                if (deleted)
                {
                    _logger.LogInformation("Customer with ID {CustomerId} deleted successfully", customerId);
                }
                return deleted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer with ID: {CustomerId}", customerId);
                throw;
            }
        }
    }
}
