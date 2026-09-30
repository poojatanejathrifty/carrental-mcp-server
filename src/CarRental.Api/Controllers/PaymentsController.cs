using CarRental.Application.DTOs;
using CarRental.Application.DTOs.Requests;
using CarRental.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class PaymentsController(IPaymentService paymentService) : ControllerBase
{
    [HttpGet("{paymentId:int}")]
    [ProducesResponseType<PaymentDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentDto>> GetById(int paymentId, CancellationToken cancellationToken)
        => Ok(await paymentService.GetByIdAsync(paymentId, cancellationToken));

    [HttpGet("by-booking/{bookingId:int}")]
    [ProducesResponseType<IReadOnlyList<PaymentDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PaymentDto>>> GetByBooking(int bookingId, CancellationToken cancellationToken)
        => Ok(await paymentService.GetByBookingAsync(bookingId, cancellationToken));

    [HttpPost]
    [ProducesResponseType<PaymentDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentDto>> Process(CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        var payment = await paymentService.ProcessPaymentAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { paymentId = payment.PaymentId }, payment);
    }

    [HttpPost("{paymentId:int}/refund")]
    [ProducesResponseType<PaymentDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentDto>> Refund(int paymentId, CancellationToken cancellationToken)
        => Ok(await paymentService.RefundAsync(paymentId, cancellationToken));
}
