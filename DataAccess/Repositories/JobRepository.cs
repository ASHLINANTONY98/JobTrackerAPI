using DataAccess.DbContext.DataAccess.DbContext;
using DataAccess.Entities;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly AppDbContext _context;

        public JobRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<JobApplication>> GetAllByUserAsync(int userId)
        {
            return await _context.JobApplications
                .Where(j => j.UserId == userId)
                .ToListAsync();
        }

        public async Task<JobApplication?> GetByIdAsync(int id, int userId)
        {
            return await _context.JobApplications
                .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId);
        }

        public async Task AddAsync(JobApplication job)
        {
            _context.JobApplications.Add(job);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(JobApplication job)
        {
            _context.JobApplications.Update(job);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(JobApplication job)
        {
            _context.JobApplications.Remove(job);
            await _context.SaveChangesAsync();
        }

        public async Task<List<JobApplication>> GetAllAsync()
        {
            return await _context.JobApplications.ToListAsync();
        }

        public async Task<JobApplication?> GetByIdAsync(int id)
        {
            return await _context.JobApplications.FirstOrDefaultAsync(j => j.Id == id);
        }

    }

}
