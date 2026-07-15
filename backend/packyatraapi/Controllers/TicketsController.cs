using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoversAndPackerApi.Services;

namespace MoversAndPackerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // This makes the base path: api/Tickets
    public class TicketsController : ControllerBase
    {
        private readonly BookingService _bookingService;

        // Inject your existing BookingService via Constructor
        public TicketsController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // GET: api/Tickets/SupervisorJobs?supervisorId=101
        [HttpGet("SupervisorJobs")]
        public async Task<IActionResult> GetSupervisorJobs([FromQuery] int supervisorId)
        {
            try
            {
                // Call the service layer query method we created earlier
                var jobs = await _bookingService.GetBookingsBySupervisorIdAsync(supervisorId);

                // Return 200 OK with the true data array JSON payload
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while fetching supervisor jobs.",
                    error = ex.Message
                });
            }
        }

        // GET: api/Tickets/BookingInventory?bookingId=12
        [HttpGet("BookingInventory")]
        public async Task<IActionResult> GetBookingInventory([FromQuery] int bookingId)
        {
            try
            {
                var items = await _bookingService.GetBookingItemsByBookingIdAsync(bookingId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to parse dynamic manifest items.", error = ex.Message });
            }
        }
    }
}
