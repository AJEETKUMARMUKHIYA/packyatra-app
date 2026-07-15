USE [MoversPackers]
GO

/****** Object:  StoredProcedure [dbo].[GetRecentBookingsWithDetails]    Script Date: 15-07-2026 16:33:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetRecentBookingsWithDetails]
    @TopCount INT = 5000
AS
--- exec [dbo].[GetRecentBookingsWithDetails]
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP (@TopCount)
        b.BookingID,
        t.TicketID,
        t.TicketNo,
        u.Name AS UserName,
        u.Email AS UserEmail,
        u.PhoneNumber AS UserPhone,
        b.PickupDate,
        b.TotalAmount,
		b.BookingAmountPaid,
		b.SourceAddressId,
        b.Status AS BookingStatus,
        t.Status AS TicketStatus,
        t.FromLocation,
        t.ToLocation,
        t.CreatedAt AS TicketCreated,
        t.AssignedSupervisorID,
		t.FromLocation,
		t.Tolocation,
		b.QuotationNumber,
		aFrom. FromAddress,
        aFrom. ToAddress,
        -- Format dates
        FORMAT(b.PickupDate, 'dd-MMM-yyyy') AS FormattedPickupDate,
        CASE 
            WHEN t.CreatedAt IS NOT NULL 
            THEN FORMAT(t.CreatedAt, 'dd-MMM-yyyy HH:mm')
            ELSE 'No Ticket'
        END AS FormattedTicketDate,
        -- Calculate booking age
        DATEDIFF(DAY, b.PickupDate, GETDATE()) AS DaysSincePickup,
        -- Payment status
        CASE 
            WHEN b.BookingAmountPaid >= b.TotalAmount THEN 'Paid'
            WHEN b.BookingAmountPaid > 0 THEN 'Partially Paid'
            ELSE 'Unpaid'
        END AS PaymentStatus,
       b.Ticket_distribution
    FROM Booking b
    INNER JOIN Users u ON b.UserID = u.UserID
    LEFT JOIN Tickets t ON b.BookingID = t.BookingID
    LEFT JOIN Address aFrom ON b.SourceAddressId = aFrom.AddressId 
   -- WHERE b.Ticket_distribution = 'Active' --AND b.Ticket_distribution != 'Closed' --OR b.Ticket_distribution IS NULL
    ORDER BY b.bookingid desc, b.PickupDate DESC, t.CreatedAt DESC;
END
GO

