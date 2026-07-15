using MimeKit.Cryptography;

namespace MoversAndPackerApi.Models
{
    //public class Booking
    //{
    //    public int BookingID { get; set; }
    //    public int UserID { get; set; }
    //    public int SourceAddressID { get; set; }
    //    public int DestinationAddressID { get; set; }
    //    public DateTime PickupDate { get; set; }
    //    public int PickupTimeSlotID { get; set; }
    //    public string Status { get; set; } = "Pending";
    //    public decimal TotalAmount { get; set; }
    //    public decimal BookingAmountPaid { get; set; }
    //}
    public class Booking
    {
        public int BookingID { get; set; }
        public int UserID { get; set; }
        public int SourceAddressID { get; set; }
        public int DestinationAddressID { get; set; }
        public DateTime PickupDate { get; set; }
        public int PickupTimeSlotID { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal BookingAmountPaid { get; set; }
        public string QuotationNumber { get; set; }
        public decimal TotalVolume { get; set; }
        public bool Isquotation {  get; set; }
   
        public string Ticket_distribution {  get; set; }
        // ✅ Add Navigation Property for Booking Items
        public ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
    }

}
