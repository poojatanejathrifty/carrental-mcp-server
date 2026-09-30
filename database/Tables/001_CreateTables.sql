-- CarRental database schema
-- Run once against an empty database before deploying stored procedures.

IF DB_ID('CarRentalDb') IS NULL
BEGIN
    PRINT 'Run this script against the CarRentalDb database.';
END
GO

CREATE TABLE dbo.VehicleCategory
(
    VehicleCategoryId  INT IDENTITY(1,1) PRIMARY KEY,
    Name               NVARCHAR(100)   NOT NULL,
    Description        NVARCHAR(500)   NULL,
    SeatingCapacity    INT             NOT NULL,
    BaseDailyRate      DECIMAL(10,2)   NOT NULL,
    IsActive           BIT             NOT NULL CONSTRAINT DF_VehicleCategory_IsActive DEFAULT (1)
);
GO

CREATE TABLE dbo.Location
(
    LocationId    INT IDENTITY(1,1) PRIMARY KEY,
    Name          NVARCHAR(150)  NOT NULL,
    AddressLine   NVARCHAR(250)  NOT NULL,
    City          NVARCHAR(100)  NOT NULL,
    Country       NVARCHAR(100)  NOT NULL,
    PhoneNumber   NVARCHAR(20)   NULL,
    IsActive      BIT            NOT NULL CONSTRAINT DF_Location_IsActive DEFAULT (1)
);
GO

CREATE TABLE dbo.Vehicle
(
    VehicleId          INT IDENTITY(1,1) PRIMARY KEY,
    VehicleCategoryId  INT             NOT NULL CONSTRAINT FK_Vehicle_VehicleCategory REFERENCES dbo.VehicleCategory (VehicleCategoryId),
    CurrentLocationId  INT             NOT NULL CONSTRAINT FK_Vehicle_Location REFERENCES dbo.Location (LocationId),
    Make               NVARCHAR(50)    NOT NULL,
    Model              NVARCHAR(50)    NOT NULL,
    Year               INT             NOT NULL,
    LicensePlate       NVARCHAR(20)    NOT NULL,
    Vin                NVARCHAR(17)    NOT NULL,
    Color              NVARCHAR(30)    NOT NULL,
    Odometer           INT             NOT NULL CONSTRAINT DF_Vehicle_Odometer DEFAULT (0),
    Status             INT             NOT NULL CONSTRAINT DF_Vehicle_Status DEFAULT (1), -- 1=Available 2=Rented 3=Maintenance 4=OutOfService
    IsActive           BIT             NOT NULL CONSTRAINT DF_Vehicle_IsActive DEFAULT (1),
    CONSTRAINT UQ_Vehicle_LicensePlate UNIQUE (LicensePlate),
    CONSTRAINT UQ_Vehicle_Vin UNIQUE (Vin)
);
GO

CREATE TABLE dbo.Customer
(
    CustomerId            INT IDENTITY(1,1) PRIMARY KEY,
    FirstName             NVARCHAR(100)  NOT NULL,
    LastName              NVARCHAR(100)  NOT NULL,
    Email                 NVARCHAR(256)  NOT NULL,
    PhoneNumber           NVARCHAR(20)   NOT NULL,
    DriverLicenseNumber   NVARCHAR(50)   NOT NULL,
    DateOfBirth           DATE           NOT NULL,
    CreatedAtUtc          DATETIME2(3)   NOT NULL CONSTRAINT DF_Customer_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
    IsActive              BIT            NOT NULL CONSTRAINT DF_Customer_IsActive DEFAULT (1),
    CONSTRAINT UQ_Customer_Email UNIQUE (Email)
);
GO

