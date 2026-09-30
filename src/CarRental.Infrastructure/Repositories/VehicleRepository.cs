using CarRental.Application.Interfaces.Repositories;
using CarRental.Domain.Entities;
using CarRental.Infrastructure.Persistence;

namespace CarRental.Infrastructure.Repositories;

public class VehicleRepository(ISqlDataAccess dataAccess) : IVehicleRepository
{
    public Task<Vehicle?> GetByIdAsync(int vehicleId, CancellationToken cancellationToken = default)
        => dataAccess.QuerySingleOrDefaultAsync<Vehicle>(
            StoredProcedures.Vehicle.GetById,
            new { VehicleId = vehicleId },
            cancellationToken);

    public async Task<IReadOnlyList<Vehicle>> GetByLocationAsync(int locationId, CancellationToken cancellationToken = default)
        => await dataAccess.QueryAsync<Vehicle>(
            StoredProcedures.Vehicle.GetByLocation,
            new { LocationId = locationId },
            cancellationToken);

    public async Task<IReadOnlyList<Vehicle>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dataAccess.QueryAsync<Vehicle>(StoredProcedures.Vehicle.GetAll, cancellationToken: cancellationToken);

    public Task<int> CreateAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
        => dataAccess.ExecuteScalarAsync<int>(
            StoredProcedures.Vehicle.Create,
            new
            {
                vehicle.VehicleCategoryId,
                vehicle.CurrentLocationId,
                vehicle.Make,
                vehicle.Model,
                vehicle.Year,
                vehicle.LicensePlate,
                vehicle.Vin,
                vehicle.Color,
                vehicle.Odometer,
                Status = (int)vehicle.Status
            },
            cancellationToken)!;

    public async Task<bool> UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        var rows = await dataAccess.ExecuteAsync(
            StoredProcedures.Vehicle.Update,
            new
            {
                vehicle.VehicleId,
                vehicle.VehicleCategoryId,
                vehicle.CurrentLocationId,
                vehicle.Make,
                vehicle.Model,
                vehicle.Year,
                vehicle.LicensePlate,
                vehicle.Vin,
                vehicle.Color,
                vehicle.Odometer,
                Status = (int)vehicle.Status
            },
            cancellationToken);

        return rows > 0;
    }

    public async Task<bool> UpdateStatusAsync(int vehicleId, int status, CancellationToken cancellationToken = default)
    {
        var rows = await dataAccess.ExecuteAsync(
            StoredProcedures.Vehicle.UpdateStatus,
            new { VehicleId = vehicleId, Status = status },
            cancellationToken);

        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int vehicleId, CancellationToken cancellationToken = default)
    {
        var rows = await dataAccess.ExecuteAsync(
            StoredProcedures.Vehicle.Delete,
            new { VehicleId = vehicleId },
            cancellationToken);

        return rows > 0;
    }
}
