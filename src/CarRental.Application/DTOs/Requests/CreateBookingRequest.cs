namespace CarRental.Application.DTOs.Requests;

public class CreateBookingRequest
{
    public int CustomerId { get; set; }
    public int PickupLocationId { get; set; }
    public int ReturnLocationId { get; set; }
    public DateTime PickupDateUtc { get; set; }
    public DateTime ReturnDateUtc { get; set; }
    public List<CreateBookingItemRequest> Items { get; set; } = [];
}

public class CreateBookingItemRequest
{
    public int VehicleId { get; set; }
}
