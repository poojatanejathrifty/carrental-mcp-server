using CarRental.Domain.Enums;

namespace CarRental.Domain.Entities;

public class Vehicle
{
    // Legacy sample-schema fields - still used by VehicleRepository's Create/Update/Delete
    // against the old usp_Vehicle_* procedures. Not yet migrated to FleetMaster.
    public int VehicleId { get; set; }
    public int VehicleCategoryId { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string Vin { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Odometer { get; set; }
    public VehicleStatus Status { get; set; } = VehicleStatus.Available;
    public bool IsActive { get; set; } = true;

    // Real FleetMaster-backed fields, populated by usp_Vehicle_GetAll/usp_Availability_GetAvailableVehicles.
    // CurrentLocationId is a location code (e.g. "DA3"), not a numeric id - keep it string.
    public int FleetCode { get; set; }
    public string FleetDescription { get; set; } = string.Empty;
    public string CurrentLocationId { get; set; } = string.Empty;
    public string HireType { get; set; } = string.Empty;
    public string GroupCode { get; set; } = string.Empty;
    public string BrandCode { get; set; } = string.Empty;
    public string EngineCapacity { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
}
