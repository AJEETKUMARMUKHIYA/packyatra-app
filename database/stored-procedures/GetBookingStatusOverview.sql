USE [MoversPackers]
GO

/****** Object:  StoredProcedure [dbo].[GetBookingStatusOverview]    Script Date: 15-07-2026 16:31:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create   PROCEDURE [dbo].[GetBookingStatusOverview]
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get booking status distribution
    SELECT 
        Status AS BookingStatus,
        COUNT(*) AS BookingCount,
        SUM(TotalAmount) AS TotalAmount,
        AVG(TotalAmount) AS AvgAmount
    FROM Booking
    GROUP BY Status
    ORDER BY 
        CASE Status
            WHEN 'Pending' THEN 1
            WHEN 'Confirmed' THEN 2
            WHEN 'In Progress' THEN 3
            WHEN 'Completed' THEN 4
            WHEN 'Cancelled' THEN 5
            ELSE 6
        END;
END
GO

