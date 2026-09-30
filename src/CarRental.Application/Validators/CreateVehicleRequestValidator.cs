using CarRental.Application.DTOs.Requests;
using FluentValidation;

namespace CarRental.Application.Validators;

public class CreateVehicleRequestValidator : AbstractValidator<CreateVehicleRequest>
{
    public CreateVehicleRequestValidator()
    {
        RuleFor(x => x.VehicleCategoryId).GreaterThan(0);
        RuleFor(x => x.CurrentLocationId).GreaterThan(0);
        RuleFor(x => x.Make).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Year).InclusiveBetween(1980, DateTime.UtcNow.Year + 1);
        RuleFor(x => x.LicensePlate).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Vin).NotEmpty().Length(17);
        RuleFor(x => x.Color).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Odometer).GreaterThanOrEqualTo(0);
    }
}
