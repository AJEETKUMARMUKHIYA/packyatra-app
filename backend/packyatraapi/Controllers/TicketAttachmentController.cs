using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoversAndPackerApi.Data;
using MoversAndPackerApi.Services;

namespace MoversAndPackerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketAttachmentController : ControllerBase
    {
        private readonly TicketAttachmentService _service;
        private readonly ApplicationDbContext _context;

        public TicketAttachmentController(ApplicationDbContext context, TicketAttachmentService service)
        {
            _context = context;
            _service = service;
        }

        [HttpPost("Upload/{ticketId}")]
        public async Task<IActionResult> Upload(int ticketId, IFormFile file)
        {
            if (file == null)
                return BadRequest("File required");

            var result = await _service.UploadAsync(ticketId, file);
            return Ok(result);
        }

        [HttpGet("ByTicket/{ticketId}")]
        public IActionResult GetByTicket(int ticketId)
        {
            var files = _service.GetByTicket(ticketId);
            return Ok(files);
        }

        [HttpGet("Download/{attachmentId}")]
        public IActionResult Download(int attachmentId)
        {
            var file = _service.GetById(attachmentId);
            if (file == null) return NotFound();

            var bytes = System.IO.File.ReadAllBytes(file.FilePath);
            return File(bytes, "application/octet-stream", file.OriginalFileName);
        }
    }
}
