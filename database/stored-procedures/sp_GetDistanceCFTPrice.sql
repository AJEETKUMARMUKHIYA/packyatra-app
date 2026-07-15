USE [MoversPackers]
GO

/****** Object:  StoredProcedure [dbo].[sp_GetDistanceCFTPrice]    Script Date: 15-07-2026 16:35:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetDistanceCFTPrice]
(
    @Distance INT,
    @CFT INT,
    @Price DECIMAL(18,2) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @MinCFT INT,
        @MaxCFT INT,
        @CFTColumn SYSNAME,
        @SQL NVARCHAR(MAX);

    /* 1️⃣ Validate CFT range */
    IF @CFT < 100 OR @CFT > 1999
    BEGIN
        SET @Price = NULL;
        RETURN;
    END

    /* 2️⃣ Calculate CFT range (50 bucket) */
    SET @MinCFT = (@CFT / 50) * 50;

    IF @MinCFT < 100 
        SET @MinCFT = 100;

    SET @MaxCFT = @MinCFT + 49;

    /* 3️⃣ Build column name */
    SET @CFTColumn = 
        'CFT_' + CAST(@MinCFT AS VARCHAR(10)) + '_' + CAST(@MaxCFT AS VARCHAR(10));

    /* 4️⃣ Dynamic SQL */
    SET @SQL = N'
        SELECT @PriceOut = ' + QUOTENAME(@CFTColumn) + '
        FROM DistanceCFT
        WHERE @Distance BETWEEN
              CAST(LEFT(DistanceRange, CHARINDEX(''-'', DistanceRange) - 1) AS INT)
          AND CAST(SUBSTRING(
                DistanceRange,
                CHARINDEX(''-'', DistanceRange) + 1,
                LEN(DistanceRange)
              ) AS INT)
    ';

    EXEC sp_executesql
        @SQL,
        N'@Distance INT, @PriceOut DECIMAL(18,2) OUTPUT',
        @Distance = @Distance,
        @PriceOut = @Price OUTPUT;
END;
GO

