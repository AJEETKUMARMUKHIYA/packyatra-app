namespace MoversAndPackerApi.Models
{
    public class BookingDetails
    {
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
        public string Ticket_distribution { get; set; }
        public List<BookingItem> BookingItemList { get; set; }
    }

    

}
