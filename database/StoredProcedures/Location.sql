-- Confirmed real columns: Code, Description, GM_LocationType (values seen so far:
-- 'R', 'AP' - airport terminals like DA1/DA2/DA3 -, 'OW'). No confirmed business rule yet
-- for which types are valid pickup locations, so no type filter is applied - revisit once
-- that's known, otherwise real locations (like the DA1-DA3 airport terminals) get silently
-- excluded, as happened when this was filtered to GM_LocationType = 'R' only.
CREATE OR ALTER PROCEDURE dbo.usp_Location_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    -- RTRIM: Code/Description are fixed-width columns padded with trailing spaces
    -- (e.g. 'DA3 ') - trim so callers get clean values.
    SELECT RTRIM(Code) AS Code, RTRIM(Description) AS Description
    FROM LOCATION;
END
GO
