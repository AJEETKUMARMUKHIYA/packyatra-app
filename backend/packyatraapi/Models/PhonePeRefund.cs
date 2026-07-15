namespace MoversAndPackerApi.Models
{
    public class PhonePeRefund
    {
        public int Id { get; set; }  // Primary key

        public string RefundId { get; set; } = null!;  // PhonePe refund ID, unique

        public string MerchantTransactionId { get; set; } = null!;  // Original merchant order ID

        public long Amount { get; set; }  // Refund amount

        public string Status { get; set; } = null!;  // SUCCESS / FAILED / PENDING

        public string? RawWebhookPayload { get; set; }  // Full JSON payload from webhook

        public DateTime CreatedAt { get; set; }  // Record creation timestamp

        public DateTime? UpdatedAt { get; set; }  // Last update timestamp (if retried)
    }
}