using CarRental.Application.DTOs;
using CarRental.Application.DTOs.Requests;

namespace CarRental.Application.Interfaces.Services;

public interface IPaymentService
{
    Task<PaymentDto> GetByIdAsync(int paymentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PaymentDto>> GetByBookingAsync(int bookingId, CancellationToken cancellationToken = default);
    Task<PaymentDto> ProcessPaymentAsync(CreatePaymentRequest request, CancellationToken cancellationToken = default);
    Task<PaymentDto> RefundAsync(int paymentId, CancellationToken cancellationToken = default);
}
