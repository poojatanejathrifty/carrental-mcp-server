using CarRental.Application.DTOs;
using CarRental.Application.DTOs.Requests;

namespace CarRental.Application.Interfaces.Services;

public interface IBookingService
{
    Task<BookingDto> GetByIdAsync(int bookingId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookingDto>> GetByCustomerAsync(int customerId, CancellationToken cancellationToken = default);
    Task<BookingDto> CreateAsync(CreateBookingRequest request, CancellationToken cancellationToken = default);
    Task<BookingDto> CancelAsync(int bookingId, CancellationToken cancellationToken = default);
}
