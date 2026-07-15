using Microsoft.AspNetCore.Mvc;
using MoversAndPackerApi.Services;
using System.Threading.Tasks;

namespace MoversAndPackerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotController : ControllerBase
    {
        private readonly TimeSlotService _timeSlotService;

        public TimeSlotController(TimeSlotService timeSlotService)
        {
            _timeSlotService = timeSlotService;
        }

        // GET: api/TimeSlot/GetAllTimeSlots
        [HttpGet]
        [Route("GetAllTimeSlots")]
        public async Task<IActionResult> GetAllTimeSlots()
        {
            var slots = await _timeSlotService.GetAllTimeSlotsAsync();
            return Ok(slots);
        }

        // GET: api/TimeSlot/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTimeSlotById(int id)
        {
            var slot = await _timeSlotService.GetTimeSlotByIdAsync(id);
            if (slot == null)
                return NotFound();

            return Ok(slot);
        }
    }
}
