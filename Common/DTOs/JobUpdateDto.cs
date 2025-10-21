using Common.Enums;

namespace Common.DTOs
{
    public class JobUpdateDto
    {
        public string? CompanyName { get; set; }
        public string? PositionTitle { get; set; }
        public string? Location { get; set; }
        public JobStatus? Status { get; set; }
        public DateTime? AppliedDate { get; set; }
        public string? JobLink { get; set; }
        public int? SalaryExpectation { get; set; }
        public string? Notes { get; set; }
        public string? ResumePath { get; set; }
    }

}
