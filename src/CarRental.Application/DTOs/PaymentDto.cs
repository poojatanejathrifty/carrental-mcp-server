namespace CarRental.Application.DTOs;

public class PaymentDto
{
    public int PaymentId { get; set; }
    public int BookingId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? TransactionReference { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
