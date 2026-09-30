using CarRental.Domain.Entities;

namespace CarRental.Application.Interfaces.Repositories;

public interface IPricingRepository
{
    Task<RentalRate?> GetActiveRateAsync(
        int vehicleCategoryId,
        int locationId,
        DateOnly onDate,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RentalRate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> CreateAsync(RentalRate rate, CancellationToken cancellationToken = default);
}
