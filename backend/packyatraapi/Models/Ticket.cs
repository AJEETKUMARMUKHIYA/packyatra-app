using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoversAndPackerApi.Models
{
    public class Tickets
    {
        [Key]
        public int TicketID { get; set; }
        public int UserID { get; set; }
        public required string TicketNo { get;set;}
       
        public int BookingID { get; set; } // Reference to the booking
        
        public required string FromLocation { get; set; }
        public required string ToLocation { get; set; }
        public int? AssignedSupervisorID { get; set; } // Assigned staff (nullable)

       
        public string Status { get; set; } = "Open"; // Open, In-Transit, Completed

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
      public int ? VehicleId { get; set; }

        // Navigation properties

    }
}
