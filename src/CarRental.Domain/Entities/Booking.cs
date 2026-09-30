using CarRental.Domain.Enums;

namespace CarRental.Domain.Entities;

public class Booking
{
    public int BookingId { get; set; }
    public string BookingNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public int PickupLocationId { get; set; }
    public int ReturnLocationId { get; set; }
    public DateTime PickupDateUtc { get; set; }
    public DateTime ReturnDateUtc { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public List<BookingItem> Items { get; set; } = [];
}
