using CarRental.Application.Common;
using CarRental.Application.DTOs.Requests;
using CarRental.Application.DTOs.Responses;
using CarRental.Application.Interfaces.Repositories;
using CarRental.Application.Interfaces.Services;

namespace CarRental.Application.Services;

public class PricingService(IPricingRepository pricingRepository) : IPricingService
{
    private const decimal TaxRate = 0.05m;

    public async Task<PriceQuoteResponse> GetQuoteAsync(PriceQuoteRequest request, CancellationToken cancellationToken = default)
    {
        var pickupDate = DateOnly.FromDateTime(request.PickupDateUtc);
        var rate = await pricingRepository.GetActiveRateAsync(request.VehicleCategoryId, request.LocationId, pickupDate, cancellationToken)
            ?? throw new BusinessRuleException($"No active rental rate found for category {request.VehicleCategoryId} at location {request.LocationId}.");

        var rentalDays = Math.Max(1, (int)Math.Ceiling((request.ReturnDateUtc - request.PickupDateUtc).TotalDays));
        var subTotal = CalculateSubTotal(rate.DailyRate, rate.WeeklyRate, rate.MonthlyRate, rentalDays);
        var taxAmount = Math.Round(subTotal * TaxRate, 2);

        return new PriceQuoteResponse
        {
            VehicleCategoryId = request.VehicleCategoryId,
            RentalDays = rentalDays,
            RatePerDay = rate.DailyRate,
            SubTotal = subTotal,
            TaxAmount = taxAmount,
            TotalAmount = subTotal + taxAmount
        };
    }

    private static decimal CalculateSubTotal(decimal dailyRate, decimal weeklyRate, decimal monthlyRate, int rentalDays)
    {
        var months = rentalDays / 30;
        var remainingAfterMonths = rentalDays % 30;
        var weeks = remainingAfterMonths / 7;
        var days = remainingAfterMonths % 7;

        return (months * monthlyRate) + (weeks * weeklyRate) + (days * dailyRate);
    }
}
