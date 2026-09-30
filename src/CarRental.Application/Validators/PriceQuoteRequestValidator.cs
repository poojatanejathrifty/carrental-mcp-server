using CarRental.Application.DTOs.Requests;
using FluentValidation;

namespace CarRental.Application.Validators;

public class PriceQuoteRequestValidator : AbstractValidator<PriceQuoteRequest>
{
    public PriceQuoteRequestValidator()
    {
        RuleFor(x => x.VehicleCategoryId).GreaterThan(0);
        RuleFor(x => x.LocationId).GreaterThan(0);
        RuleFor(x => x.PickupDateUtc).LessThan(x => x.ReturnDateUtc);
    }
}
