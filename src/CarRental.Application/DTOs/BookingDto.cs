namespace CarRental.Application.DTOs;

public class BookingDto
{
    public int BookingId { get; set; }
    public string BookingNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public int PickupLocationId { get; set; }
    public int ReturnLocationId { get; set; }
    public DateTime PickupDateUtc { get; set; }
    public DateTime ReturnDateUtc { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public List<BookingItemDto> Items { get; set; } = [];
}
