using CarRental.Application.Common;
using CarRental.Application.DTOs;
using CarRental.Application.DTOs.Requests;
using CarRental.Application.Interfaces.Repositories;
using CarRental.Application.Interfaces.Services;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Services;

public class PaymentService(
    IPaymentRepository paymentRepository,
    IBookingRepository bookingRepository,
    ILogger<PaymentService> logger) : IPaymentService
{
    public async Task<PaymentDto> GetByIdAsync(int paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await paymentRepository.GetByIdAsync(paymentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Payment), paymentId);

        return ToDto(payment);
    }

    public async Task<IReadOnlyList<PaymentDto>> GetByBookingAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        var payments = await paymentRepository.GetByBookingAsync(bookingId, cancellationToken);
        return payments.Select(ToDto).ToList();
    }

    public async Task<PaymentDto> ProcessPaymentAsync(CreatePaymentRequest request, CancellationToken cancellationToken = default)
    {
        _ = await bookingRepository.GetByIdAsync(request.BookingId, cancellationToken)
            ?? throw new NotFoundException(nameof(Booking), request.BookingId);

        if (!Enum.TryParse<PaymentMethod>(request.Method, ignoreCase: true, out var method))
        {
            throw new BusinessRuleException($"Unsupported payment method '{request.Method}'.");
        }

        var payment = new Payment
        {
            BookingId = request.BookingId,
            Amount = request.Amount,
            Method = method,
            Status = PaymentStatus.Captured,
            TransactionReference = $"TXN-{Guid.NewGuid():N}",
            CreatedAtUtc = DateTime.UtcNow
        };

        payment.PaymentId = await paymentRepository.CreateAsync(payment, cancellationToken);
        logger.LogInformation("Processed payment {PaymentId} for booking {BookingId}", payment.PaymentId, payment.BookingId);

        return ToDto(payment);
    }

    public async Task<PaymentDto> RefundAsync(int paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await paymentRepository.GetByIdAsync(paymentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Payment), paymentId);

        if (payment.Status != PaymentStatus.Captured)
        {
            throw new BusinessRuleException($"Payment {paymentId} cannot be refunded from status '{payment.Status}'.");
        }

        await paymentRepository.UpdateStatusAsync(paymentId, (int)PaymentStatus.Refunded, payment.TransactionReference, cancellationToken);
        payment.Status = PaymentStatus.Refunded;

        return ToDto(payment);
    }

    private static PaymentDto ToDto(Payment payment) => new()
    {
        PaymentId = payment.PaymentId,
        BookingId = payment.BookingId,
        Amount = payment.Amount,
        Method = payment.Method.ToString(),
        Status = payment.Status.ToString(),
        TransactionReference = payment.TransactionReference,
        CreatedAtUtc = payment.CreatedAtUtc
    };
}
