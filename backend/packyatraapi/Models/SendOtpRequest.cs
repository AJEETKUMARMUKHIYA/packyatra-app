using MoversAndPackerApi.Models.Enum;

namespace MoversAndPackerApi.Models
{

    public class SendOtpRequest
    {
        public string MobileNumber { get; set; }
        public OtpPurpose Purpose { get; set; }
        public string ReferenceId { get; set; } // BookingId / JobId
    }

}
