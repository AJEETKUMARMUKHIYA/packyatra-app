namespace MoversAndPackerApi.Models
{
    public class Users
    {
        public int UserID { get; set; } // Use the correct property name
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int AddressID { get; set; }
    }
}
