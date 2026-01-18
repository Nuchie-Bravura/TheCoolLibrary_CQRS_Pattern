using AutoMapper;
using CoolLibrary.Application.DTO.Customer;
using CoolLibrary.Application.Services.Cache;
using CoolLibrary.Domain.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoolLibrary.Application.UseCases.Customers.Queries.GetAllCustomers
{
    public class GetAllCustomersHandler : IRequestHandler<GetAllCustomersQuery, IEnumerable<CustomerDTO>>
    {
        private readonly ICustomers _customersRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly ILogger<GetAllCustomersHandler> _logger;

        public GetAllCustomersHandler(
            ICustomers customersRepository,
            IMapper mapper,
            ICacheService cacheService,
            ILogger<GetAllCustomersHandler> logger)
        {
            _customersRepository = customersRepository;
            _mapper = mapper;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<IEnumerable<CustomerDTO>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            const string cacheKey = "customers:all";

            try
            {
                return await _cacheService.GetOrSetAsync(cacheKey, async () =>
                {
                    _logger.LogInformation("Cache miss for {CacheKey}, fetching from database", cacheKey);
                    var customers = await _customersRepository.GetAllAsync();
                    return _mapper.Map<IEnumerable<CustomerDTO>>(customers);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all customers");
                throw;
            }
        }
    }
}
