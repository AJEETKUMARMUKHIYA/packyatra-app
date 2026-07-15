using Microsoft.AspNetCore.Mvc;
using MoversAndPackerApi.Services;


namespace MoversAndPackerApi.Controllers
{
    [ApiController]
    [Route("api/way2smsotp")]
    public class Fast2smsController : Controller
    {
        private readonly Fast2smsService _otpService;

        public Fast2smsController(Fast2smsService otpService)
        {
            _otpService = otpService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendOtp(SendOtpRequest request)
        {
            var result = await _otpService.SendOtp(request);

            if (!result)
                return BadRequest("Failed to send OTP");

            return Ok(new { message = "OTP sent successfully" });
        }

        [HttpPost("verify")]
        public IActionResult VerifyOtp(VerifyOtpRequest request)
        {
            var valid = _otpService.VerifyOtp(request);

            if (!valid)
                return BadRequest("Invalid or expired OTP");

            return Ok(new { message = "OTP verified successfully" });
        }
    }
}