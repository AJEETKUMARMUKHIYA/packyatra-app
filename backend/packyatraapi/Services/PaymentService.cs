using Microsoft.EntityFrameworkCore;
using MoversAndPackerApi.Data;
using MoversAndPackerApi.Models;
using System.Threading.Tasks;

namespace MoversAndPackerApi.Services
{
    public class PaymentService
    {
        private readonly ApplicationDbContext _context;

        public PaymentService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Process Payment
        public async Task<bool> ProcessPaymentAsync(int bookingId, PaymentDetails payment)
        {
            var booking = await _context.Booking.FindAsync(bookingId);
            if (booking == null) return false; // Booking not found

            // Update payment details
            var newPayment = new Payments
            {
                UserID=payment.UserID,
                BookingID = bookingId,
                Amount = payment.Amount,
                PaymentStatus = payment.PaymentStatus,
                TransactionID = payment.TransactionID,
                PaymentDate = DateTime.UtcNow
            };

            _context.Payments.Add(newPayment);
            //booking.Status = "Paid"; // Update booking status
            await _context.SaveChangesAsync();

            return true;
        }
    }
}