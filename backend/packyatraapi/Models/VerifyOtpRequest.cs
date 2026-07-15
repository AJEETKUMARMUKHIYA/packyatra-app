using MoversAndPackerApi.Models.Enum;

namespace MoversAndPackerApi.Models
{

    public class VerifyOtpRequest
    {
        public string MobileNumber { get; set; }
        public string Otp { get; set; }
        public OtpPurpose Purpose { get; set; }
        public string ReferenceId { get; set; }
    }

}
