CREATE OR ALTER PROCEDURE dbo.usp_RentalRate_GetActive
    @VehicleCategoryId INT,
    @LocationId        INT,
    @OnDate            DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1) RentalRateId, VehicleCategoryId, LocationId, DailyRate, WeeklyRate, MonthlyRate,
           EffectiveFrom, EffectiveTo
    FROM dbo.RentalRate
    WHERE VehicleCategoryId = @VehicleCategoryId
      AND LocationId = @LocationId
      AND IsActive = 1
      AND EffectiveFrom <= @OnDate
      AND (EffectiveTo IS NULL OR EffectiveTo >= @OnDate)
    ORDER BY EffectiveFrom DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_RentalRate_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT RentalRateId, VehicleCategoryId, LocationId, DailyRate, WeeklyRate, MonthlyRate,
           EffectiveFrom, EffectiveTo
    FROM dbo.RentalRate
    WHERE IsActive = 1
    ORDER BY RentalRateId;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_RentalRate_Create
    @VehicleCategoryId INT,
    @LocationId        INT,
    @DailyRate         DECIMAL(10,2),
    @WeeklyRate        DECIMAL(10,2),
    @MonthlyRate       DECIMAL(10,2),
    @EffectiveFrom     DATE,
    @EffectiveTo       DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.RentalRate (VehicleCategoryId, LocationId, DailyRate, WeeklyRate, MonthlyRate, EffectiveFrom, EffectiveTo)
    VALUES (@VehicleCategoryId, @LocationId, @DailyRate, @WeeklyRate, @MonthlyRate, @EffectiveFrom, @EffectiveTo);

    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO
