namespace MoversAndPackerApi.Models.Callback;

public class PhonePeCallbackModel
{
    public string Event { get; set; }
    public CallbackPayload Payload { get; set; }
}

public class CallbackPayload
{
    public string OrderId { get; set; }
    public string MerchantOrderId { get; set; }
    public string State { get; set; }
    public long Amount { get; set; }
}
