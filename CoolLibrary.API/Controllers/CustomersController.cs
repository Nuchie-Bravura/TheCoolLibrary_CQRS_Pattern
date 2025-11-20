using AutoMapper;
using CoolLibrary.Domain.Contracts;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using CoolLibrary.Application.DTO.Customer;
using CoolLibrary.Application.Services.Cache;

namespace CoolLibrary.API.Controllers;

/// <summary>
/// Customer management controller
/// All endpoints require JWT authentication ([Authorize])
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]  // ← Versioned route
[Produces("application/json")]
[Tags("👥 Management - Customers")]
[Authorize(Roles = "Admin")] // JWT token required for all endpoints in this controller and only visible to Admin role
[ApiVersion("1.0")]  // ← This controller belongs to API v1.0
public class CustomersController : ControllerBase
{
    private readonly ICustomers _customersRepository;
    private readonly ILogger<CustomersController> _logger;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public CustomersController(
        ICustomers customersRepository, 
        ILogger<CustomersController> logger, 
        IMapper mapper,
        ICacheService cacheService)
    {
        _customersRepository = customersRepository;
        _logger = logger;
        _mapper = mapper;
        _cacheService = cacheService;
    }



    /// <summary>
    /// gets all customers
    /// </summary>
    /// <remarks>
    /// 
    /// 
    /// Response Sample:
    /// 
    ///     GET /api/customers
    ///     [
    ///         {
    ///             "customerId": 1,
    ///             "firstName": "John",
    ///             "lastName": "Doe",
    ///             "email": "john.doe@example.com"
    ///         }
    ///     ]
    /// 
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CustomerDTO>>> GetAll()
    {
        const string cacheKey = "customers:all";

        try
        {
            var customerDTOs = await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                _logger.LogInformation("Cache miss for {CacheKey}, fetching from database", cacheKey);
                var customers = await _customersRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<CustomerDTO>>(customers);
            });

            return Ok(customerDTOs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all customers");
            return StatusCode(500, "An error occurred while retrieving customers");
        }
    }


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerDTO>> Create([FromBody] CreateCustomerDTO createCustomerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var emailExists = await _customersRepository.EmailExistsAsync(createCustomerDto.Email);
            if (emailExists)
            {
                return BadRequest($"A customer with email '{createCustomerDto.Email}' already exists.");
            }

            // Mapear CreateCustomerDTO a Customer (entidad)
            var customer = _mapper.Map<Domain.Entities.Customer>(createCustomerDto);
            
            // Insertar en la base de datos
            var createdCustomer = await _customersRepository.InsertAsync(customer);
            
            // Mapear Customer (entidad) a CustomerDTO (respuesta)
            var customerDTO = _mapper.Map<CustomerDTO>(createdCustomer);
            
            return CreatedAtAction(nameof(GetAll), new { id = createdCustomer.CustomerId }, customerDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return StatusCode(500, "An error occurred while creating the customer");
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _customersRepository.DeleteAsync(id);
            
            if (!deleted)
            {
                return NotFound($"Customer with ID {id} not found");
            }

            _logger.LogInformation("Customer with ID {CustomerId} deleted successfully", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer with ID: {CustomerId}", id);
            return StatusCode(500, "An error occurred while deleting the customer");
        }
    }

 
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDTO>> Patch(int id, [FromBody] JsonPatchDocument<UpdateCustomerDTO> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest("Patch document cannot be null.");
        }

        var customerEntity = await _customersRepository.GetByIdAsync(id);
        if (customerEntity == null)
        {
            return NotFound($"Customer with ID {id} not found.");
        }

    
        var customerToPatch = _mapper.Map<UpdateCustomerDTO>(customerEntity);
        
        patchDoc.ApplyTo(customerToPatch, ModelState);

 
        if (!TryValidateModel(customerToPatch))
        {
            return BadRequest(ModelState);
        }

      
        _mapper.Map(customerToPatch, customerEntity);

 
        await _customersRepository.UpdateAsync(customerEntity);

   
        var updatedCustomerDto = _mapper.Map<CustomerDTO>(customerEntity);

        return Ok(updatedCustomerDto);
    }
}
