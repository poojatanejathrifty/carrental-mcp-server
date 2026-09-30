namespace CarRental.Application.DTOs;

public class VehicleCategoryDto
{
    public int VehicleCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SeatingCapacity { get; set; }
    public decimal BaseDailyRate { get; set; }
}
