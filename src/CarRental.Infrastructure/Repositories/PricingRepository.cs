using CarRental.Application.Interfaces.Repositories;
using CarRental.Domain.Entities;
using CarRental.Infrastructure.Persistence;

namespace CarRental.Infrastructure.Repositories;

public class PricingRepository(ISqlDataAccess dataAccess) : IPricingRepository
{
    public Task<RentalRate?> GetActiveRateAsync(
        int vehicleCategoryId,
        int locationId,
        DateOnly onDate,
        CancellationToken cancellationToken = default)
        => dataAccess.QuerySingleOrDefaultAsync<RentalRate>(
            StoredProcedures.Pricing.GetActiveRate,
            new { VehicleCategoryId = vehicleCategoryId, LocationId = locationId, OnDate = onDate },
            cancellationToken);

    public async Task<IReadOnlyList<RentalRate>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dataAccess.QueryAsync<RentalRate>(StoredProcedures.Pricing.GetAll, cancellationToken: cancellationToken);

    public Task<int> CreateAsync(RentalRate rate, CancellationToken cancellationToken = default)
        => dataAccess.ExecuteScalarAsync<int>(
            StoredProcedures.Pricing.Create,
            new
            {
                rate.VehicleCategoryId,
                rate.LocationId,
                rate.DailyRate,
                rate.WeeklyRate,
                rate.MonthlyRate,
                rate.EffectiveFrom,
                rate.EffectiveTo
            },
            cancellationToken)!;
}
