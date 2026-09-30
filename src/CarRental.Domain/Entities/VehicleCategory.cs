namespace CarRental.Domain.Entities;

public class VehicleCategory
{
    public int VehicleCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SeatingCapacity { get; set; }
    public decimal BaseDailyRate { get; set; }
    public bool IsActive { get; set; } = true;
}
