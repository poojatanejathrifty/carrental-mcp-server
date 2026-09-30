using CarRental.Application.DTOs;
using CarRental.Application.DTOs.Requests;
using CarRental.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class CustomersController(ICustomerService customerService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<CustomerDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CustomerDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await customerService.GetAllAsync(cancellationToken));

    [HttpGet("{customerId:int}")]
    [ProducesResponseType<CustomerDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetById(int customerId, CancellationToken cancellationToken)
        => Ok(await customerService.GetByIdAsync(customerId, cancellationToken));

    [HttpPost]
    [ProducesResponseType<CustomerDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CustomerDto>> Create(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = await customerService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { customerId = customer.CustomerId }, customer);
    }

    [HttpPut("{customerId:int}")]
    [ProducesResponseType<CustomerDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> Update(int customerId, UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        if (customerId != request.CustomerId)
        {
            return BadRequest("Route customerId does not match request body.");
        }

        return Ok(await customerService.UpdateAsync(request, cancellationToken));
    }

    [HttpDelete("{customerId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int customerId, CancellationToken cancellationToken)
    {
        await customerService.DeleteAsync(customerId, cancellationToken);
        return NoContent();
    }
}
