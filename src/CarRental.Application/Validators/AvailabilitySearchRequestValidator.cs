using CarRental.Application.DTOs.Requests;
using FluentValidation;

namespace CarRental.Application.Validators;

public class AvailabilitySearchRequestValidator : AbstractValidator<AvailabilitySearchRequest>
{
    public AvailabilitySearchRequestValidator()
    {
        When(x => x.PickupDateUtc.HasValue && x.ReturnDateUtc.HasValue, () =>
        {
            RuleFor(x => x.PickupDateUtc).LessThan(x => x.ReturnDateUtc);
            RuleFor(x => x.ReturnDateUtc).GreaterThan(DateTime.UtcNow);
        });
    }
}
