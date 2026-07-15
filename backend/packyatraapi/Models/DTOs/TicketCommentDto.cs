namespace MoversAndPackerApi.Models.DTOs
{
    public class TicketCommentDto
    {
        public int CommentId { get; set; }
        public string CommentType { get; set; }
        public string CommentText { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
