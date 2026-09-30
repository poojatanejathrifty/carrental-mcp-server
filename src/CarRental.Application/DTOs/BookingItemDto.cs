namespace CarRental.Application.DTOs;

public class BookingItemDto
{
    public int BookingItemId { get; set; }
    public int VehicleId { get; set; }
    public string VehicleDisplayName { get; set; } = string.Empty;
    public decimal RatePerDay { get; set; }
    public int RentalDays { get; set; }
    public decimal LineTotal { get; set; }
}
