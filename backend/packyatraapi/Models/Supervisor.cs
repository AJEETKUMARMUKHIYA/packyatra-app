using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoversAndPackerApi.Models
{
    public class Supervisor
    {
        [Key]
        public int SupervisorID { get; set; } // Unique ID for the supervisor

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // Supervisor's Name

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } // Supervisor's Email

        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } // Contact number

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } // Example: "Manager", "Field Staff", etc.

        public bool IsActive { get; set; } = true; // Whether supervisor is active

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property - A Supervisor can have multiple tickets assigned
        public ICollection<Tickets> Tickets { get; set; }
    }
}
