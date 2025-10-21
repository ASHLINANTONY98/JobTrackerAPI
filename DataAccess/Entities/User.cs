namespace DataAccess.Entities
{
    public class User
    {
        public int Id { get; set; }                          // Primary key
        public string Username { get; set; } = string.Empty; // Unique username
        public string Email { get; set; } = string.Empty;    // User email
        public string PasswordHash { get; set; } = string.Empty; // Hashed password
        public string Role { get; set; } = "User";           // Optional: "User" or "Admin"

        // Navigation property
        public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
    }

}
