using Microsoft.EntityFrameworkCore;
using MoversAndPackerApi.Data;
using MoversAndPackerApi.Models;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace MoversAndPackerApi.Services
{
    public class TicketService
    {
        private readonly ApplicationDbContext _context;

        public TicketService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Generate Ticket after Payment
        public async Task<Tickets> CreateTicketAsync(int bookingId)
        {
            var booking = await _context.Booking.FindAsync(bookingId);
            if (booking == null) return null; // Booking not found
            var addressdetails= await _context.Address.FindAsync(booking.SourceAddressID);
         
            var newTicket = new Tickets
            {
                BookingID = bookingId,
                FromLocation= addressdetails.FromAddress,
                ToLocation= addressdetails.ToAddress,
                UserID=booking.UserID,
              
                TicketNo= Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(),
                AssignedSupervisorID = null, // Assign manually later
                Status = "Open",
                CreatedAt = DateTime.UtcNow
            };

            _context.Tickets.Add(newTicket);
            await _context.SaveChangesAsync();
            return newTicket;
        }

        // ✅ Get Ticket Status
        public async Task<Tickets> GetTicketStatusAsync(int bookingId)
        {
            return await _context.Tickets
                .Where(t => t.BookingID == bookingId)
                .FirstOrDefaultAsync();
        }

        // ✅ Update Ticket (For Supervisors)
        public async Task<bool> UpdateTicketStatusAsync(int ticketId, string status)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null) return false;

            ticket.Status = status;
            _context.Tickets.Update(ticket);
            return await _context.SaveChangesAsync() > 0;
        }

        // ✅ Get All Tracking Updates for a Ticket
        public async Task<List<TicketUpdate>> GetTicketUpdatesAsync(int ticketId)
        {
            return await _context.TicketUpdate
                .Where(t => t.TicketID == ticketId)
                .OrderByDescending(t => t.UpdatedAt)
                .ToListAsync();
        }

        public async Task<List<Tickets>> GetAllTicketsAsync()
        {
            return await _context.Tickets
                .Select(t => new Tickets
                {
                    TicketID = t.TicketID,
                    BookingID = t.BookingID,
                    UserID = t.UserID,
                    FromLocation = t.FromLocation,
                    ToLocation = t.ToLocation,
                    //BookingAmount = t.BookingAmount,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    AssignedSupervisorID = t.AssignedSupervisorID,
                    TicketNo = t.TicketNo,
                    VehicleId =t.VehicleId
                })
                .ToListAsync();
        }

        public async Task<List<UserAdmin>> GetSupervisorsAsync()
        {
            // Assuming supervisors have a Role of "Supervisor"
            return await _context.UserAdmin
                .Where(u => u.RoleId == 2)
                .Select(u => new UserAdmin
                {
                    UserId = u.UserId,

                    RoleId = u.RoleId,
                    Password = u.Password,
                    Email = u.Email,
                    Mobile = u.Mobile,
                    CreatedBy = u.CreatedBy,
                    CreatedDate = u.CreatedDate,
                    //UpdatedBy = u.UpdatedBy,
                    UpdatedDate = u.UpdatedDate,
                    Active = u.Active,
                    LastActivityDate = u.LastActivityDate,
                    FirstName = u.FirstName,
                    LastName = u.LastName,

                    DefaultAccountId = u.DefaultAccountId
                })
                .ToListAsync();
        }

        public async Task<bool> AssignTicketAsync(int ticketId, int supervisorId)
        {
            try
            {
                var ticket = await _context.Tickets
                    .FirstOrDefaultAsync(t => t.TicketID == ticketId);
                if (ticket == null)
                    return false;

                var supervisor = await _context.UserAdmin
                    .FirstOrDefaultAsync(u => u.UserId == supervisorId && u.RoleId == 2);
              //  if (supervisor == null)
               //     return false;

                ticket.AssignedSupervisorID = supervisorId ==0 ?null :supervisorId;
                ticket.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error assigning ticket: {ex.Message}");
                return false;
            }
        }
    }
}