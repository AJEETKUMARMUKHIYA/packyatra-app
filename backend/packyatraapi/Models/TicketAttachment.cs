using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoversAndPackerApi.Models
{

    [Table("TicketAttachment", Schema = "dbo")]
    public class TicketAttachment
    {
        public int AttachmentId { get; set; }
        public int TicketId { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedOn { get; set; } = DateTime.Now;
    }
}
