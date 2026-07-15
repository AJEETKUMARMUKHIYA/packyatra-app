USE [MoversPackers]
GO

/****** Object:  StoredProcedure [dbo].[GetTicketStatusData]    Script Date: 15-07-2026 16:33:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create   PROCEDURE [dbo].[GetTicketStatusData]
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get all ticket statuses and their counts
    SELECT 
        Status,
        COUNT(*) AS TicketCount
    FROM Tickets
    GROUP BY Status
    
    UNION ALL
    
    -- Get total tickets for last month
    SELECT 
        'Last Month' AS Status,
        COUNT(*) AS TicketCount
    FROM Tickets
    WHERE MONTH(CreatedAt) = MONTH(DATEADD(MONTH, -1, GETDATE()))
    AND YEAR(CreatedAt) = YEAR(DATEADD(MONTH, -1, GETDATE()))
    
    -- Ensure we always have data
    ORDER BY Status;
END
GO

