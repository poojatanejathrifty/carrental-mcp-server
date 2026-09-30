namespace CarRental.Application.DTOs;

public class VehicleDto
{
    public int FleetCode { get; set; }
    public string FleetDescription { get; set; }
    public string CurrentLocationId { get; set; } = string.Empty;
    public string HireType { get; set; }
    public string GroupCode { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
     public string Color { get; set; } = string.Empty;
     
     public string BrandCode { get; set; } = string.Empty;
    public int Year { get; set; }
    public string EngineCapacity { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
   
    public string VehicleType { get; set; }=string.Empty;
    public string Status { get; set; } = string.Empty;
}


