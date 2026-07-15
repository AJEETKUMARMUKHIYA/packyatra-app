namespace MoversAndPackerApi.Models
{
    public class VehicleCreateDto
    {
        public string VehicleNumber { get; set; }
        public string VehicleType { get; set; }
        public string DriverName { get; set; }
        public string DriverPhone { get; set; }
        public int Capacity { get; set; }
    }
    public class VehicleUpdateDto
    {
        public string VehicleType { get; set; }
        public string DriverName { get; set; }
        public string DriverPhone { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; }
    }
}