CREATE TABLE dbo.RentalRate
(
    RentalRateId       INT IDENTITY(1,1) PRIMARY KEY,
    VehicleCategoryId  INT             NOT NULL CONSTRAINT FK_RentalRate_VehicleCategory REFERENCES dbo.VehicleCategory (VehicleCategoryId),
    LocationId         INT             NOT NULL CONSTRAINT FK_RentalRate_Location REFERENCES dbo.Location (LocationId),
    DailyRate          DECIMAL(10,2)   NOT NULL,
    WeeklyRate         DECIMAL(10,2)   NOT NULL,
    MonthlyRate        DECIMAL(10,2)   NOT NULL,
    EffectiveFrom      DATE            NOT NULL,
    EffectiveTo        DATE            NULL,
    IsActive           BIT             NOT NULL CONSTRAINT DF_RentalRate_IsActive DEFAULT (1)
);
GO

CREATE TABLE dbo.Booking
(
    BookingId          INT IDENTITY(1,1) PRIMARY KEY,
    BookingNumber      NVARCHAR(40)    NOT NULL,
    CustomerId         INT             NOT NULL CONSTRAINT FK_Booking_Customer REFERENCES dbo.Customer (CustomerId),
    PickupLocationId   INT             NOT NULL CONSTRAINT FK_Booking_PickupLocation REFERENCES dbo.Location (LocationId),
    ReturnLocationId   INT             NOT NULL CONSTRAINT FK_Booking_ReturnLocation REFERENCES dbo.Location (LocationId),
    PickupDateUtc      DATETIME2(3)    NOT NULL,
    ReturnDateUtc      DATETIME2(3)    NOT NULL,
    Status             INT             NOT NULL CONSTRAINT DF_Booking_Status DEFAULT (1), -- 1=Pending 2=Confirmed 3=CheckedOut 4=Completed 5=Cancelled
    TotalAmount        DECIMAL(12,2)   NOT NULL CONSTRAINT DF_Booking_TotalAmount DEFAULT (0),
    CreatedAtUtc       DATETIME2(3)    NOT NULL CONSTRAINT DF_Booking_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT UQ_Booking_BookingNumber UNIQUE (BookingNumber)
);
GO

CREATE TABLE dbo.BookingItem
(
    BookingItemId  INT IDENTITY(1,1) PRIMARY KEY,
    BookingId      INT             NOT NULL CONSTRAINT FK_BookingItem_Booking REFERENCES dbo.Booking (BookingId),
    VehicleId      INT             NOT NULL CONSTRAINT FK_BookingItem_Vehicle REFERENCES dbo.Vehicle (VehicleId),
    RatePerDay     DECIMAL(10,2)   NOT NULL,
    RentalDays     INT             NOT NULL,
    LineTotal      DECIMAL(12,2)   NOT NULL
);
GO

CREATE TABLE dbo.Payment
(
    PaymentId             INT IDENTITY(1,1) PRIMARY KEY,
    BookingId             INT             NOT NULL CONSTRAINT FK_Payment_Booking REFERENCES dbo.Booking (BookingId),
    Amount                DECIMAL(12,2)   NOT NULL,
    Method                INT             NOT NULL, -- 1=CreditCard 2=DebitCard 3=Cash 4=BankTransfer 5=Wallet
    Status                INT             NOT NULL, -- 1=Pending 2=Authorized 3=Captured 4=Failed 5=Refunded
    TransactionReference  NVARCHAR(100)   NULL,
    CreatedAtUtc          DATETIME2(3)    NOT NULL CONSTRAINT DF_Payment_CreatedAtUtc DEFAULT (SYSUTCDATETIME())
);
GO

CREATE INDEX IX_Vehicle_CurrentLocationId ON dbo.Vehicle (CurrentLocationId);
CREATE INDEX IX_Vehicle_VehicleCategoryId ON dbo.Vehicle (VehicleCategoryId);
CREATE INDEX IX_RentalRate_Category_Location ON dbo.RentalRate (VehicleCategoryId, LocationId, EffectiveFrom);
CREATE INDEX IX_Booking_CustomerId ON dbo.Booking (CustomerId);
CREATE INDEX IX_BookingItem_BookingId ON dbo.BookingItem (BookingId);
CREATE INDEX IX_BookingItem_VehicleId ON dbo.BookingItem (VehicleId);
CREATE INDEX IX_Payment_BookingId ON dbo.Payment (BookingId);
GO
