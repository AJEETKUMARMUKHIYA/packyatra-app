USE [MoversPackers]
GO

/****** Object:  StoredProcedure [dbo].[sp_GetInvoiceDetails_ByInvoiceNumber]    Script Date: 15-07-2026 16:35:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetInvoiceDetails_ByInvoiceNumber]
    @InvoiceNumber NVARCHAR(100)
AS
 --EXEC sp_GetInvoiceDetails_ByInvoiceNumber  @InvoiceNumber = 'INV-2026-0001';
BEGIN
    SET NOCOUNT ON;

    ----------------------------------------------------
    -- 1️⃣ Invoice + Booking + User + Address + Slot
    ----------------------------------------------------
    SELECT 
        ISNULL(I.InvoiceID, 0) AS InvoiceID,
        ISNULL(I.InvoiceNumber, '') AS InvoiceNumber,
        ISNULL(I.CreatedDate, '1900-01-01') AS InvoiceDate,

        ISNULL(B.BookingID, 0) AS BookingID,
        ISNULL(B.QuotationNumber, '') AS QuotationNumber,
        ISNULL(B.PickupDate, '1900-01-01') AS PickupDate,
        ISNULL(B.Status, '') AS Status,
        ISNULL(B.TotalAmount, 0) AS TotalAmount,
        ISNULL(B.BookingAmountPaid, 0) AS BookingAmountPaid,
        ISNULL(B.TotalVolume, 0) AS TotalVolume,
        ISNULL(B.MovingType, '') AS MovingType,

        -- User Details
        ISNULL(U.UserID, 0) AS UserID,
        ISNULL(U.Name, '') AS CustomerName,
        ISNULL(U.PhoneNumber, '') AS PhoneNumber,
        ISNULL(U.Email, '') AS Email,

        -- Address
        ISNULL(A.FromAddress, '') AS FromAddress,
        ISNULL(A.ToAddress, '') AS ToAddress,

        -- Time Slot
        ISNULL(T.SlotTime, '') AS SlotTime

    FROM Invoice I
    INNER JOIN Booking B 
        ON I.BookingID = B.BookingID
    LEFT JOIN Users U 
        ON B.UserID = U.UserID
    LEFT JOIN Address A 
        ON B.SourceAddressID = A.AddressID
    LEFT JOIN TimeSlot T 
        ON B.PickupTimeSlotID = T.SlotID
    WHERE I.InvoiceNumber = @InvoiceNumber;


    ----------------------------------------------------
    -- 2️⃣ Booking Items
    ----------------------------------------------------
    SELECT 
        ISNULL(BI.BookingItemID, 0) AS BookingItemID,
        ISNULL(BI.BookingID, 0) AS BookingID,
        ISNULL(BI.ItemID, 0) AS ItemID,

        ISNULL(II.Name, '') AS ItemName,
        ISNULL(II.Category, '') AS Category,
        ISNULL(II.Description, '') AS Description,
        ISNULL(II.SizeCFT, 0) AS SizeCFT,

        ISNULL(BI.Quantity, 0) AS BookedQuantity

    FROM BookingItem BI
    INNER JOIN Booking B 
        ON BI.BookingID = B.BookingID
    INNER JOIN Invoice I
        ON B.BookingID = I.BookingID
    LEFT JOIN InventoryItem II
        ON BI.ItemID = II.ItemID
    WHERE I.InvoiceNumber = @InvoiceNumber;

END
GO

