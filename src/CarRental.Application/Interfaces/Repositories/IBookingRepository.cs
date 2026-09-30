using CarRental.Domain.Entities;

namespace CarRental.Application.Interfaces.Repositories;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(int bookingId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Booking>> GetByCustomerAsync(int customerId, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(Booking booking, CancellationToken cancellationToken = default);
    Task<bool> UpdateStatusAsync(int bookingId, int status, CancellationToken cancellationToken = default);
    Task<bool> CancelAsync(int bookingId, CancellationToken cancellationToken = default);
}
