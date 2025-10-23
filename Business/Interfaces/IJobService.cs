using Common.DTOs;

namespace Business.Interfaces
{
    public interface IJobService
    {
        Task<int> AddJobAsync(int userId, JobCreateDto dto);
        Task<List<JobResponseDto>> GetJobsAsync(int userId, bool isAdmin, string? status, string? location, DateTime? from, DateTime? to, int page, int limit);
        Task<bool> UpdateJobAsync(int userId, bool isAdmin, int jobId, JobUpdateDto dto);
        Task<bool> DeleteJobAsync(int userId, bool isAdmin, int jobId);
        Task<byte[]> ExportToCsvAsync(int userId, bool isAdmin);
        Task<byte[]> ExportToPdfAsync(int userId, bool isAdmin);

        Task<JobResponseDto?> GetJobByIdAsync(int userId, bool isAdmin, int jobId);
    }
}
