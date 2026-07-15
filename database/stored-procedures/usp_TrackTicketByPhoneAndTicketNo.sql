USE [MoversPackers]
GO

/****** Object:  StoredProcedure [dbo].[usp_TrackTicketByPhoneAndTicketNo]    Script Date: 15-07-2026 16:39:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_TrackTicketByPhoneAndTicketNo]
(
    @PhoneNumber VARCHAR(15),
    @TicketNo VARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    /* STEP 1: Validate User */
    DECLARE @UserID INT;

    SELECT @UserID = UserID
    FROM dbo.Users
    WHERE PhoneNumber = @PhoneNumber;

    IF @UserID IS NULL
    BEGIN
        RAISERROR ('Invalid phone number', 16, 1);
        RETURN;
    END

    /* STEP 2: Validate Ticket */
    DECLARE @TicketID INT;
    DECLARE @BookingID INT;

    SELECT 
        @TicketID = TicketID,
        @BookingID = BookingID
    FROM dbo.Tickets
    WHERE TicketNo = @TicketNo
      AND UserID = @UserID;

    IF @TicketID IS NULL
    BEGIN
        RAISERROR ('Invalid ticket number or ticket does not belong to this user', 16, 1);
        RETURN;
    END

    /* STEP 3: Ticket + Booking Summary */
    SELECT
        T.TicketID,
        T.TicketNo,
        T.Status AS TicketStatus,
        T.FromLocation,
        T.ToLocation,
        T.CreatedAt AS TicketCreatedAt,
        T.UpdatedAt AS TicketUpdatedAt,

        B.BookingID,
        B.PickupDate,
        B.Status AS BookingStatus,
        B.BookingAmountPaid,

        U.Name,
        U.PhoneNumber,
        U.Email
    FROM dbo.Tickets T
    INNER JOIN dbo.Booking B ON T.BookingID = B.BookingID
    INNER JOIN dbo.Users U ON T.UserID = U.UserID
    WHERE T.TicketID = @TicketID;

    /* STEP 4: Ticket Comments (Timeline) */
    SELECT
        TC.CommentId,
        TC.CommentType,
        TC.CommentText,
        TC.CreatedBy,
        TC.CreatedAt
    FROM dbo.TicketComments TC
    WHERE TC.TicketId = @TicketID
    ORDER BY TC.CreatedAt DESC;
END
GO

