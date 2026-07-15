using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MoversAndPackerApi.Data;
using MoversAndPackerApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MoversAndPackerApi.Services
{
    public class PriceService
    {
        private readonly ApplicationDbContext _context;

        public PriceService(ApplicationDbContext context)
        {
            _context = context;
        }

       
        public async Task<decimal?> GetPriceAsync(int distance, int cft, string activeTab)
        {
            // If the request is for "within city", fetch data from dbo.CityDistanceCFT
            //    if (activeTab == "within")
            //    {
            //        List<CityDistanceCFT> cityDistanceCFTList = await _context.CityDistanceCFTs.ToListAsync();

            //        // Find the correct row based on the distance range
            //        CityDistanceCFT matchedRow = cityDistanceCFTList.FirstOrDefault(row =>
            //        {
            //            string[] rangeParts = row.DistanceRange.Split('-');
            //            if (rangeParts.Length == 2 && int.TryParse(rangeParts[0], out int minRange) && int.TryParse(rangeParts[1], out int maxRange))
            //            {
            //                return distance >= minRange && distance <= maxRange;
            //            }
            //            return false;
            //        });

            //        if (matchedRow == null)
            //        {
            //            return null; // No matching distance range found
            //        }

            //        // Define CFT range mappings based on the CityDistanceCFT table
            //        var cftPriceMap = new Dictionary<(int Min, int Max), decimal>
            //{
            //                  { (1, 100), matchedRow.CFT_1_100 },
            //    { (100, 200), matchedRow.CFT_101_200 },
            //    { (201, 300), matchedRow.CFT_201_300 },
            //    { (301, 400), matchedRow.CFT_301_400 },
            //    { (401, 500), matchedRow.CFT_401_500 },
            //    { (501, 700), matchedRow.CFT_501_700 },
            //    { (701, 900), matchedRow.CFT_701_900 },
            //    { (900, 1100), matchedRow.CFT_901_1100 },
            //    { (1100, 1300), matchedRow.CFT_1101_1300 }
            //};

            //        // Find the corresponding CFT price
            //        foreach (var (range, price) in cftPriceMap)
            //        {
            //            if (cft >= range.Min && cft <= range.Max)
            //            {
            //                return price;
            //            }
            //        }

            //        return null; // No matching CFT range found
            //    }

            if (activeTab == "within")
            {
                var distanceParam = new SqlParameter("@Distance", SqlDbType.Int)
                {
                    Value = distance
                };

                var cftParam = new SqlParameter("@CFT", SqlDbType.Int)
                {
                    Value = cft
                };

                var priceParam = new SqlParameter("@Price", SqlDbType.Decimal)
                {
                    Precision = 18,
                    Scale = 2,
                    Direction = ParameterDirection.Output
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.sp_GetCityDistanceCFTPrice @Distance, @CFT, @Price OUTPUT",
                    distanceParam,
                    cftParam,
                    priceParam
                );

                return priceParam.Value == DBNull.Value
                    ? null
                    : (decimal?)priceParam.Value;
            }

            else if (activeTab != "within")
            {
                var distanceParam = new SqlParameter("@Distance", SqlDbType.Int)
                {
                    Value = distance
                };

                var cftParam = new SqlParameter("@CFT", SqlDbType.Int)
                {
                    Value = cft
                };

                var priceParam = new SqlParameter("@Price", SqlDbType.Decimal)
                {
                    Precision = 18,
                    Scale = 2,
                    Direction = ParameterDirection.Output
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.sp_GetDistanceCFTPrice @Distance, @CFT, @Price OUTPUT",
                    distanceParam,
                    cftParam,
                    priceParam
                );

                return priceParam.Value == DBNull.Value
                    ? null
                    : (decimal?)priceParam.Value;
            }

            else return null; // No matching CFT range found

            //        // If "between cities", use the existing logic
            //        List<DistanceCFT> distanceCFTList = await _context.DistanceCFTs.ToListAsync();

            //        // Find the correct row based on the distance range
            //        DistanceCFT matchedRowBetweenCities = distanceCFTList.FirstOrDefault(row =>
            //        {
            //            string[] rangeParts = row.DistanceRange.Split('-');
            //            if (rangeParts.Length == 2 && int.TryParse(rangeParts[0], out int minRange) && int.TryParse(rangeParts[1], out int maxRange))
            //            {
            //                return distance >= minRange && distance <= maxRange;
            //            }
            //            return false;
            //        });

            //        if (matchedRowBetweenCities == null)
            //        {
            //            return null; // No matching distance range found
            //        }

            //        // Define CFT range mappings based on the DistanceCFT table
            //        var cftPriceMapBetweenCities = new Dictionary<(int Min, int Max), decimal>
            //{
            //    { (100, 149), matchedRowBetweenCities.CFT_100_149 },
            //    { (150, 199), matchedRowBetweenCities.CFT_150_199 },
            //    { (200, 249), matchedRowBetweenCities.CFT_200_249 },
            //    { (250, 299), matchedRowBetweenCities.CFT_250_299 },
            //    { (300, 349), matchedRowBetweenCities.CFT_300_349 },
            //    { (350, 399), matchedRowBetweenCities.CFT_350_399 },
            //    { (400, 499), matchedRowBetweenCities.CFT_400_499 },
            //    { (500, 549), matchedRowBetweenCities.CFT_500_549 },
            //    { (550, 599), matchedRowBetweenCities.CFT_550_599 },
            //    { (600, 649), matchedRowBetweenCities.CFT_600_649 },
            //    { (650, 699), matchedRowBetweenCities.CFT_650_699 },
            //    { (700, 749), matchedRowBetweenCities.CFT_700_749 },
            //    { (750, 799), matchedRowBetweenCities.CFT_750_799 },
            //    { (800, 849), matchedRowBetweenCities.CFT_800_849 },
            //    { (850, 899), matchedRowBetweenCities.CFT_850_899 },
            //    { (900, 949), matchedRowBetweenCities.CFT_900_949 },
            //    { (950, 999), matchedRowBetweenCities.CFT_950_999 }
            //};

            //        // Find the corresponding CFT price
            //        foreach (var (range, price) in cftPriceMapBetweenCities)
            //        {
            //            if (cft >= range.Min && cft <= range.Max)
            //            {
            //                return price;
            //            }
            //        }

            //        return null; // No matching CFT range found
        }

    }
}
