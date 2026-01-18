using Asp.Versioning;
using CoolLibrary.Application.DTO.Customer;
using CoolLibrary.Application.UseCases.Customers.Commands.CreateCustomer;
using CoolLibrary.Application.UseCases.Customers.Commands.DeleteCustomer;
using CoolLibrary.Application.UseCases.Customers.Commands.UpdateCustomer;
using CoolLibrary.Application.UseCases.Customers.Queries.GetAllCustomers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

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
    private readonly IMediator _mediator;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(
        IMediator mediator, 
        ILogger<CustomersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
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
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CustomerDTO>>> GetAll()
    {
        var customers = await _mediator.Send(new GetAllCustomersQuery());
        return Ok(customers);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerDTO>> Create([FromBody] CreateCustomerDTO createCustomerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var createdCustomer = await _mediator.Send(new CreateCustomerCommand(createCustomerDto));
            return CreatedAtAction(nameof(GetAll), new { id = createdCustomer.CustomerId }, createdCustomer);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _mediator.Send(new DeleteCustomerCommand(id));
        
        if (!deleted)
        {
            return NotFound($"Customer with ID {id} not found");
        }

        return NoContent();
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

        try
        {
            var updatedCustomer = await _mediator.Send(new UpdateCustomerCommand(id, patchDoc, ModelState));

            if (updatedCustomer == null)
            {
               return NotFound($"Customer with ID {id} not found.");
            }
            
            // Check if ModelState became invalid during Handler processing (although unlikely with my current implementation exception)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(updatedCustomer);
        }
        catch (ArgumentException ex)
        {
             // If handler threw exception due to validation
             return BadRequest(ModelState.IsValid ? ex.Message : ModelState);
        }
    }
}
