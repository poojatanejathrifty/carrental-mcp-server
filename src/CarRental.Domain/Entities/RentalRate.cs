namespace CarRental.Domain.Entities;

public class RentalRate
{
    public int RentalRateId { get; set; }
    public int VehicleCategoryId { get; set; }
    public int LocationId { get; set; }
    public decimal DailyRate { get; set; }
    public decimal WeeklyRate { get; set; }
    public decimal MonthlyRate { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;
}
