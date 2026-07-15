namespace MoversAndPackerApi.Models
{
    public class TicketComment
    {
        public int CommentId { get; set; }
        public int TicketId { get; set; }

        public string CommentType { get; set; }   // General, Delivery Update, Consignment
        public string CommentText { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
