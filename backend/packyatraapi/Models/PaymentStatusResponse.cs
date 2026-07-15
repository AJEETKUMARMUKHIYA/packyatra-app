namespace MoversAndPackerApi.Models
{
    public class PaymentStatusResponse
    {
        public bool Success { get; set; }
        public string OrderId { get; set; }
        public string Status { get; set; } // SUCCESS, CANCELLED, FAILED, PENDING
        public string RawStatus { get; set; } // Original PhonePe status
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public string ErrorMessage { get; set; }
    }
}
