// EmailController.cs
using Microsoft.AspNetCore.Mvc;
using MoversAndPackerApi.Models;
using MoversAndPackerApi.Services;
using MoversAndPackerApi.Services.Interfaces;

namespace MoversAndPackerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly ISendGridEmailService _emailService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(ISendGridEmailService emailService, ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("send-quotation")]
        public async Task<IActionResult> SendQuotation([FromBody] SendQuotationEmailDto dto)
        {
            try
            {
                _logger.LogInformation("Sending quotation email to {Email}", dto.RecipientEmail);

                // Validate request
                if (string.IsNullOrEmpty(dto.RecipientEmail))
                    return BadRequest(new { message = "Recipient email is required" });

                if (string.IsNullOrEmpty(dto.PdfBase64))
                    return BadRequest(new { message = "PDF is required" });

                // Send email via SendGrid
                var success = await _emailService.SendQuotationAsync(dto);

                if (success)
                {
                    _logger.LogInformation("Email sent successfully to {Email}", dto.RecipientEmail);

                    // Log to database if needed
                    // await _emailLogService.LogEmailAsync(dto);

                    return Ok(new EmailResponseDto
                    {
                        Success = true,
                        MessageId = Guid.NewGuid().ToString(),
                        Message = $"Email sent to {dto.RecipientEmail}"
                    });
                }
                else
                {
                    _logger.LogWarning("Failed to send email to {Email}", dto.RecipientEmail);
                    return StatusCode(500, new EmailResponseDto
                    {
                        Success = false,
                        Error = "Failed to send email via email service"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending quotation email");
                return StatusCode(500, new EmailResponseDto
                {
                    Success = false,
                    Error = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new
            {
                message = "Email API is working",
                timestamp = DateTime.UtcNow,
                service = "SendGrid Email Service"
            });
        }
    }
}