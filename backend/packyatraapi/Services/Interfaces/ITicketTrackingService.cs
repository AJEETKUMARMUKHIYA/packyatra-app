using MoversAndPackerApi.Models.DTOs;

namespace MoversAndPackerApi.Services.Interfaces
{
    public interface ITicketTrackingService
    {
        TicketTrackingDto TrackTicket(string phoneNumber, string ticketNo);
    }

}
