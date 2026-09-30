namespace CarRental.Application.DTOs.Responses;

public class PriceQuoteResponse
{
    public int VehicleCategoryId { get; set; }
    public int RentalDays { get; set; }
    public decimal RatePerDay { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
}
