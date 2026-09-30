using CarRental.Domain.Entities;

namespace CarRental.Application.Interfaces.Repositories;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(int vehicleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Vehicle>> GetByLocationAsync(int locationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Vehicle>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> CreateAsync(Vehicle vehicle, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default);
    Task<bool> UpdateStatusAsync(int vehicleId, int status, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int vehicleId, CancellationToken cancellationToken = default);
}
