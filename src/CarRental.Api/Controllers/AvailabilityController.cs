using CarRental.Application.DTOs;
using CarRental.Application.DTOs.Requests;
using CarRental.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AvailabilityController(IAvailabilityService availabilityService) : ControllerBase
{
    [HttpPost("search")]
    [ProducesResponseType<IReadOnlyList<VehicleDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<VehicleDto>>> Search(AvailabilitySearchRequest request, CancellationToken cancellationToken)
        => Ok(await availabilityService.SearchAvailableVehiclesAsync(request, cancellationToken));

    [HttpGet("{vehicleId:int}")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> IsAvailable(int vehicleId, [FromQuery] DateTime pickupDateUtc, [FromQuery] DateTime returnDateUtc, CancellationToken cancellationToken)
        => Ok(await availabilityService.IsVehicleAvailableAsync(vehicleId, pickupDateUtc, returnDateUtc, cancellationToken));
}
