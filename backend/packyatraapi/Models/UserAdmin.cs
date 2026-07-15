namespace MoversAndPackerApi.Models
{
    public class UserAdmin
    {
        
        public int UserId { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public string Mobile { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public int RoleId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool Active { get; set; }
        public DateTime LastActivityDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? FullName { get; set; }
        public int DefaultAccountId { get; set; }
   
    }

}
