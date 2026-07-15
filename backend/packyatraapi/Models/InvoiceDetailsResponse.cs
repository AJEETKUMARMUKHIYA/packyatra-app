namespace MoversAndPackerApi.Models
{
    public class InvoiceDetailsResponse
    {
        public InvoiceSummaryDto Invoice { get; set; }
        public List<InvoiceItemDto> Items { get; set; }
    }

    public class InvoiceSummaryDto
    {
        public int InvoiceID { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }

        public int BookingID { get; set; }
        public string QuotationNumber { get; set; }
        public DateTime PickupDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal BookingAmountPaid { get; set; }
        public decimal TotalVolume { get; set; }
        public string MovingType { get; set; }

        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public string FromAddress { get; set; }
        public string ToAddress { get; set; }

        public string SlotTime { get; set; }
    }

    public class InvoiceItemDto
    {
        public int BookingItemID { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public decimal SizeCFT { get; set; }
        public int BookedQuantity { get; set; }
    }
}
