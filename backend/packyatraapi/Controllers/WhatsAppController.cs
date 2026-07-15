using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoversAndPackerApi.Models;
using MoversAndPackerApi.Services;

namespace MoversAndPackerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhatsAppController : ControllerBase
    {
        private readonly WhatsAppService _service;

        public WhatsAppController(WhatsAppService service)
        {
            _service = service;
        }

        [HttpPost("send-pdf")]
        public async Task<IActionResult> SendPdf(
            [FromBody] SendWhatsAppPdfRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.MobileNumber))
                return BadRequest("Mobile number is required");

            if (string.IsNullOrWhiteSpace(request.PdfUrl))
                return BadRequest("PDF URL is required");

            await _service.SendWhatsAppWithPdfAsync(
                request.MobileNumber,
                request.PdfUrl
            );

            return Ok(new
            {
                success = true,
                message = "WhatsApp message with PDF sent successfully"
            });
        }
    }
}

