using CarRental.Application.Interfaces.Repositories;
using CarRental.Domain.Entities;
using CarRental.Infrastructure.Persistence;

namespace CarRental.Infrastructure.Repositories;

public class LocationRepository(ISqlDataAccess dataAccess) : ILocationRepository
{
    public async Task<IReadOnlyList<Location>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dataAccess.QueryAsync<Location>(StoredProcedures.Location.GetAll, cancellationToken: cancellationToken);
}
