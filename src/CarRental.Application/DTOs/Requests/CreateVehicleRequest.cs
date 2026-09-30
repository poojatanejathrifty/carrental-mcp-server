namespace CarRental.Application.DTOs.Requests;

public class CreateVehicleRequest
{
    public int VehicleCategoryId { get; set; }
    public int CurrentLocationId { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string Vin { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Odometer { get; set; }
}
