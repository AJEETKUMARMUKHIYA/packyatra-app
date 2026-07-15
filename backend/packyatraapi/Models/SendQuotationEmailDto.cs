namespace MoversAndPackerApi.Models
{
    public class SendQuotationEmailDto
    {
        public string RecipientEmail { get; set; }
        public string CustomerName { get; set; }
        public string QuotationNumber { get; set; }
        public string PickupDate { get; set; }
        public string PickupTime { get; set; }
        public int TotalAmount { get; set; }
        public string PdfBase64 { get; set; }
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public string CustomerPhone { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public decimal Distance { get; set; }
        public decimal TotalCFT { get; set; }
        public int TotalItems { get; set; }
      
    }

    public class EmailResponseDto
    {
        public bool Success { get; set; }
        public string MessageId { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
    }
}
