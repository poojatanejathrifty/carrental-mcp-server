using CarRental.Domain.Entities;

namespace CarRental.Application.Interfaces.Repositories;

public interface IAvailabilityRepository
{
    Task<IReadOnlyList<Vehicle>> GetAvailableVehiclesAsync(
        string? location,
        DateTime? pickupDateUtc,
        DateTime? returnDateUtc,
        string? vehicleCategoryId,
        CancellationToken cancellationToken = default);

    Task<bool> IsVehicleAvailableAsync(
        int vehicleId,
        DateTime pickupDateUtc,
        DateTime returnDateUtc,
        CancellationToken cancellationToken = default);
}
