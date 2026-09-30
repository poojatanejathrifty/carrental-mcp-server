using CarRental.Application.DTOs;
using CarRental.Application.DTOs.Requests;

namespace CarRental.Application.Interfaces.Services;

public interface IAvailabilityService
{
    Task<IReadOnlyList<VehicleDto>> SearchAvailableVehiclesAsync(AvailabilitySearchRequest request, CancellationToken cancellationToken = default);
    Task<bool> IsVehicleAvailableAsync(int vehicleId, DateTime pickupDateUtc, DateTime returnDateUtc, CancellationToken cancellationToken = default);
}
