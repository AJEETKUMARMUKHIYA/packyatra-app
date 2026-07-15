USE [MoversPackers]
GO

/****** Object:  StoredProcedure [dbo].[UpdateBookingAndTicketFlexible]    Script Date: 15-07-2026 16:36:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE   PROCEDURE [dbo].[UpdateBookingAndTicketFlexible]
    @BookingID INT,
    @TotalAmount DECIMAL(18,2) = NULL,
    @BookingStatus NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    ----------------------------------------------------
    -- ❌ Prevent Empty Update Call
    ----------------------------------------------------
    IF @TotalAmount IS NULL AND @BookingStatus IS NULL
    BEGIN
        SELECT 0 AS Success, 'No values provided to update.' AS Message;
        RETURN;
    END

    ----------------------------------------------------
    -- ❌ Validate Booking Exists
    ----------------------------------------------------
    IF NOT EXISTS (SELECT 1 FROM Booking WHERE BookingID = @BookingID)
    BEGIN
        SELECT 0 AS Success, 'Invalid BookingID.' AS Message;
        RETURN;
    END

    BEGIN TRY
        BEGIN TRANSACTION;

        ----------------------------------------------------
        -- ✅ Update Booking
        ----------------------------------------------------
        UPDATE Booking
        SET 
            BookingAmountPaid = ISNULL(@TotalAmount, BookingAmountPaid),
            Status = ISNULL(@BookingStatus, Status)
            --UpdatedAt = GETUTCDATE()
        WHERE BookingID = @BookingID;

        ----------------------------------------------------
        -- ✅ Update Ticket Only If Exists & Status Provided
        ----------------------------------------------------
        IF @BookingStatus IS NOT NULL
           AND EXISTS (SELECT 1 FROM Tickets WHERE BookingID = @BookingID)
        BEGIN
            UPDATE Tickets
            SET 
                Status = @BookingStatus
                --UpdatedAt = GETUTCDATE()
            WHERE BookingID = @BookingID;
        END

        COMMIT TRANSACTION;

        SELECT 1 AS Success, 
               'Booking updated successfully. Ticket updated if available.' AS Message;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

        SELECT 0 AS Success,
               ERROR_MESSAGE() AS Message;
    END CATCH
END
GO

