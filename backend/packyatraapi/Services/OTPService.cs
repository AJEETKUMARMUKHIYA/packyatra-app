
using System.Collections.Concurrent;
using System.Net;

namespace PackersAndMoversAPI.Services
{
    public class OTPService
    {
        private readonly string _user;
        private readonly string _pass;
        private readonly string _sender;
        private readonly ConcurrentDictionary<string, (int Otp, DateTime Expiry)> _otpStore
            = new ConcurrentDictionary<string, (int, DateTime)>();

        public OTPService(string user, string pass, string sender)
        {
            _user = user;
            _pass = pass;
            _sender = sender;
        }

        public async Task<bool> SendOtpAsync(string phone)
        {
            // Generate OTP
            var rnd = new Random();
            int otp = rnd.Next(100000, 999999);

            // Store OTP with expiry (5 minutes)
            _otpStore[phone] = (otp, DateTime.Now.AddMinutes(5));

            string message = $"Your OTP is {otp}";
            string url = $"http://bhashsms.com/api/sendmsg.php?user={_user}&pass={_pass}&sender={_sender}&phone={phone}&text={message}&priority=ndnd&stype=normal";

            using (WebClient client = new WebClient())
            {
                string result = await client.DownloadStringTaskAsync(url);
                // You can parse result to check success/failure
                return result.Contains("S") || result.Contains("success");
            }
        }

        public bool VerifyOtp(string phone, int userInputOtp)
        {
            if (_otpStore.TryGetValue(phone, out var otpData))
            {
                if (otpData.Otp == userInputOtp && DateTime.Now <= otpData.Expiry)
                {
                    _otpStore.TryRemove(phone, out _); // remove after success
                    return true;
                }
            }
            return false;
        }
    }
}



