using Microsoft.AspNetCore.Mvc;
using MoversAndPackerApi.Models;
using MoversAndPackerApi.Services;

namespace MoversAndPackerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _bookingService;
      
        private readonly PaymentService _paymentService;
        private readonly TicketService _ticketService;

        public BookingController(BookingService bookingService,PaymentService paymentService,TicketService ticketService)
        {
            _bookingService = bookingService;
         
            _paymentService = paymentService;
            _ticketService = ticketService;
        }

       
        //[HttpPost]
        //[Route("CreateBooking")]
        //public async Task<IActionResult> CreateBooking([FromBody] BookingDetails booking)
        //{
        //    var createdBooking = await _bookingService.CreateBookingAsync(booking);
        //    return Ok(createdBooking);
        //}
        [HttpPost]
        [Route("CreateBooking")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDetails booking)
        {
            var createdBooking = await _bookingService.CreateBookingAsync(booking);
               return Ok(createdBooking);
        }
        //[HttpGet]
        //[Route("GetByUser/{userId}")]
        //public async Task<IActionResult> GetBookingsByUser(int userId)
        //{
        //    var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
        //    return Ok(bookings);
        //}

        [HttpPut]
        [Route("Update/{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] Booking booking)
        {
            if (id != booking.BookingID) return BadRequest("Booking ID mismatch");

            var updated = await _bookingService.UpdateBookingAsync(booking);
            return updated ? Ok("Updated successfully") : NotFound("Booking not found");
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var deleted = await _bookingService.DeleteBookingAsync(id);
            return deleted ? Ok("Deleted successfully") : NotFound("Booking not found");
        }
        // ✅ 1️⃣ Confirm Payment & Generate Ticket
        [HttpPost]
        [Route("ConfirmPayment/{bookingId}")]
        public async Task<IActionResult> ConfirmPayment(int bookingId, [FromBody] PaymentDetails payment)
        {
            var success = await _paymentService.ProcessPaymentAsync(bookingId, payment);
            if (!success) return BadRequest("Payment failed or booking not found.");

            // ✅ Auto-Generate Ticket after successful payment
            var ticket = await _ticketService.CreateTicketAsync(bookingId);
            return Ok(new { Message = "Payment successful & Ticket Generated", Ticket = ticket });
        }

        // ✅ 2️⃣ Track Booking Status
        [HttpGet]
        [Route("TrackBooking/{bookingId}")]
        public async Task<IActionResult> TrackBooking(int bookingId)
        {
            var trackingDetails = await _ticketService.GetTicketUpdatesAsync(bookingId);
            return trackingDetails != null ? Ok(trackingDetails) : NotFound("No tracking details found.");
        }
        [HttpPost]
        [Route("ProcessPayment")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentDetails payment)
        {
            var result = await _paymentService.ProcessPaymentAsync(payment.BookingID, payment);
            var ticket = await _ticketService.CreateTicketAsync(payment.BookingID);
            return Ok(new { Message = "Payment successful & Ticket Generated", Ticket = ticket });
          
        }
        // ✅ API to Fetch All Bookings for a User
        [HttpGet]
        [Route("GetBookings/{userId}")]
        public async Task<IActionResult> GetBookings(int userId)
        {
            var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
            //if (bookings == null)
            //{
            //    return NotFound("No bookings found for this user.");
            //}
            return Ok(bookings);
          
        }
        [HttpGet]
        [Route("Tickets")]

        public async Task<IActionResult> GetTickets()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }

        // New endpoint to fetch all supervisors
  
        [HttpGet("Supervisors")]
        public async Task<IActionResult> GetSupervisors()
        {
            var supervisors = await _ticketService.GetSupervisorsAsync();
            return Ok(supervisors);
        }

        // New endpoint to assign a ticket to a supervisor
        [HttpPut]
        [Route("AssignTicket/{ticketId}")]
        public async Task<IActionResult> AssignTicket(int ticketId, [FromBody] AssignTicketRequest request)
        {
            if (ticketId != request.TicketID) return BadRequest("Ticket ID mismatch");
            var success = await _ticketService.AssignTicketAsync(ticketId, request.SupervisorID);
            return success ? Ok("Ticket assigned successfully") : NotFound("Ticket or supervisor not found");
        }
        [HttpPost("update-booking")]
        public async Task<IActionResult> UpdateBooking([FromBody] UpdateBookingRequest request)
        {
            if (request == null || request.BookingID <= 0)
                return BadRequest("Invalid request");

            var result = await _bookingService.UpdateBookingAndTicketAsync(request);

            return Ok(result);
        }
        [HttpGet("SupervisorJobs")]
        public async Task<IActionResult> GetSupervisorJobs([FromQuery] int supervisorId)
        {
            try
            {
                var jobs = await _bookingService.GetBookingsBySupervisorIdAsync(supervisorId);
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Database mapping breakdown occurred", error = ex.Message });
            }
        }
    }
}

