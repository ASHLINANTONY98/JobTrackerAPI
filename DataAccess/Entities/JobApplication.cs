using Common.Enums;

namespace DataAccess.Entities
{
    public class JobApplication
    {
        public int Id { get; set; }                          // Primary key

        public int UserId { get; set; }                      // Foreign key to User
        public required User User { get; set; }                       // Navigation property

        public string CompanyName { get; set; } = string.Empty;
        public string PositionTitle { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        public JobStatus Status { get; set; }                // Enum: Applied, Interview, Offer, Rejected

        public DateTime AppliedDate { get; set; }            // Date of application
        public string JobLink { get; set; } = string.Empty;  // URL to job posting
        public int SalaryExpectation { get; set; }           // Expected salary
        public string Notes { get; set; } = string.Empty;    // Additional notes
        public string ResumePath { get; set; } = string.Empty; // Path or URL to resume

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

}
