using CarRental.Application.DTOs.Requests;
using CarRental.Application.DTOs.Responses;

namespace CarRental.Application.Interfaces.Services;

public interface IPricingService
{
    Task<PriceQuoteResponse> GetQuoteAsync(PriceQuoteRequest request, CancellationToken cancellationToken = default);
}
