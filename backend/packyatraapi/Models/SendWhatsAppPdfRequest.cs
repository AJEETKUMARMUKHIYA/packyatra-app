namespace MoversAndPackerApi.Models
{
    public class SendWhatsAppPdfRequest
    {
        public string MobileNumber { get; set; } // 919XXXXXXXXX
        public string PdfUrl { get; set; }        // Public HTTPS PDF
    }
}
