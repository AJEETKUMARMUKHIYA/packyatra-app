using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MoversAndPackerApi.Models
{
    [Table("CityDistanceCFT")]  // Maps to the database table
    public class CityDistanceCFT
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // Primary Key

        [Required]
        [StringLength(20)]
        public string DistanceRange { get; set; }  // Distance range (e.g., "0-10", "11-20")
        [Required]
        public int CFT_1_100 { get; set; }

        [Required]
        public int CFT_101_200 { get; set; }

        [Required]
        public int CFT_201_300 { get; set; }

        [Required]
        public int CFT_301_400 { get; set; }

        [Required]
        public int CFT_401_500 { get; set; }

        [Required]
        public int CFT_501_700 { get; set; }

        [Required]
        public int CFT_701_900 { get; set; }

        [Required]
        public int CFT_901_1100 { get; set; }

        [Required]
        public int CFT_1101_1300 { get; set; }
    }
}
