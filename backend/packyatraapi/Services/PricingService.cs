using Microsoft.Data.SqlClient;

namespace MoversAndPackerApi.Services
{


    public class PricingService
    {
        private readonly string _conn;

        public PricingService(IConfiguration config)
        {
            _conn = config.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CalculatePrice(int distance, int cft)
        {
            using var con = new SqlConnection(_conn);
            await con.OpenAsync();

            string sql = """
SELECT TOP 1
    CASE
        WHEN @cft BETWEEN 100 AND 200 THEN CFT_100_200
        WHEN @cft BETWEEN 201 AND 300 THEN CFT_201_300
        WHEN @cft BETWEEN 301 AND 400 THEN CFT_301_400
        WHEN @cft BETWEEN 401 AND 500 THEN CFT_401_500
        WHEN @cft BETWEEN 501 AND 700 THEN CFT_501_700
        ELSE CFT_701_900
    END AS Price
FROM CityDistanceCFT
WHERE @distance BETWEEN
      CAST(PARSENAME(REPLACE(DistanceRange,'-','.'),2) AS INT)
  AND CAST(PARSENAME(REPLACE(DistanceRange,'-','.'),1) AS INT)
""";

            var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@distance", distance);
            cmd.Parameters.AddWithValue("@cft", cft);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
    }
}

