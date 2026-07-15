using MoversAndPackerApi.Models;
using MoversAndPackerApi.Models.Enum;
using System.Net.Http.Headers;

namespace MoversAndPackerApi.Services
{
    public class Fast2smsService
    {
        private readonly IConfiguration _config;

        // Key = mobile + purpose + referenceId
        private static Dictionary<string, OtpEntry> _store = new();

        public Fast2smsService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendOtp(SendOtpRequest request)
        {
            var otp = new Random().Next(100000, 999999).ToString();

            var key = GetKey(request);

            _store[key] = new OtpEntry
            {
                Otp = otp,
                Expiry = DateTime.UtcNow.AddMinutes(30) // here set otp expire timing
            };

            var message = GetMessage(request.Purpose, otp);

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _config["Fast2Sms:ApiKey"]);

            var url =
                $"https://www.fast2sms.com/dev/bulkV2?route=otp&variables_values={otp}&numbers={request.MobileNumber}";

            var response = await client.GetAsync(url);
            return response.IsSuccessStatusCode;
        }

        public bool VerifyOtp(VerifyOtpRequest request)
        {
            var key = GetKey(request);

            if (!_store.ContainsKey(key))
                return false;

            var entry = _store[key];

            if (DateTime.UtcNow > entry.Expiry)
            {
                _store.Remove(key);
                return false;
            }

            if (entry.Otp != request.Otp)
                return false;

            _store.Remove(key);
            return true;
        }

        private string GetKey(dynamic req)
            => $"{req.MobileNumber}_{req.Purpose}_{req.ReferenceId}";

        private string GetMessage(OtpPurpose purpose, string otp)
        {
            return purpose switch
            {
                OtpPurpose.JobStart =>
                    $"Your Job Start OTP is {otp}.\r\nShare this OTP with the service executive to start the job.\r\nhttps://packyatra.com/",

                OtpPurpose.Delivery =>
                    $"Dear Customer,\r\nYour Delivery OTP is {otp}.\r\nPlease share this OTP with the delivery executive to complete the delivery.\r\n– PackYatra",

                OtpPurpose.JobCompletion =>
                    $"Hello {{CustomerName}},\r\nYour delivery has been completed successfully.\r\nThank you for trusting PackYatra for your moving needs.",

                _ => $"Your OTP is {otp}. - PackYatra"
            };
        }
    }

}
