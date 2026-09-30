CREATE OR ALTER PROCEDURE dbo.usp_Booking_GetById
    @BookingId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT BookingId, BookingNumber, CustomerId, PickupLocationId, ReturnLocationId,
           PickupDateUtc, ReturnDateUtc, Status, TotalAmount, CreatedAtUtc
    FROM dbo.Booking
    WHERE BookingId = @BookingId;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_BookingItem_GetByBookingId
    @BookingId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT BookingItemId, BookingId, VehicleId, RatePerDay, RentalDays, LineTotal
    FROM dbo.BookingItem
    WHERE BookingId = @BookingId
    ORDER BY BookingItemId;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Booking_GetByCustomer
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT BookingId, BookingNumber, CustomerId, PickupLocationId, ReturnLocationId,
           PickupDateUtc, ReturnDateUtc, Status, TotalAmount, CreatedAtUtc
    FROM dbo.Booking
    WHERE CustomerId = @CustomerId
    ORDER BY BookingId DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Booking_Create
    @BookingNumber      NVARCHAR(40),
    @CustomerId         INT,
    @PickupLocationId   INT,
    @ReturnLocationId   INT,
    @PickupDateUtc      DATETIME2(3),
    @ReturnDateUtc      DATETIME2(3),
    @Status             INT,
    @TotalAmount        DECIMAL(12,2),
    @CreatedAtUtc       DATETIME2(3)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Booking (BookingNumber, CustomerId, PickupLocationId, ReturnLocationId, PickupDateUtc, ReturnDateUtc, Status, TotalAmount, CreatedAtUtc)
    VALUES (@BookingNumber, @CustomerId, @PickupLocationId, @ReturnLocationId, @PickupDateUtc, @ReturnDateUtc, @Status, @TotalAmount, @CreatedAtUtc);

    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_BookingItem_Create
    @BookingId  INT,
    @VehicleId  INT,
    @RatePerDay DECIMAL(10,2),
    @RentalDays INT,
    @LineTotal  DECIMAL(12,2)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.BookingItem (BookingId, VehicleId, RatePerDay, RentalDays, LineTotal)
    VALUES (@BookingId, @VehicleId, @RatePerDay, @RentalDays, @LineTotal);

    UPDATE dbo.Vehicle SET Status = 2 WHERE VehicleId = @VehicleId; -- Rented

    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Booking_UpdateStatus
    @BookingId INT,
    @Status    INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Booking
    SET Status = @Status
    WHERE BookingId = @BookingId;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Booking_Cancel
    @BookingId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Booking
    SET Status = 5 -- Cancelled
    WHERE BookingId = @BookingId;

    UPDATE v
    SET v.Status = 1 -- Available
    FROM dbo.Vehicle v
    JOIN dbo.BookingItem bi ON bi.VehicleId = v.VehicleId
    WHERE bi.BookingId = @BookingId;
END
GO
