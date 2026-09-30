namespace CarRental.Application.DTOs.Requests;

public class AvailabilitySearchRequest
{
    /// <summary>Exact location code (e.g. "DA3") or free text (e.g. "Dubai Airport Terminal 3", "Dubai Airport").</summary>
    public string? Location { get; set; }
    /// <summary>Omit both dates to just check what's currently at the location, without a date-range check.</summary>
    public DateTime? PickupDateUtc { get; set; }
    public DateTime? ReturnDateUtc { get; set; }
    public string? VehicleCategoryId { get; set; }
}
