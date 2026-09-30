using CarRental.Domain.Entities;

namespace CarRental.Application.Interfaces.Repositories;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(int paymentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payment>> GetByBookingAsync(int bookingId, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(Payment payment, CancellationToken cancellationToken = default);
    Task<bool> UpdateStatusAsync(int paymentId, int status, string? transactionReference, CancellationToken cancellationToken = default);
}
