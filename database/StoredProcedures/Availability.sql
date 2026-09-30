CREATE OR ALTER PROCEDURE dbo.usp_Availability_GetAvailableVehicles
    @Location          varchar(100) = NULL, -- exact code ('DA3') OR free text ('Dubai Airport Terminal 3', 'Dubai Airport')
    @PickupDateUtc     DATETIME2(3) = NULL,
    @ReturnDateUtc     DATETIME2(3) = NULL,
    @VehicleCategoryId varchar(5)   = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CanBeReRented BIT = 0;
    DECLARE @NotForReplacement BIT = 0;

    -- Resolve @Location into the set of matching location codes, without requiring the
    -- caller to look the code up first:
    --   - an exact code passed straight through ('DA3')                 -> that one code
    --   - an exact description match                                   -> that location
    --   - free text, words together or apart ('Dubai Airport',
    --     'dubai', 'airport')                                          -> every location whose
    --     Description contains ALL of the words in the input, in any order
    --   - free text with a trailing number ('Dubai Airport Terminal 3',
    --     'Dubai Airport 3')                                            -> narrowed further to
    --     the code(s) ending in that number
    -- Limitation: this is a heuristic, not real NLP - it won't handle spelled-out numbers
    -- ("Terminal Three"), abbreviations ("DXB T3"), or misspellings. list_locations remains
    -- available separately for browsing/disambiguating when this doesn't find a match.
    DECLARE @MatchedCodes TABLE (Code varchar(10));

    IF @Location IS NOT NULL
    BEGIN
        DECLARE @Trimmed varchar(100) = LTRIM(RTRIM(@Location));

        INSERT INTO @MatchedCodes (Code)
        SELECT RTRIM(Code) FROM LOCATION
        WHERE Code = @Trimmed OR Description = @Trimmed;

        IF NOT EXISTS (SELECT 1 FROM @MatchedCodes)
        BEGIN
            DECLARE @i INT = LEN(@Trimmed);
            WHILE @i > 0 AND SUBSTRING(@Trimmed, @i, 1) LIKE '[0-9]'
                SET @i = @i - 1;
            DECLARE @TrailingDigits varchar(10) = NULLIF(SUBSTRING(@Trimmed, @i + 1, LEN(@Trimmed) - @i), '');
            DECLARE @BaseText varchar(100) = LTRIM(RTRIM(LEFT(@Trimmed, @i)));

            INSERT INTO @MatchedCodes (Code)
            SELECT RTRIM(L.Code)
            FROM LOCATION L
            WHERE NOT EXISTS (
                    SELECT 1 FROM STRING_SPLIT(@BaseText, ' ') w
                    WHERE LEN(w.value) > 0
                      AND CHARINDEX(w.value, L.Description) = 0
                  )
              AND (@TrailingDigits IS NULL OR RIGHT(RTRIM(L.Code), LEN(@TrailingDigits)) = @TrailingDigits);
        END
    END

    SELECT  FleetCode              AS FleetCode,
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
    FROM FleetMaster F LEFT JOIN AGREEMENTS A ON F.ActiveAgreement = A.AgreementNo AND A.FLT_OriginalFleetID = F.FleetCode AND A.GM_HireTypeID='L'
    WHERE
	   F.STATUS_STATUSCODE in ('LCR','LCI','IDL') -- Available
      AND (@Location IS NULL OR F.LCN_RevenueLocation IN (SELECT Code FROM @MatchedCodes))
	  AND F.CanBeReRented = @CanBeReRented
      AND F.NotForReplacement = @NotForReplacement
      AND (@VehicleCategoryId IS NULL OR F.GRP_GroupCode = @VehicleCategoryId)
	  AND ISNULL(A.GM_AgreementStatus,'') <> 'A'
    -- TODO: @PickupDateUtc/@ReturnDateUtc are accepted but still not used for a real
    -- date-overlap check against AGREEMENTS' actual pickup/return date columns.
    ;
    END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Availability_IsVehicleAvailable
    @VehicleId     INT,
    @PickupDateUtc DATETIME2(3),
    @ReturnDateUtc DATETIME2(3)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CASE WHEN EXISTS (
        SELECT 1
        FROM dbo.Vehicle v
        WHERE v.VehicleId = @VehicleId
          AND v.IsActive = 1
          AND v.Status = 1
          AND NOT EXISTS (
              SELECT 1
              FROM dbo.BookingItem bi
              JOIN dbo.Booking b ON b.BookingId = bi.BookingId
              WHERE bi.VehicleId = v.VehicleId
                AND b.Status IN (1, 2, 3)
                AND b.PickupDateUtc < @ReturnDateUtc
                AND b.ReturnDateUtc > @PickupDateUtc
          )
    ) THEN 1 ELSE 0 END;
END
GO
