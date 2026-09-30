CREATE OR ALTER PROCEDURE dbo.usp_Customer_GetById
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CustomerId, FirstName, LastName, Email, PhoneNumber, DriverLicenseNumber, DateOfBirth
    FROM dbo.Customer
    WHERE CustomerId = @CustomerId AND IsActive = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Customer_GetByEmail
    @Email NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CustomerId, FirstName, LastName, Email, PhoneNumber, DriverLicenseNumber, DateOfBirth
    FROM dbo.Customer
    WHERE Email = @Email AND IsActive = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Customer_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CustomerId, FirstName, LastName, Email, PhoneNumber, DriverLicenseNumber, DateOfBirth
    FROM dbo.Customer
    WHERE IsActive = 1
    ORDER BY CustomerId;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Customer_Create
    @FirstName            NVARCHAR(100),
    @LastName             NVARCHAR(100),
    @Email                NVARCHAR(256),
    @PhoneNumber          NVARCHAR(20),
    @DriverLicenseNumber  NVARCHAR(50),
    @DateOfBirth          DATE,
    @CreatedAtUtc         DATETIME2(3)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Customer (FirstName, LastName, Email, PhoneNumber, DriverLicenseNumber, DateOfBirth, CreatedAtUtc)
    VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @DriverLicenseNumber, @DateOfBirth, @CreatedAtUtc);

    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Customer_Update
    @CustomerId  INT,
    @FirstName   NVARCHAR(100),
    @LastName    NVARCHAR(100),
    @Email       NVARCHAR(256),
    @PhoneNumber NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Customer
    SET FirstName = @FirstName,
        LastName = @LastName,
        Email = @Email,
        PhoneNumber = @PhoneNumber
    WHERE CustomerId = @CustomerId AND IsActive = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Customer_Delete
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Customer
    SET IsActive = 0
    WHERE CustomerId = @CustomerId AND IsActive = 1;
END
GO
