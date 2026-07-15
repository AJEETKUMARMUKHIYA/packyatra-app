namespace MoversAndPackerApi.Models
{
    public class InvoiceResponseDto
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public int? InvoiceID { get; set; }
        public int? BookingID { get; set; }
        public string? QuotationNumber { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
