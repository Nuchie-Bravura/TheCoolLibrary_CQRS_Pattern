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
/// All business logic and logging is delegated to handlers (CQRS pattern)
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[Tags("👥 Management - Customers")]
[Authorize(Roles = "Admin")]
[ApiVersion("1.0")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all customers
    /// </summary>
    /// <remarks>
    /// Response Sample:
    /// 
    ///     GET /api/v1.0/customers
    ///     [
    ///         {
    ///             "customerId": 1,
    ///             "fullName": "John Doe",
    ///             "email": "john.doe@example.com",
    ///             "membershipStatus": "Active",
    ///             "membershipDate": "2024-01-01T00:00:00Z"
    ///         }
    ///     ]
    /// 
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CustomerDTO>>> GetAll()
    {
        var customers = await _mediator.Send(new GetAllCustomersQuery());
        return Ok(customers);
    }

    /// <summary>
    /// Creates a new customer
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerDTO), StatusCodes.Status201Created)]
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

    /// <summary>
    /// Deletes a customer by ID
    /// </summary>
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
 
    /// <summary>
    /// Partially updates a customer using JSON Patch
    /// </summary>
    [HttpPatch("{id}")]
    [ProducesResponseType(typeof(CustomerDTO), StatusCodes.Status200OK)]
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
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(updatedCustomer);
        }
        catch (ArgumentException ex)
        {
             return BadRequest(ModelState.IsValid ? ex.Message : ModelState);
        }
    }
}
