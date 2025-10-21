using Common.Enums;

namespace Common.DTOs
{
    public class JobCreateDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public string PositionTitle { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public JobStatus Status { get; set; }
        public DateTime AppliedDate { get; set; }
        public string JobLink { get; set; } = string.Empty;
        public int SalaryExpectation { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string ResumePath { get; set; } = string.Empty;
    }
}
