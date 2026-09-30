using CarRental.Application.Common;
using CarRental.Application.DTOs;
using CarRental.Application.DTOs.Requests;
using CarRental.Application.Interfaces.Repositories;
using CarRental.Application.Interfaces.Services;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Services;

public class BookingService(
    IBookingRepository bookingRepository,
    IAvailabilityRepository availabilityRepository,
    IPricingRepository pricingRepository,
    IVehicleRepository vehicleRepository,
    ILogger<BookingService> logger) : IBookingService
{
    public async Task<BookingDto> GetByIdAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(bookingId, cancellationToken)
            ?? throw new NotFoundException(nameof(Booking), bookingId);

        return ToDto(booking);
    }

    public async Task<IReadOnlyList<BookingDto>> GetByCustomerAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var bookings = await bookingRepository.GetByCustomerAsync(customerId, cancellationToken);
        return bookings.Select(ToDto).ToList();
    }

    public async Task<BookingDto> CreateAsync(CreateBookingRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Items.Count == 0)
        {
            throw new BusinessRuleException("A booking requires at least one vehicle.");
        }

        var rentalDays = Math.Max(1, (int)Math.Ceiling((request.ReturnDateUtc - request.PickupDateUtc).TotalDays));
        var pickupDate = DateOnly.FromDateTime(request.PickupDateUtc);

        var booking = new Booking
        {
            BookingNumber = $"BK-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}",
            CustomerId = request.CustomerId,
            PickupLocationId = request.PickupLocationId,
            ReturnLocationId = request.ReturnLocationId,
            PickupDateUtc = request.PickupDateUtc,
            ReturnDateUtc = request.ReturnDateUtc,
            Status = BookingStatus.Pending,
            CreatedAtUtc = DateTime.UtcNow
        };

        foreach (var item in request.Items)
        {
            var vehicle = await vehicleRepository.GetByIdAsync(item.VehicleId, cancellationToken)
                ?? throw new NotFoundException(nameof(Vehicle), item.VehicleId);

            var isAvailable = await availabilityRepository.IsVehicleAvailableAsync(
                item.VehicleId, request.PickupDateUtc, request.ReturnDateUtc, cancellationToken);

            if (!isAvailable)
            {
                throw new BusinessRuleException($"Vehicle {item.VehicleId} is not available for the requested dates.");
            }

            var rate = await pricingRepository.GetActiveRateAsync(vehicle.VehicleCategoryId, request.PickupLocationId, pickupDate, cancellationToken)
                ?? throw new BusinessRuleException($"No active rental rate found for vehicle {item.VehicleId}.");

            var lineTotal = rate.DailyRate * rentalDays;

            booking.Items.Add(new BookingItem
            {
                VehicleId = item.VehicleId,
                RatePerDay = rate.DailyRate,
                RentalDays = rentalDays,
                LineTotal = lineTotal
            });
        }

        booking.TotalAmount = booking.Items.Sum(i => i.LineTotal);
        booking.BookingId = await bookingRepository.CreateAsync(booking, cancellationToken);

        logger.LogInformation("Created booking {BookingNumber} for customer {CustomerId}", booking.BookingNumber, booking.CustomerId);

        return ToDto(booking);
    }

    public async Task<BookingDto> CancelAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(bookingId, cancellationToken)
            ?? throw new NotFoundException(nameof(Booking), bookingId);

        if (booking.Status is BookingStatus.Completed or BookingStatus.Cancelled)
        {
            throw new BusinessRuleException($"Booking {bookingId} cannot be cancelled from status '{booking.Status}'.");
        }

        await bookingRepository.CancelAsync(bookingId, cancellationToken);
        booking.Status = BookingStatus.Cancelled;

        return ToDto(booking);
    }

    private static BookingDto ToDto(Booking booking) => new()
    {
        BookingId = booking.BookingId,
        BookingNumber = booking.BookingNumber,
        CustomerId = booking.CustomerId,
        PickupLocationId = booking.PickupLocationId,
        ReturnLocationId = booking.ReturnLocationId,
        PickupDateUtc = booking.PickupDateUtc,
        ReturnDateUtc = booking.ReturnDateUtc,
        Status = booking.Status.ToString(),
        TotalAmount = booking.TotalAmount,
        Items = booking.Items.Select(i => new BookingItemDto
        {
            BookingItemId = i.BookingItemId,
            VehicleId = i.VehicleId,
            RatePerDay = i.RatePerDay,
            RentalDays = i.RentalDays,
            LineTotal = i.LineTotal
        }).ToList()
    };
}
