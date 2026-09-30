namespace CarRental.Domain.Entities;

public class Customer
{
    public int CustomerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string DriverLicenseNumber { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public bool IsActive { get; set; } = true;
}
