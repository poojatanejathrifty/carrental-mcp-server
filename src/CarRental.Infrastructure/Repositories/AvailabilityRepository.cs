using CarRental.Application.Interfaces.Repositories;
using CarRental.Domain.Entities;
using CarRental.Infrastructure.Persistence;

namespace CarRental.Infrastructure.Repositories;

public class AvailabilityRepository(ISqlDataAccess dataAccess) : IAvailabilityRepository
{
    public async Task<IReadOnlyList<Vehicle>> GetAvailableVehiclesAsync(
        string? location,
        DateTime? pickupDateUtc,
        DateTime? returnDateUtc,
        string? vehicleCategoryId,
        CancellationToken cancellationToken = default)
        => await dataAccess.QueryAsync<Vehicle>(
            StoredProcedures.Availability.GetAvailableVehicles,
            new
            {
                Location = location,
                PickupDateUtc = pickupDateUtc,
                ReturnDateUtc = returnDateUtc,
                VehicleCategoryId = vehicleCategoryId
            },
            cancellationToken);

    public async Task<bool> IsVehicleAvailableAsync(
        int vehicleId,
        DateTime pickupDateUtc,
        DateTime returnDateUtc,
        CancellationToken cancellationToken = default)
    {
        var result = await dataAccess.ExecuteScalarAsync<int>(
            StoredProcedures.Availability.IsVehicleAvailable,
            new { VehicleId = vehicleId, PickupDateUtc = pickupDateUtc, ReturnDateUtc = returnDateUtc },
            cancellationToken);

        return result == 1;
    }
}
