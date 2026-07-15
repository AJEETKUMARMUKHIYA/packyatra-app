namespace MoversAndPackerApi.Models
{
    //public class BookingItem
    //{
    //    public int BookingItemID { get; set; }
    //    public int BookingID { get; set; }
    //    public int ItemID { get; set; }
    //    public int Quantity { get; set; }
    //}
    public class BookingItem
    {
        public int BookingItemID { get; set; }
        public int BookingID { get; set; }  // Foreign Key to Booking
        public int ItemID { get; set; }  // Foreign Key to Item
        public int Quantity { get; set; }
      

        // ✅ Navigation Properties
       // public Booking Booking { get; set; }
       // public Item Item { get; set; }  // ✅ Reference to Item
    }
}
