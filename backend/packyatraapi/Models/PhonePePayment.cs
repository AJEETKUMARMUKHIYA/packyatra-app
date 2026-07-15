namespace MoversAndPackerApi.Models
{
    public class PhonePePayment
    {
        public int Id { get; set; }

        public string MerchantTransactionId { get; set; } = null!;

        public string? PhonePeTransactionId { get; set; }

        public long Amount { get; set; }

        public string Status { get; set; } = null!;

        public string? RawWebhookPayload { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}