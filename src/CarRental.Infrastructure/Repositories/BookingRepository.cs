using CarRental.Application.Interfaces.Repositories;
using CarRental.Domain.Entities;
using CarRental.Infrastructure.Persistence;

namespace CarRental.Infrastructure.Repositories;

public class BookingRepository(ISqlDataAccess dataAccess) : IBookingRepository
{
    public async Task<Booking?> GetByIdAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        var booking = await dataAccess.QuerySingleOrDefaultAsync<Booking>(
            StoredProcedures.Booking.GetById,
            new { BookingId = bookingId },
            cancellationToken);

        if (booking is null)
        {
            return null;
        }

        var items = await dataAccess.QueryAsync<BookingItem>(
            StoredProcedures.Booking.GetItemsByBookingId,
            new { BookingId = bookingId },
            cancellationToken);

        booking.Items = items.ToList();
        return booking;
    }

    public async Task<IReadOnlyList<Booking>> GetByCustomerAsync(int customerId, CancellationToken cancellationToken = default)
        => await dataAccess.QueryAsync<Booking>(
            StoredProcedures.Booking.GetByCustomer,
            new { CustomerId = customerId },
            cancellationToken);

    public async Task<int> CreateAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        var bookingId = await dataAccess.ExecuteScalarAsync<int>(
            StoredProcedures.Booking.Create,
            new
            {
                booking.BookingNumber,
                booking.CustomerId,
                booking.PickupLocationId,
                booking.ReturnLocationId,
                booking.PickupDateUtc,
                booking.ReturnDateUtc,
                Status = (int)booking.Status,
                booking.TotalAmount,
                booking.CreatedAtUtc
            },
            cancellationToken);

        foreach (var item in booking.Items)
        {
            await dataAccess.ExecuteAsync(
                StoredProcedures.Booking.CreateItem,
                new
                {
                    BookingId = bookingId,
                    item.VehicleId,
                    item.RatePerDay,
                    item.RentalDays,
                    item.LineTotal
                },
                cancellationToken);
        }

        return bookingId;
    }

    public async Task<bool> UpdateStatusAsync(int bookingId, int status, CancellationToken cancellationToken = default)
    {
        var rows = await dataAccess.ExecuteAsync(
            StoredProcedures.Booking.UpdateStatus,
            new { BookingId = bookingId, Status = status },
            cancellationToken);

        return rows > 0;
    }

    public async Task<bool> CancelAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        var rows = await dataAccess.ExecuteAsync(
            StoredProcedures.Booking.Cancel,
            new { BookingId = bookingId },
            cancellationToken);

        return rows > 0;
    }
}
