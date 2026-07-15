using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MoversAndPackerApi.Models.Enum;

[ApiController]
[Route("api/otp")]
public class OtpController : ControllerBase
{
    private readonly IMemoryCache _cache;
    private readonly BhashSmsService _smsService;

    public OtpController(IMemoryCache cache, BhashSmsService smsService)
    {
        _cache = cache;
        _smsService = smsService;
    }

    // SEND OTP
    [HttpPost("send")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
    {
        if (string.IsNullOrEmpty(request.MobileNumber))
            return BadRequest("Mobile number is required");

        var otp = OtpHelper.GenerateOtp();

        _cache.Set(request.MobileNumber, otp, TimeSpan.FromMinutes(30));// valid for 30 minuts

        var sent = await _smsService.SendOtpSmsAsync(request.MobileNumber, otp);

        if (!sent)
            return StatusCode(500, "Failed to send OTP");

        return Ok(new { message = "OTP sent successfully" });
    }

    // VERIFY OTP
    [HttpPost("verify")]
    public IActionResult VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        if (_cache.TryGetValue(request.MobileNumber, out string storedOtp))
        {
            if (storedOtp == request.Otp)
            {
                _cache.Remove(request.MobileNumber);
                return Ok(new { message = "OTP verified successfully" });
            }
        }
        return BadRequest("Invalid or expired OTP");
    }
}


public class SendOtpRequest
{
    public string MobileNumber { get; set; }
    public OtpPurpose Purpose { get; set; }
}

public class VerifyOtpRequest
{
    public string MobileNumber { get; set; }
    public string Otp { get; set; }
}
public static class OtpHelper
{
    public static string GenerateOtp()
    {
        return new Random().Next(100000, 999999).ToString();
    }
}
public class VerifyRequest
{
    public string Phone { get; set; }
    public int Otp { get; set; }
}