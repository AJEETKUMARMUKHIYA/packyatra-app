namespace MoversAndPackerApi.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string VehicleNumber { get; set; }
        public required string VehicleType { get; set; }
        public required string DriverName { get; set; }
        public string DriverPhone { get; set; } = "";
        public int Capacity { get; set; }
        public string Status { get; set; } = "Available";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
