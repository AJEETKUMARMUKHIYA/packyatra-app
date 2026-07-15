USE [MoversPackers]
GO

/****** Object:  StoredProcedure [dbo].[GetMonthlyRevenueChart]    Script Date: 15-07-2026 16:32:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create   PROCEDURE [dbo].[GetMonthlyRevenueChart]
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get revenue for current year
    WITH Months AS (
        SELECT 1 AS MonthNumber, 'January' AS MonthName
        UNION SELECT 2, 'February'
        UNION SELECT 3, 'March'
        UNION SELECT 4, 'April'
        UNION SELECT 5, 'May'
        UNION SELECT 6, 'June'
        UNION SELECT 7, 'July'
        UNION SELECT 8, 'August'
        UNION SELECT 9, 'September'
        UNION SELECT 10, 'October'
        UNION SELECT 11, 'November'
        UNION SELECT 12, 'December'
    )
    SELECT 
        m.MonthNumber,
        m.MonthName,
        ISNULL(SUM(b.TotalAmount), 0) AS Revenue,
        ISNULL(COUNT(b.BookingID), 0) AS BookingCount,
        -- Calculate average booking value
        CASE 
            WHEN COUNT(b.BookingID) > 0 
            THEN ISNULL(SUM(b.TotalAmount), 0) / COUNT(b.BookingID)
            ELSE 0
        END AS AvgBookingValue
    FROM Months m
    LEFT JOIN Booking b ON 
        MONTH(b.PickupDate) = m.MonthNumber 
        AND YEAR(b.PickupDate) = YEAR(GETDATE())
        AND b.Status IN ('Completed', 'Confirmed', 'In Progress')
    WHERE m.MonthNumber <= MONTH(GETDATE()) -- Only months up to current
    GROUP BY m.MonthNumber, m.MonthName
    ORDER BY m.MonthNumber;
END
GO

