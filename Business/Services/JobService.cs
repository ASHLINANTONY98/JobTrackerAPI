using AutoMapper;
using Common.DTOs;
using Business.Export;
using Business.Interfaces;
using DataAccess.Entities;
using DataAccess.Interfaces;

namespace Business.Services
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepo;
        private readonly IMapper _mapper;

        public JobService(IJobRepository jobRepo, IMapper mapper)
        {
            _jobRepo = jobRepo;
            _mapper = mapper;
        }

        public async Task<int> AddJobAsync(int userId, JobCreateDto dto)
        {
            var job = _mapper.Map<JobApplication>(dto);
            job.UserId = userId;
            await _jobRepo.AddAsync(job);
            return job.Id;
        }

        public async Task<List<JobResponseDto>> GetJobsAsync(int userId, bool isAdmin, string? status, string? location, DateTime? from, DateTime? to, int page, int limit)
        {
            var jobs = isAdmin
                ? await _jobRepo.GetAllAsync()
                : await _jobRepo.GetAllByUserAsync(userId);

            // Filtering
            if (!string.IsNullOrEmpty(status))
                jobs = jobs.Where(j => j.Status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrEmpty(location))
                jobs = jobs.Where(j => j.Location.Contains(location)).ToList();
            if (from.HasValue)
                jobs = jobs.Where(j => j.AppliedDate >= from.Value).ToList();
            if (to.HasValue)
                jobs = jobs.Where(j => j.AppliedDate <= to.Value).ToList();

            // Pagination
            jobs = jobs.Skip((page - 1) * limit).Take(limit).ToList();

            return _mapper.Map<List<JobResponseDto>>(jobs);
        }

        public async Task<bool> UpdateJobAsync(int userId, bool isAdmin, int jobId, JobUpdateDto dto)
        {
            var job = isAdmin
                ? await _jobRepo.GetByIdAsync(jobId)
                : await _jobRepo.GetByIdAsync(jobId, userId);

            if (job == null) return false;

            _mapper.Map(dto, job);
            job.UpdatedAt = DateTime.UtcNow;
            await _jobRepo.UpdateAsync(job);
            return true;
        }

        public async Task<bool> DeleteJobAsync(int userId, bool isAdmin, int jobId)
        {
            var job = isAdmin
                ? await _jobRepo.GetByIdAsync(jobId) // no user filter
                : await _jobRepo.GetByIdAsync(jobId, userId);

            if (job == null) return false;

            await _jobRepo.DeleteAsync(job);
            return true;
        }

        public async Task<byte[]> ExportToCsvAsync(int userId, bool isAdmin)
        {
            var jobs = isAdmin
                ? await _jobRepo.GetAllAsync()
                : await _jobRepo.GetAllByUserAsync(userId);

            return CsvExporter.GenerateCsv(jobs);
        }

        public async Task<byte[]> ExportToPdfAsync(int userId, bool isAdmin)
        {
            var jobs = isAdmin
                ? await _jobRepo.GetAllAsync()
                : await _jobRepo.GetAllByUserAsync(userId);

            return PdfExporter.GeneratePdf(jobs);
        }


        public async Task<JobResponseDto?> GetJobByIdAsync(int userId, bool isAdmin, int jobId)
        {
            var job = isAdmin
                ? await _jobRepo.GetByIdAsync(jobId)
                : await _jobRepo.GetByIdAsync(jobId, userId);

            return job == null ? null : _mapper.Map<JobResponseDto>(job);
        }


    }

}
