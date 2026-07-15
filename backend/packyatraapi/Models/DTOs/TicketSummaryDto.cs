namespace MoversAndPackerApi.Models.DTOs
{
    public class TicketSummaryDto
    {
        public int TicketID { get; set; }
        public string TicketNo { get; set; }
        public string TicketStatus { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public DateTime TicketCreatedAt { get; set; }
        public DateTime TicketUpdatedAt { get; set; }

        public DateTime PickupDate { get; set; }
        public string BookingStatus { get; set; }

        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }
}