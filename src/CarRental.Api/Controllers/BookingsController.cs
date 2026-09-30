using CarRental.Application.DTOs;
using CarRental.Application.DTOs.Requests;
using CarRental.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class BookingsController(IBookingService bookingService) : ControllerBase
{
    [HttpGet("{bookingId:int}")]
    [ProducesResponseType<BookingDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingDto>> GetById(int bookingId, CancellationToken cancellationToken)
        => Ok(await bookingService.GetByIdAsync(bookingId, cancellationToken));

    [HttpGet("by-customer/{customerId:int}")]
    [ProducesResponseType<IReadOnlyList<BookingDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BookingDto>>> GetByCustomer(int customerId, CancellationToken cancellationToken)
        => Ok(await bookingService.GetByCustomerAsync(customerId, cancellationToken));

    [HttpPost]
    [ProducesResponseType<BookingDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookingDto>> Create(CreateBookingRequest request, CancellationToken cancellationToken)
    {
        var booking = await bookingService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { bookingId = booking.BookingId }, booking);
    }

    [HttpPost("{bookingId:int}/cancel")]
    [ProducesResponseType<BookingDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookingDto>> Cancel(int bookingId, CancellationToken cancellationToken)
        => Ok(await bookingService.CancelAsync(bookingId, cancellationToken));
}
