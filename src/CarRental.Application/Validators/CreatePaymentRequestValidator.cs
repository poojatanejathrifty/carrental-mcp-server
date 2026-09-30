using CarRental.Application.DTOs.Requests;
using FluentValidation;

namespace CarRental.Application.Validators;

public class CreatePaymentRequestValidator : AbstractValidator<CreatePaymentRequest>
{
    public CreatePaymentRequestValidator()
    {
        RuleFor(x => x.BookingId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Method).NotEmpty()
            .Must(m => Enum.TryParse<Domain.Enums.PaymentMethod>(m, true, out _))
            .WithMessage("Invalid payment method.");
    }
}
