namespace MoversAndPackerApi.Models
{
    public class UpdateBookingRequest
    {
        public int BookingID { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? BookingStatus { get; set; }
       
    }
}
