namespace CarRental.Application.DTOs.Requests;

public class PriceQuoteRequest
{
    public int VehicleCategoryId { get; set; }
    public int LocationId { get; set; }
    public DateTime PickupDateUtc { get; set; }
    public DateTime ReturnDateUtc { get; set; }
}
