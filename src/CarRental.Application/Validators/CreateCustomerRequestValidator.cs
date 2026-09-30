using CarRental.Application.DTOs.Requests;
using FluentValidation;

namespace CarRental.Application.Validators;

public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.DriverLicenseNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.DateOfBirth)
            .Must(dob => dob <= DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-18)))
            .WithMessage("Customer must be at least 18 years old.");
    }
}
