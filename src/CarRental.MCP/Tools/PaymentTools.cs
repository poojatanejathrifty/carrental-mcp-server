using System.ComponentModel;
using CarRental.Application.DTOs;
using CarRental.Application.DTOs.Requests;
using CarRental.Application.Interfaces.Services;
using ModelContextProtocol.Server;

namespace CarRental.MCP.Tools;

[McpServerToolType]
public static class PaymentTools
{
    [McpServerTool(Name = "get_payment"), Description("Gets a single payment by id.")]
    public static Task<PaymentDto> GetPayment(IPaymentService paymentService, int paymentId, CancellationToken cancellationToken)
        => paymentService.GetByIdAsync(paymentId, cancellationToken);

    [McpServerTool(Name = "list_payments_by_booking"), Description("Lists all payments recorded against a booking.")]
    public static Task<IReadOnlyList<PaymentDto>> ListPaymentsByBooking(IPaymentService paymentService, int bookingId, CancellationToken cancellationToken)
        => paymentService.GetByBookingAsync(bookingId, cancellationToken);

    [McpServerTool(Name = "process_payment"), Description("Processes a payment for a booking.")]
    public static Task<PaymentDto> ProcessPayment(IPaymentService paymentService, CreatePaymentRequest request, CancellationToken cancellationToken)
        => paymentService.ProcessPaymentAsync(request, cancellationToken);

    [McpServerTool(Name = "refund_payment"), Description("Refunds a previously captured payment.")]
    public static Task<PaymentDto> RefundPayment(IPaymentService paymentService, int paymentId, CancellationToken cancellationToken)
        => paymentService.RefundAsync(paymentId, cancellationToken);
}
