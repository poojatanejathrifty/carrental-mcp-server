CREATE OR ALTER PROCEDURE dbo.usp_Vehicle_GetById
    @VehicleId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT VehicleId, VehicleCategoryId, CurrentLocationId, Make, Model, Year,
           LicensePlate, Vin, Color, Odometer, Status
    FROM dbo.Vehicle
    WHERE VehicleId = @VehicleId AND IsActive = 1;
END
GO

-- NOTE: this environment (MPDBTDX) has an existing FleetMaster table instead of a
-- fresh dbo.Vehicle table. All translation from the real schema to the C# Vehicle
-- entity's expected column names happens here via aliases - nothing in C# changes.
-- VehicleCategoryId/CurrentLocationId are hardcoded to 0 as placeholders: FleetMaster
-- has no equivalent columns yet, so location-based availability search will only ever
-- match a search for LocationId = 0 until a real mapping is wired in.
CREATE OR ALTER PROCEDURE dbo.usp_Vehicle_GetAll
AS
BEGIN
    SET NOCOUNT ON;

   SELECT TOP (200)
        FleetCode              AS FleetCode,
		FleetDescription       AS FleetDescription,
        RTRIM(LCN_RevenueLocation) AS CurrentLocationId,
		GM_HireType            AS HireType,
		GRP_GroupCode          AS GroupCode,
        FGM_MakeCode           AS Make,
        FGM_Model              AS Model,
	    VCL_VehicleColor       AS Color,
		FGM_BrandCode          as BrandCode,
        YearModel              AS [Year],
		FGM_EngineCapacity     AS EngineCapacity,
		FGM_VehicleType        AS VehicleType,
        LIC_LicenseCoCode      AS LicensePlate,
        1                      AS Status -- already filtered to idle/available below
    FROM FleetMaster
    WHERE STATUS_StatusCode = 'IDL'
    ORDER BY FleetCode;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Vehicle_GetByLocation
    @LocationId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT VehicleId, VehicleCategoryId, CurrentLocationId, Make, Model, Year,
           LicensePlate, Vin, Color, Odometer, Status
    FROM dbo.Vehicle
    WHERE CurrentLocationId = @LocationId AND IsActive = 1
    ORDER BY VehicleId;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Vehicle_Create
    @VehicleCategoryId INT,
    @CurrentLocationId INT,
    @Make              NVARCHAR(50),
    @Model             NVARCHAR(50),
    @Year              INT,
    @LicensePlate      NVARCHAR(20),
    @Vin               NVARCHAR(17),
    @Color             NVARCHAR(30),
    @Odometer          INT,
    @Status            INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Vehicle (VehicleCategoryId, CurrentLocationId, Make, Model, Year, LicensePlate, Vin, Color, Odometer, Status)
    VALUES (@VehicleCategoryId, @CurrentLocationId, @Make, @Model, @Year, @LicensePlate, @Vin, @Color, @Odometer, @Status);

    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Vehicle_Update
    @VehicleId         INT,
    @VehicleCategoryId INT,
    @CurrentLocationId INT,
    @Make              NVARCHAR(50),
    @Model             NVARCHAR(50),
    @Year              INT,
    @LicensePlate      NVARCHAR(20),
    @Vin               NVARCHAR(17),
    @Color             NVARCHAR(30),
    @Odometer          INT,
    @Status            INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Vehicle
    SET VehicleCategoryId = @VehicleCategoryId,
        CurrentLocationId = @CurrentLocationId,
        Make = @Make,
        Model = @Model,
        Year = @Year,
        LicensePlate = @LicensePlate,
        Vin = @Vin,
        Color = @Color,
        Odometer = @Odometer,
        Status = @Status
    WHERE VehicleId = @VehicleId AND IsActive = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Vehicle_UpdateStatus
    @VehicleId INT,
    @Status    INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Vehicle
    SET Status = @Status
    WHERE VehicleId = @VehicleId AND IsActive = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Vehicle_Delete
    @VehicleId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Vehicle
    SET IsActive = 0
    WHERE VehicleId = @VehicleId AND IsActive = 1;
END
GO
