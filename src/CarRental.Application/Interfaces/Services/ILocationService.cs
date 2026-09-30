using CarRental.Application.DTOs;

namespace CarRental.Application.Interfaces.Services;

public interface ILocationService
{
    Task<IReadOnlyList<LocationDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
