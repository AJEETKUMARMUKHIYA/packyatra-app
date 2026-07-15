using PackersAndMoversAPI.Models;

namespace PackersAndMoversAPI.Repositories
{
    public interface IOTPRepository
    {
         void SentOTP(string phoneNumber);
         string VerifyOTP(OtpRequest otpRequest); 
    }
}
