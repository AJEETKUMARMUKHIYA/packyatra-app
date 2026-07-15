using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoversAndPackerApi.Models
{
    [Table("DistanceCFT")]
    public class DistanceCFT
    {
        [Key]
        public int Id { get; set; }  // Auto-generated primary key (optional)

        [Required]
        public string DistanceRange { get; set; }  // Stores values like "100-199"

        public int CFT_100_149 { get; set; }
        public int CFT_150_199 { get; set; }
        public int CFT_200_249 { get; set; }
        public int CFT_250_299 { get; set; }
        public int CFT_300_349 { get; set; }
        public int CFT_350_399 { get; set; }
        public int CFT_400_499 { get; set; }
        public int CFT_500_549 { get; set; }
        public int CFT_550_599 { get; set; }
        public int CFT_600_649 { get; set; }
        public int CFT_650_699 { get; set; }
        public int CFT_700_749 { get; set; }
        public int CFT_750_799 { get; set; }
        public int CFT_800_849 { get; set; }
        public int CFT_850_899 { get; set; }
        public int CFT_900_949 { get; set; }
        public int CFT_950_999 { get; set; }
    }
}
