

using MoversAndPackerApi.Models.Payment;

namespace MoversAndPackerApi.Services.Interfaces
{
    public interface IPhonePeService
    {
        Task<CreatePaymentResponse> CreatePaymentAsync(CreatePaymentRequest request);
        Task<object> GetOrderStatusAsync(string orderId);
        void ValidateAndProcessCallback(string authHeader, string rawBody);
    }
}