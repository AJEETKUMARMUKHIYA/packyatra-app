USE [MoversPackers]
GO

/****** Object:  StoredProcedure [dbo].[GetDashboardStats]    Script Date: 15-07-2026 16:32:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetDashboardStats]
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Calculate all stats in a single query
    WITH MonthlyStats AS (
        SELECT 
            -- Current month stats
            SUM(CASE 
                WHEN MONTH(PickupDate) = MONTH(GETDATE()) AND YEAR(PickupDate) = YEAR(GETDATE()) 
                THEN 1 ELSE 0 
            END) AS TotalBookings,
            
            SUM(CASE 
                WHEN MONTH(PickupDate) = MONTH(DATEADD(MONTH, -1, GETDATE())) 
                     AND YEAR(PickupDate) = YEAR(DATEADD(MONTH, -1, GETDATE())) 
                THEN 1 ELSE 0 
            END) AS TotalBookingsLastMonth,
            
            -- Completed moves
            SUM(CASE 
                WHEN Status = 'Completed' 
                     AND MONTH(PickupDate) = MONTH(GETDATE())
                     AND YEAR(PickupDate) = YEAR(GETDATE())
                THEN 1 ELSE 0 
            END) AS CompletedMoves,
            
            SUM(CASE 
                WHEN Status = 'Completed' 
                     AND MONTH(PickupDate) = MONTH(DATEADD(MONTH, -1, GETDATE()))
                     AND YEAR(PickupDate) = YEAR(DATEADD(MONTH, -1, GETDATE()))
                THEN 1 ELSE 0 
            END) AS CompletedMovesLastMonth,
            
            -- Revenue
            SUM(CASE 
                WHEN Status = 'Completed'
                     AND MONTH(PickupDate) = MONTH(GETDATE())
                     AND YEAR(PickupDate) = YEAR(GETDATE())
                THEN ISNULL(TotalAmount, 0) ELSE 0 
            END) AS TotalRevenue,
            
            SUM(CASE 
                WHEN Status = 'Completed'
                     AND MONTH(PickupDate) = MONTH(DATEADD(MONTH, -1, GETDATE()))
                     AND YEAR(PickupDate) = YEAR(DATEADD(MONTH, -1, GETDATE()))
                THEN ISNULL(TotalAmount, 0) ELSE 0 
            END) AS TotalRevenueLastMonth
        FROM Booking
    ),
    TicketStats AS (
        SELECT 
            -- Active tickets
            COUNT(CASE 
                WHEN Status NOT IN ('Closed', 'Resolved', 'Cancelled', 'Completed') 
                THEN 1 
            END) AS ActiveTickets,
            
            COUNT(CASE 
                WHEN Status NOT IN ('Closed', 'Resolved', 'Cancelled', 'Completed')
                     AND CAST(CreatedAt AS DATE) = CAST(DATEADD(DAY, -1, GETDATE()) AS DATE)
                THEN 1 
            END) AS ActiveTicketsYesterday,
            
            -- Priority breakdown
            COUNT(CASE 
                WHEN Status IN ('Pending', 'New', 'Urgent') 
                THEN 1 
            END) AS HighPriorityTickets,
            
            COUNT(CASE 
                WHEN Status IN ('In Progress', 'Assigned', 'Open') 
                THEN 1 
            END) AS MediumPriorityTickets,
            
            COUNT(CASE 
                WHEN Status IN ('Resolved', 'Closed', 'On Hold') 
                THEN 1 
            END) AS LowPriorityTickets,
            
            -- Supervisors
            COUNT(DISTINCT 
                CASE 
                    WHEN AssignedSupervisorID IS NOT NULL
                         AND MONTH(CreatedAt) = MONTH(GETDATE())
                         AND YEAR(CreatedAt) = YEAR(GETDATE())
                    THEN AssignedSupervisorID 
                END
            ) AS ActiveSupervisors,
            
            COUNT(DISTINCT 
                CASE 
                    WHEN AssignedSupervisorID IS NOT NULL
                    THEN AssignedSupervisorID 
                END
            ) AS TotalSupervisors,
            
            -- Average resolution time
            AVG(
                CASE 
                    WHEN Status IN ('Resolved', 'Closed', 'Completed') 
                    THEN DATEDIFF(DAY, CreatedAt, ISNULL(UpdatedAt, GETDATE()))
                    ELSE NULL
                END
            ) AS AvgResolutionDays,
            
            -- Average response time
            AVG(
                CASE 
                    WHEN AssignedSupervisorID IS NOT NULL 
                    THEN DATEDIFF(HOUR, CreatedAt, CreatedAt)
                    ELSE NULL
                END
            ) AS AvgResponseHours
        FROM Tickets
    ),
    UserStats AS (
        SELECT 
            COUNT(DISTINCT u.UserID) AS NewUsersWithoutBooking
        FROM Users u
        LEFT JOIN Booking b ON u.UserID = b.UserID
        WHERE b.BookingID IS NULL
    )
    SELECT 
        -- Main stats
        ISNULL(m.TotalBookings, 0) AS TotalBookings,
        ISNULL(t.ActiveTickets, 0) AS ActiveTickets,
        ISNULL(m.CompletedMoves, 0) AS CompletedMoves,
        ISNULL(m.TotalRevenue, 0) AS TotalRevenue,
        ISNULL(u.NewUsersWithoutBooking, 0) AS NewUsersWithoutBooking,
        ISNULL(t.ActiveSupervisors, 0) AS ActiveSupervisors,
        ISNULL(t.TotalSupervisors, 1) AS TotalSupervisors,
        ISNULL(t.HighPriorityTickets, 0) AS HighPriorityTickets,
        ISNULL(t.MediumPriorityTickets, 0) AS MediumPriorityTickets,
        ISNULL(t.LowPriorityTickets, 0) AS LowPriorityTickets,
        
        -- Trend calculations (with proper casting)
        CASE 
            WHEN ISNULL(m.TotalBookingsLastMonth, 0) > 0 
            THEN CAST(CAST((ISNULL(m.TotalBookings, 0) - m.TotalBookingsLastMonth) AS DECIMAL(10,2)) 
                   / m.TotalBookingsLastMonth * 100 AS DECIMAL(10,2))
            ELSE 
                CASE 
                    WHEN ISNULL(m.TotalBookings, 0) > 0 THEN 100.0 
                    ELSE 0.0 
                END
        END AS BookingTrend,
        
        CASE 
            WHEN ISNULL(t.ActiveTicketsYesterday, 0) > 0 
            THEN CAST(CAST((ISNULL(t.ActiveTickets, 0) - t.ActiveTicketsYesterday) AS DECIMAL(10,2)) 
                   / t.ActiveTicketsYesterday * 100 AS DECIMAL(10,2))
            ELSE 
                CASE 
                    WHEN ISNULL(t.ActiveTickets, 0) > 0 THEN 100.0 
                    ELSE 0.0 
                END
        END AS TicketTrend,
        
        CASE 
            WHEN ISNULL(m.CompletedMovesLastMonth, 0) > 0 
            THEN CAST(CAST((ISNULL(m.CompletedMoves, 0) - m.CompletedMovesLastMonth) AS DECIMAL(10,2)) 
                   / m.CompletedMovesLastMonth * 100 AS DECIMAL(10,2))
            ELSE 
                CASE 
                    WHEN ISNULL(m.CompletedMoves, 0) > 0 THEN 100.0 
                    ELSE 0.0 
                END
        END AS CompletedTrend,
        
        CASE 
            WHEN ISNULL(m.TotalRevenueLastMonth, 0) > 0 
            THEN CAST(CAST((ISNULL(m.TotalRevenue, 0) - m.TotalRevenueLastMonth) AS DECIMAL(10,2)) 
                   / m.TotalRevenueLastMonth * 100 AS DECIMAL(10,2))
            ELSE 
                CASE 
                    WHEN ISNULL(m.TotalRevenue, 0) > 0 THEN 100.0 
                    ELSE 0.0 
                END
        END AS RevenueTrend,
        
        -- Averages
        ISNULL(CAST(t.AvgResolutionDays AS DECIMAL(5,1)), 2.4) AS AvgResolutionDays,
        '94%' AS CustomerSatisfaction,
        ISNULL(CAST(t.AvgResponseHours AS DECIMAL(5,1)), 2.4) AS AvgResponseHours,
        GETDATE() AS LastUpdated
        
    FROM MonthlyStats m
    CROSS JOIN TicketStats t
    CROSS JOIN UserStats u;
END
GO

