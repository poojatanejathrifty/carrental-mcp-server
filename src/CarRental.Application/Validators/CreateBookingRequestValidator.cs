using CarRental.Application.DTOs.Requests;
using FluentValidation;

namespace CarRental.Application.Validators;

public class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
{
    public CreateBookingRequestValidator()
    {
        RuleFor(x => x.CustomerId).GreaterThan(0);
        RuleFor(x => x.PickupLocationId).GreaterThan(0);
        RuleFor(x => x.ReturnLocationId).GreaterThan(0);
        RuleFor(x => x.PickupDateUtc).LessThan(x => x.ReturnDateUtc);
        RuleFor(x => x.PickupDateUtc).GreaterThanOrEqualTo(DateTime.UtcNow.AddMinutes(-5));
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.VehicleId).GreaterThan(0);
        });
    }
}
