USE [MoversPackers]
GO

/****** Object:  StoredProcedure [dbo].[sp_GetCityDistanceCFTPrice]    Script Date: 15-07-2026 16:34:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetCityDistanceCFTPrice]
(
    @Distance INT,
    @CFT INT,
    @Price DECIMAL(18,2) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        @Price =
        CASE
            WHEN @CFT BETWEEN 1 AND 100 THEN CFT_1_100
            WHEN @CFT BETWEEN 101 AND 200 THEN CFT_101_200
            WHEN @CFT BETWEEN 201 AND 300 THEN CFT_201_300
            WHEN @CFT BETWEEN 301 AND 400 THEN CFT_301_400
            WHEN @CFT BETWEEN 401 AND 500 THEN CFT_401_500
            WHEN @CFT BETWEEN 501 AND 700 THEN CFT_501_700
            WHEN @CFT BETWEEN 701 AND 900 THEN CFT_701_900
            WHEN @CFT BETWEEN 901 AND 1100 THEN CFT_901_1100
            WHEN @CFT BETWEEN 1101 AND 1300 THEN CFT_1101_1300
        END
    FROM CityDistanceCFT
    WHERE @Distance BETWEEN
          CAST(LEFT(DistanceRange, CHARINDEX('-', DistanceRange) - 1) AS INT)
      AND CAST(SUBSTRING(DistanceRange, CHARINDEX('-', DistanceRange) + 1, 10) AS INT);
END;
GO

