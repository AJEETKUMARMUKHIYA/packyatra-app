using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoversAndPackerApi.Models
{
    public class Payments
    {
        [Key]
        public int PaymentID { get; set; }
   
        [Required]
        public int UserID { get; set; } // Booking ID for payment
        [ForeignKey("Booking")]
        public int BookingID { get; set; } // Booking ID for payment

        [Required]
        public decimal Amount { get; set; } // Payment amount
      
        public string PaymentStatus { get; set; } // Example: Card, UPI, PayPal

        public  string TransactionID { get; set; } // Payment transaction ID

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;


    }
}
