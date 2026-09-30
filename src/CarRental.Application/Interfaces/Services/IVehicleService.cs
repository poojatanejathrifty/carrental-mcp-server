using CarRental.Application.DTOs;
using CarRental.Application.DTOs.Requests;

namespace CarRental.Application.Interfaces.Services;

public interface IVehicleService
{
    Task<VehicleDto> GetByIdAsync(int vehicleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VehicleDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VehicleDto>> GetByLocationAsync(int locationId, CancellationToken cancellationToken = default);
    Task<VehicleDto> CreateAsync(CreateVehicleRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int vehicleId, CancellationToken cancellationToken = default);
}
