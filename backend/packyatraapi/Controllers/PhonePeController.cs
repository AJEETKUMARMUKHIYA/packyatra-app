using EllipticCurve.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoversAndPackerApi.Models;
using MoversAndPackerApi.Models.Payment;
using MoversAndPackerApi.Services;
using MoversAndPackerApi.Services.Interfaces;
using System.IO;

namespace MoversAndPackerApi.Controllers
{
    [ApiController]
    [Route("api/phonepe")]
    public class PhonePeController : ControllerBase
    {
        private readonly IPhonePeService _phonePeService;

        public PhonePeController(IPhonePeService phonePeService)
        {
            _phonePeService = phonePeService;
        }

        // 1️⃣ CREATE PAYMENT
        [HttpPost("pay")]
        public async Task<IActionResult> Pay([FromBody] CreatePaymentRequest request)
        {
            var response = await _phonePeService.CreatePaymentAsync(request);
            return Ok(response);
        }

        // 2️⃣ ORDER STATUS
        [HttpGet("status/{orderId}")]
        public async Task<IActionResult> Status(string orderId)
        {
            var response = await _phonePeService.GetOrderStatusAsync(orderId);
            return Ok(response);
        }

        // 3️⃣ CALLBACK
        [HttpPost("callback")]
        public async Task<IActionResult> Callback()
        {
            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync();
            var authHeader = Request.Headers["Authorization"].ToString();
            var logMessage = $"{DateTime.UtcNow:O} | Callback: {rawBody}\n";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "phonepe_callback.log");
            await System.IO.File.AppendAllTextAsync(filePath, logMessage);

           

            _phonePeService.ValidateAndProcessCallback(authHeader, rawBody);

            return Ok();
        }

        // In PhonePeController.cs
        [HttpPost("cancel-notify")]
        public IActionResult CancelNotify([FromBody] CancelNotifyRequest request)
        {
           // _logger.LogInformation("Payment cancelled: {TransactionId}", request.TransactionId);
            return Ok(new { success = true, message = "Cancellation noted" });
        }
    }
}
