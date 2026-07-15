using System;
using System.ComponentModel.DataAnnotations;

namespace MoversAndPackerApi.Models
{
    public class PaymentDetails
    {
        [Required]
        public int UserID { get; set; } // Booking ID for payment
        [Required]
        public int BookingID { get; set; } // Booking ID for payment

        [Required]
        public decimal Amount { get; set; } // Payment amount

        [Required]
        [MaxLength(50)]
        public string PaymentStatus { get; set; } // Example: Card, UPI, PayPal

        public string TransactionID { get; set; } // Payment transaction ID

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    }
}
	