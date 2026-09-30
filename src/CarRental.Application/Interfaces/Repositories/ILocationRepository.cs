using CarRental.Domain.Entities;

namespace CarRental.Application.Interfaces.Repositories;

public interface ILocationRepository
{
    Task<IReadOnlyList<Location>> GetAllAsync(CancellationToken cancellationToken = default);
}
