namespace MoversAndPackerApi.Models.DTOs
{
    public class TicketTrackingDto
    {
        public TicketSummaryDto Ticket { get; set; }
        public List<TicketCommentDto> Comments { get; set; }
    }

}
