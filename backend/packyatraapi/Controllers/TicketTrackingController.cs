using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoversAndPackerApi.Services.Interfaces;

namespace MoversAndPackerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketTrackingController : ControllerBase
    {
        private readonly ITicketTrackingService _service;

        public TicketTrackingController(ITicketTrackingService service)
        {
            _service = service;
        }

        // GET: api/TicketTracking/track?phoneNumber=xxx&ticketNo=xxx
        [HttpGet("track")]
        public IActionResult Track([FromQuery] string phoneNumber, [FromQuery] string ticketNo)
        {
            if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(ticketNo))
                return BadRequest("Phone number and Ticket number are required");

            var result = _service.TrackTicket(phoneNumber, ticketNo);

            if (result?.Ticket == null)
                return NotFound("Ticket not found");

            return Ok(result);
        }
    }
}
