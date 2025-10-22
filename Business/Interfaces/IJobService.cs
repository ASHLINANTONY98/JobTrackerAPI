using Common.DTOs;

namespace Business.Interfaces
{
    public interface IJobService
    {
        Task<int> AddJobAsync(int userId, JobCreateDto dto);
        Task<List<JobResponseDto>> GetJobsAsync(int userId, string? status, string? location, DateTime? from, DateTime? to, int page, int limit);
        Task<bool> UpdateJobAsync(int userId, int jobId, JobUpdateDto dto);
        Task<bool> DeleteJobAsync(int userId, int jobId);
        Task<byte[]> ExportToCsvAsync(int userId);
        Task<byte[]> ExportToPdfAsync(int userId);

        Task<JobResponseDto?> GetJobByIdAsync(int userId, int jobId);
    }
}
