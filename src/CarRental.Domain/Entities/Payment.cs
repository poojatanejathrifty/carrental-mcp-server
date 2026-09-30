using CarRental.Domain.Enums;

namespace CarRental.Domain.Entities;

public class Payment
{
    public int PaymentId { get; set; }
    public int BookingId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? TransactionReference { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
