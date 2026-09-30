namespace CarRental.Application.DTOs.Requests;

public class CreatePaymentRequest
{
    public int BookingId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
}
