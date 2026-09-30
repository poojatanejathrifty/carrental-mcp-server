namespace CarRental.Domain.Entities;

public class BookingItem
{
    public int BookingItemId { get; set; }
    public int BookingId { get; set; }
    public int VehicleId { get; set; }
    public decimal RatePerDay { get; set; }
    public int RentalDays { get; set; }
    public decimal LineTotal { get; set; }
}
