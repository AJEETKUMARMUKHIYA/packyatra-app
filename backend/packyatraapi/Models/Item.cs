namespace MoversAndPackerApi.Models
{
    public class Item
    {
        public int ItemID { get; set; }  // Primary Key
        public string Name { get; set; }  // Item Name (e.g., Sofa, Table)
        public decimal Price { get; set; }  // Price Per Item (Optional)
        public decimal Volume { get; set; }  // CFT (Cubic Feet) for Moving Cost Calculation

        // ✅ Navigation Property (Optional: Only if BookingItem references Item)
        public ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
    }
}
