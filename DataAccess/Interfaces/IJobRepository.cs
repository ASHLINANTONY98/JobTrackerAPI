using DataAccess.Entities;

namespace DataAccess.Interfaces
{
    public interface IJobRepository
    {
        Task<List<JobApplication>> GetAllByUserAsync(int userId);
        Task<JobApplication?> GetByIdAsync(int id, int userId);
        Task AddAsync(JobApplication job);
        Task UpdateAsync(JobApplication job);
        Task DeleteAsync(JobApplication job);
    }
}
