using Microsoft.Extensions.Options;
using MoversAndPackerApi.Models;
using PackersAndMoversAPI.Models;
using System;
using System.Collections.Generic;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
namespace PackersAndMoversAPI.Repositories
{
    public class OTPRepository : IOTPRepository
    {
        private  readonly Random random = new Random();
        private readonly TwilioCredentials _settings;

        public OTPRepository(IOptions<TwilioCredentials> settings)
        {
            _settings = settings.Value;
        }

        // Dictionary to store OTPs temporarily (Use DB for production)
        private readonly Dictionary<string, (string otp, DateTime expiry)> otpStore = new();

        public  string GenerateOTP()
        {
            return random.Next(100000, 999999).ToString();
        }

        public  void SentOTP(string phoneNumber)
        {
            var AccountSid = _settings.AccountSid;
            var AuthToken = _settings.AuthToken;
            var TwilioPhoneNumber = _settings.TwilioPhoneNumber;

            string otp = GenerateOTP();
            otpStore[phoneNumber] = (otp, DateTime.UtcNow.AddMinutes(5)); // OTP valid for 5 minutes

            TwilioClient.Init(AccountSid, AuthToken);

            var message = MessageResource.Create(
                to: new PhoneNumber("+91"+phoneNumber),
                from: new PhoneNumber(TwilioPhoneNumber),
                body: $"Your OTP code is: {otp}. It expires in 5 minutes."
            );
        }

        public string VerifyOTP(OtpRequest otpRequest )
        {
            if (!otpStore.TryGetValue(otpRequest.MobileNo, out var otpData))
            {
                return "No OTP found for this number.";
            }

            var (otp, expiry) = otpData;

            // Remove OTP before returning response to prevent reuse
            otpStore.Remove(otpRequest.MobileNo);

            if (DateTime.UtcNow > expiry)
            {
                return "OTP Expired";
            }
           
            return otp == otpRequest.Otp ? "OTP Verified Successfully!" : "Invalid OTP";
        }

    }
}
