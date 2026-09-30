using CarRental.Application.DTOs;
using CarRental.Application.DTOs.Requests;
using CarRental.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class VehiclesController(IVehicleService vehicleService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<VehicleDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<VehicleDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await vehicleService.GetAllAsync(cancellationToken));

    [HttpGet("{vehicleId:int}")]
    [ProducesResponseType<VehicleDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VehicleDto>> GetById(int vehicleId, CancellationToken cancellationToken)
        => Ok(await vehicleService.GetByIdAsync(vehicleId, cancellationToken));

    [HttpGet("by-location/{locationId:int}")]
    [ProducesResponseType<IReadOnlyList<VehicleDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<VehicleDto>>> GetByLocation(int locationId, CancellationToken cancellationToken)
        => Ok(await vehicleService.GetByLocationAsync(locationId, cancellationToken));

    [HttpPost]
    [ProducesResponseType<VehicleDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VehicleDto>> Create(CreateVehicleRequest request, CancellationToken cancellationToken)
    {
        var vehicle = await vehicleService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { vehicleId = vehicle.FleetCode }, vehicle);
    }

    [HttpDelete("{vehicleId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int vehicleId, CancellationToken cancellationToken)
    {
        await vehicleService.DeleteAsync(vehicleId, cancellationToken);
        return NoContent();
    }
}
