using MoversAndPackerApi.Services.Interfaces;
using pg_sdk_dotnet;
using pg_sdk_dotnet.Common.Utils;
using pg_sdk_dotnet.Payments.v2;
using pg_sdk_dotnet.Payments.v2.Models.Request;
using MoversAndPackerApi.Models.Payment;
using System.Text.Json;

namespace MoversAndPackerApi.Services
{
    public class PhonePeService : IPhonePeService
    {
        private readonly StandardCheckoutClient _client;
        private readonly IConfiguration _config;
        private readonly ILogger<PhonePeService> _logger;

        public PhonePeService(
            IConfiguration config,
            ILoggerFactory loggerFactory,
            ILogger<PhonePeService> logger)
        {
            _config = config;
            _logger = logger;

            _client = StandardCheckoutClient.GetInstance(
                config["PhonePe:ClientId"],
                config["PhonePe:ClientSecret"],
                int.Parse(config["PhonePe:ClientVersion"]),
                Env.PRODUCTION,
                loggerFactory
            );
        }

        // 1️⃣ CREATE PAYMENT - KEEP AS IS
        public async Task<CreatePaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
        {
            var merchantOrderId = Guid.NewGuid().ToString();

            long amount = request.Amount;
            var payRequest = StandardCheckoutPayRequest.Builder()
                .SetMerchantOrderId(merchantOrderId)
                .SetAmount(amount)
                .SetRedirectUrl(_config["PhonePe:RedirectUrl"])
                .SetExpireAfter(300)
                .SetMessage("PhonePe Payment")
                .Build();

            var response = await _client.Pay(payRequest);

            _logger.LogInformation("Pay Response: {0}",
                JsonSerializer.Serialize(response, JsonOptions.IndentedWithRelaxedEscaping));

            return new CreatePaymentResponse
            {
                MerchantOrderId = merchantOrderId,
                RedirectUrl = response.RedirectUrl
            };
        }

        // 2️⃣ GET ORDER STATUS - FIXED MINIMALLY
        public async Task<object> GetOrderStatusAsync(string orderId)
        {
            try
            {
                // Get the raw response
                var rawResponse = await _client.GetOrderStatus(orderId, true);

                // Convert to JSON to see what we get
                string jsonResponse = JsonSerializer.Serialize(rawResponse,
                    new JsonSerializerOptions { WriteIndented = true });

                _logger.LogInformation("PhonePe Raw Status Response for {OrderId}:\n{Response}",
                    orderId, jsonResponse);

                // Try to extract status from the response
                // PhonePe SDK v2 typically returns OrderStatusResponse
                string status = "UNKNOWN";
                string transactionId = orderId;

                // Use reflection to get properties (safe way)
                var responseType = rawResponse.GetType();

                // Try to get State property
                var stateProperty = responseType.GetProperty("State");
                if (stateProperty != null)
                {
                    status = stateProperty.GetValue(rawResponse)?.ToString() ?? "UNKNOWN";
                }

                // Try to get TransactionId property
                var txnProperty = responseType.GetProperty("TransactionId");
                if (txnProperty != null)
                {
                    transactionId = txnProperty.GetValue(rawResponse)?.ToString() ?? orderId;
                }

                // Map to simple status
                string simpleStatus = status.ToUpper().Contains("CANCELLED") ? "CANCELLED" :
                                    status.ToUpper().Contains("SUCCESS") ? "SUCCESS" :
                                    status.ToUpper().Contains("FAILED") ? "FAILED" : "PENDING";

                // Return structured response that frontend can use
                return new
                {
                    Success = simpleStatus == "SUCCESS",
                    OrderId = orderId,
                    Status = simpleStatus, // SUCCESS, CANCELLED, FAILED, PENDING
                    RawStatus = status, // Original PhonePe status
                    TransactionId = transactionId,
                    Timestamp = DateTime.UtcNow,
                    // Include raw response for debugging
                    Debug = new { RawResponse = jsonResponse }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order status for {OrderId}", orderId);

                return new
                {
                    Success = false,
                    OrderId = orderId,
                    Status = "ERROR",
                    ErrorMessage = ex.Message,
                    Timestamp = DateTime.UtcNow
                };
            }
        }

        // 3️⃣ CALLBACK VALIDATION - KEEP AS IS
        public void ValidateAndProcessCallback(string authHeader, string rawBody)
        {
            var callbackResponse = _client.ValidateCallback(
                _config["PhonePe:MerchantUsername"],
                _config["PhonePe:MerchantPassword"],
                authHeader,
                rawBody
            );

            var orderId = callbackResponse.Payload.OrderId;
            var state = callbackResponse.Payload.State;

            _logger.LogInformation("Callback OrderId={0}, State={1}", orderId, state);

            // TODO: Update DB here
        }
    }
}