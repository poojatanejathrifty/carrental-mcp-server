CREATE OR ALTER PROCEDURE dbo.usp_Payment_GetById
    @PaymentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT PaymentId, BookingId, Amount, Method, Status, TransactionReference, CreatedAtUtc
    FROM dbo.Payment
    WHERE PaymentId = @PaymentId;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Payment_GetByBooking
    @BookingId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT PaymentId, BookingId, Amount, Method, Status, TransactionReference, CreatedAtUtc
    FROM dbo.Payment
    WHERE BookingId = @BookingId
    ORDER BY PaymentId DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Payment_Create
    @BookingId             INT,
    @Amount                DECIMAL(12,2),
    @Method                INT,
    @Status                INT,
    @TransactionReference  NVARCHAR(100) = NULL,
    @CreatedAtUtc          DATETIME2(3)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Payment (BookingId, Amount, Method, Status, TransactionReference, CreatedAtUtc)
    VALUES (@BookingId, @Amount, @Method, @Status, @TransactionReference, @CreatedAtUtc);

    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Payment_UpdateStatus
    @PaymentId             INT,
    @Status                INT,
    @TransactionReference  NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Payment
    SET Status = @Status,
        TransactionReference = COALESCE(@TransactionReference, TransactionReference)
    WHERE PaymentId = @PaymentId;
END
GO
