using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoversAndPackerApi.Data;
using MoversAndPackerApi.Models;
using System;

namespace MoversAndPackerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketCommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TicketCommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TicketComments/123
        [HttpGet("{ticketId}")]
        public IActionResult GetByTicket(int ticketId)
        {
            var comments = _context.TicketComments
                .Where(x => x.TicketId == ticketId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return Ok(comments);
        }

        // POST: api/TicketComments
        [HttpPost]
        public IActionResult Add([FromBody] TicketComment comment)
        {
            if (comment == null)
                return BadRequest();

            _context.TicketComments.Add(comment);
            _context.SaveChanges();

            return Ok(comment);
        }
    }
}