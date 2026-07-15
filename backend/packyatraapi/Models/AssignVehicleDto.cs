namespace MoversAndPackerApi.Models
{
    public class AssignVehicleDto
    {
        public int BookingId { get; set; }
        public int VehicleId { get; set; }
        public string? Remarks { get; set; }
        public DateTime? AssignedDate { get; set; }
    }
}
