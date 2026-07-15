using System.ComponentModel.DataAnnotations;
namespace MoversAndPackerApi.Models
{
 

    public class InventoryItem
    {
        [Key]  // Explicitly marks ItemID as the primary key
        public int ItemID { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public int Quantity { get; set; }
        [Required]
        public double SizeCFT { get; set; }
    }


}
