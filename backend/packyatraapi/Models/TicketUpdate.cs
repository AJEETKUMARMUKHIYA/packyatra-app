using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoversAndPackerApi.Models
{
    public class TicketUpdate
    {
        [Key]
        public int UpdateID { get; set; }

        [ForeignKey("Ticket")]
        public int TicketID { get; set; } // Reference to Ticket

        [Required]
        public string? StatusUpdate { get; set; } // Example: "Dispatched", "In Transit", "Delivered"

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Tickets? Ticket { get; set; }
    }
}
