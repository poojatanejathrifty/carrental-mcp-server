using System.ComponentModel;
using CarRental.Application.DTOs;
using CarRental.Application.DTOs.Requests;
using CarRental.Application.Interfaces.Services;
using ModelContextProtocol.Server;

namespace CarRental.MCP.Tools;

[McpServerToolType]
public static class BookingTools
{
    [McpServerTool(Name = "get_booking"), Description("Gets a single booking, including its vehicle line items, by id.")]
    public static Task<BookingDto> GetBooking(IBookingService bookingService, int bookingId, CancellationToken cancellationToken)
        => bookingService.GetByIdAsync(bookingId, cancellationToken);

    [McpServerTool(Name = "list_bookings_by_customer"), Description("Lists all bookings for a given customer.")]
    public static Task<IReadOnlyList<BookingDto>> ListBookingsByCustomer(IBookingService bookingService, int customerId, CancellationToken cancellationToken)
        => bookingService.GetByCustomerAsync(customerId, cancellationToken);

    [McpServerTool(Name = "create_booking"), Description("Creates a new car rental booking for one or more vehicles.")]
    public static Task<BookingDto> CreateBooking(IBookingService bookingService, CreateBookingRequest request, CancellationToken cancellationToken)
        => bookingService.CreateAsync(request, cancellationToken);

    [McpServerTool(Name = "cancel_booking"), Description("Cancels an existing booking.")]
    public static Task<BookingDto> CancelBooking(IBookingService bookingService, int bookingId, CancellationToken cancellationToken)
        => bookingService.CancelAsync(bookingId, cancellationToken);
}
