USE [MoversPackers]
GO

/****** Object:  StoredProcedure [dbo].[sp_GetBookingDetails_ByQuotation]    Script Date: 15-07-2026 16:34:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetBookingDetails_ByQuotation]
    @QuotationNumber NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    ----------------------------------------------------
    -- 1️⃣ Booking + User + Address + Slot Details
    ----------------------------------------------------
    SELECT 
        ISNULL(B.BookingID, 0) AS BookingID,
        ISNULL(B.QuotationNumber, '') AS QuotationNumber,
        ISNULL(B.PickupDate, '1900-01-01') AS PickupDate,
        ISNULL(B.Status, '') AS Status,
        ISNULL(B.TotalAmount, 0) AS TotalAmount,
        ISNULL(B.BookingAmountPaid, 0) AS BookingAmountPaid,
        ISNULL(B.TotalVolume, 0) AS TotalVolume,
        ISNULL(B.MovingType, '') AS MovingType,
        ISNULL(B.IsQuotation, 0) AS IsQuotation,

        -- User Details
        ISNULL(U.UserID, 0) AS UserID,
        ISNULL(U.Name, '') AS CustomerName,
        ISNULL(U.PhoneNumber, '') AS PhoneNumber,
        ISNULL(U.Email, '') AS Email,

        -- Address Details
        ISNULL(A.FromAddress, '') AS FromAddress,
        ISNULL(A.ToAddress, '') AS ToAddress,

        -- Time Slot
        ISNULL(T.SlotTime, '') AS SlotTime

    FROM Booking B
    LEFT JOIN Users U 
        ON B.UserID = U.UserID
    LEFT JOIN Address A 
        ON B.SourceAddressID = A.AddressID
    LEFT JOIN TimeSlot T 
        ON B.PickupTimeSlotID = T.SlotID
    WHERE B.QuotationNumber = @QuotationNumber;


    ----------------------------------------------------
    -- 2️⃣ Booking Items + Inventory Item Details
    ----------------------------------------------------
    SELECT 
        ISNULL(BI.BookingItemID, 0) AS BookingItemID,
        ISNULL(BI.BookingID, 0) AS BookingID,
        ISNULL(BI.ItemID, 0) AS ItemID,

        -- Inventory Item Details
        ISNULL(II.Name, '') AS ItemName,
        ISNULL(II.Category, '') AS Category,
        ISNULL(II.Description, '') AS Description,
        ISNULL(II.SizeCFT, 0) AS SizeCFT,

        -- Booked Quantity
        ISNULL(BI.Quantity, 0) AS BookedQuantity

    FROM BookingItem BI
    INNER JOIN Booking B 
        ON BI.BookingID = B.BookingID
    LEFT JOIN InventoryItem II
        ON BI.ItemID = II.ItemID
    WHERE B.QuotationNumber = @QuotationNumber;

END
GO

