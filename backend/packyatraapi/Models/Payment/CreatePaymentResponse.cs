namespace MoversAndPackerApi.Models.Payment
{
    public class CreatePaymentResponse
    {
        public string MerchantOrderId { get; set; }
        public string RedirectUrl { get; set; }
    }
}
