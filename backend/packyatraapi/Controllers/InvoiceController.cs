using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoversAndPackerApi.Services.Interfaces;

namespace MoversAndPackerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateInvoice([FromQuery] string quotationNumber)
        {
            if (string.IsNullOrWhiteSpace(quotationNumber))
                return BadRequest("Quotation number is required");

            var result = await _invoiceService.GenerateInvoiceAsync(quotationNumber);

            if (result.StatusCode == 0)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetInvoiceDetails([FromQuery] string invoiceNumber)
        {
            if (string.IsNullOrWhiteSpace(invoiceNumber))
                return BadRequest("Invoice number is required");

            var result = await _invoiceService.GetInvoiceDetailsByNumberAsync(invoiceNumber);

            return Ok(result);
        }

    }
}
