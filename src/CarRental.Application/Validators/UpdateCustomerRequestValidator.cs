using CarRental.Application.DTOs.Requests;
using FluentValidation;

namespace CarRental.Application.Validators;

public class UpdateCustomerRequestValidator : AbstractValidator<UpdateCustomerRequest>
{
    public UpdateCustomerRequestValidator()
    {
        RuleFor(x => x.CustomerId).GreaterThan(0);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20);
    }
}
