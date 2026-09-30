using CarRental.Application.Interfaces.Repositories;
using CarRental.Domain.Entities;
using CarRental.Infrastructure.Persistence;

namespace CarRental.Infrastructure.Repositories;

public class PaymentRepository(ISqlDataAccess dataAccess) : IPaymentRepository
{
    public Task<Payment?> GetByIdAsync(int paymentId, CancellationToken cancellationToken = default)
        => dataAccess.QuerySingleOrDefaultAsync<Payment>(
            StoredProcedures.Payment.GetById,
            new { PaymentId = paymentId },
            cancellationToken);

    public async Task<IReadOnlyList<Payment>> GetByBookingAsync(int bookingId, CancellationToken cancellationToken = default)
        => await dataAccess.QueryAsync<Payment>(
            StoredProcedures.Payment.GetByBooking,
            new { BookingId = bookingId },
            cancellationToken);

    public Task<int> CreateAsync(Payment payment, CancellationToken cancellationToken = default)
        => dataAccess.ExecuteScalarAsync<int>(
            StoredProcedures.Payment.Create,
            new
            {
                payment.BookingId,
                payment.Amount,
                Method = (int)payment.Method,
                Status = (int)payment.Status,
                payment.TransactionReference,
                payment.CreatedAtUtc
            },
            cancellationToken)!;

    public async Task<bool> UpdateStatusAsync(int paymentId, int status, string? transactionReference, CancellationToken cancellationToken = default)
    {
        var rows = await dataAccess.ExecuteAsync(
            StoredProcedures.Payment.UpdateStatus,
            new { PaymentId = paymentId, Status = status, TransactionReference = transactionReference },
            cancellationToken);

        return rows > 0;
    }
}
