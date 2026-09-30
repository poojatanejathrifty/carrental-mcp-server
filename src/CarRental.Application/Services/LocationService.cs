using CarRental.Application.DTOs;
using CarRental.Application.Interfaces.Repositories;
using CarRental.Application.Interfaces.Services;
using CarRental.Domain.Entities;

namespace CarRental.Application.Services;

public class LocationService(ILocationRepository locationRepository) : ILocationService
{
    public async Task<IReadOnlyList<LocationDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var locations = await locationRepository.GetAllAsync(cancellationToken);
        return locations.Select(ToDto).ToList();
    }

    private static LocationDto ToDto(Location location) => new()
    {
        Code = location.Code,
        Description = location.Description
    };
}
