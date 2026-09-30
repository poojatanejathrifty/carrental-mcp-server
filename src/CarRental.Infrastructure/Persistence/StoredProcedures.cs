namespace CarRental.Infrastructure.Persistence;

/// <summary>
/// Centralized registry of every stored procedure name used by the data-access layer.
/// Repositories must reference these constants rather than hard-coding SP names.
/// </summary>
public static class StoredProcedures
{
    public static class Vehicle
    {
        public const string GetById = "dbo.usp_Vehicle_GetById";
        public const string GetAll = "dbo.usp_Vehicle_GetAll";
        public const string GetByLocation = "dbo.usp_Vehicle_GetByLocation";
        public const string Create = "dbo.usp_Vehicle_Create";
        public const string Update = "dbo.usp_Vehicle_Update";
        public const string UpdateStatus = "dbo.usp_Vehicle_UpdateStatus";
        public const string Delete = "dbo.usp_Vehicle_Delete";
    }

    public static class Availability
    {
        public const string GetAvailableVehicles = "dbo.usp_Availability_GetAvailableVehicles";
        public const string IsVehicleAvailable = "dbo.usp_Availability_IsVehicleAvailable";
    }

    public static class Location
    {
        public const string GetAll = "dbo.usp_Location_GetAll";
    }

    public static class Pricing
    {
        public const string GetActiveRate = "dbo.usp_RentalRate_GetActive";
        public const string GetAll = "dbo.usp_RentalRate_GetAll";
        public const string Create = "dbo.usp_RentalRate_Create";
    }

    public static class Customer
    {
        public const string GetById = "dbo.usp_Customer_GetById";
        public const string GetByEmail = "dbo.usp_Customer_GetByEmail";
        public const string GetAll = "dbo.usp_Customer_GetAll";
        public const string Create = "dbo.usp_Customer_Create";
        public const string Update = "dbo.usp_Customer_Update";
        public const string Delete = "dbo.usp_Customer_Delete";
    }

    public static class Booking
    {
        public const string GetById = "dbo.usp_Booking_GetById";
        public const string GetItemsByBookingId = "dbo.usp_BookingItem_GetByBookingId";
        public const string GetByCustomer = "dbo.usp_Booking_GetByCustomer";
        public const string Create = "dbo.usp_Booking_Create";
        public const string CreateItem = "dbo.usp_BookingItem_Create";
        public const string UpdateStatus = "dbo.usp_Booking_UpdateStatus";
        public const string Cancel = "dbo.usp_Booking_Cancel";
    }

    public static class Payment
    {
        public const string GetById = "dbo.usp_Payment_GetById";
        public const string GetByBooking = "dbo.usp_Payment_GetByBooking";
        public const string Create = "dbo.usp_Payment_Create";
        public const string UpdateStatus = "dbo.usp_Payment_UpdateStatus";
    }
}
