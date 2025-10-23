using Common.DTOs;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DataAccess.Entities;

namespace JobTrackerApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("jobs")]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly ILogger<JobsController> _logger;

        public JobsController(IJobService jobService, ILogger<JobsController> logger)
        {
            _jobService = jobService;
            _logger = logger;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private bool IsAdmin() =>
           User.IsInRole("Admin");

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateJob([FromBody] JobCreateDto dto)
        {
            var jobId = await _jobService.AddJobAsync(GetUserId(), dto);
            _logger.LogInformation("User {UserId} is creating a job for company {Company}", GetUserId(), dto.CompanyName);
            return CreatedAtAction(nameof(GetJobById), new { id = jobId }, new { id = jobId });
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public async Task<IActionResult> GetJobs(
            [FromQuery] string? status,
            [FromQuery] string? location,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10)
        {
            var jobs = await _jobService.GetJobsAsync(GetUserId(), IsAdmin(), status, location, from, to, page, limit);
            _logger.LogInformation("User {UserId} requested job list. IsAdmin: {IsAdmin}, " +
                "Filters: status={Status}, location={Location}, from={From}, to={To}, page={Page}, " +
                "limit={Limit}",GetUserId(), IsAdmin(), status, location, from, to, page, limit);
            return Ok(jobs);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobById(int id)
        {
            var job = await _jobService.GetJobByIdAsync(GetUserId(), IsAdmin(), id);
            if (job == null)
            {
                _logger.LogWarning("User {UserId} tried to access missing job {JobId}", GetUserId(), id);
                return NotFound();
            }   
            _logger.LogInformation("User {UserId} requested job {JobId}. IsAdmin: {IsAdmin}", GetUserId(), id, IsAdmin());
            return Ok(job);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] JobUpdateDto dto)
        {
            var updated = await _jobService.UpdateJobAsync(GetUserId(), IsAdmin(), id, dto);
            if (!updated)
            {
                _logger.LogWarning("Update failed: job {JobId} not found for user {UserId}", id, GetUserId());
                return NotFound();
            }
            _logger.LogInformation("User {UserId} is updating job {JobId}. IsAdmin: {IsAdmin}", GetUserId(), id, IsAdmin());
            return NoContent();

        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var deleted = await _jobService.DeleteJobAsync(GetUserId(), IsAdmin(), id);
            if (!deleted)
            {
                _logger.LogWarning("Delete failed: job {JobId} not found for admin {UserId}", id, GetUserId());
                return NotFound();
            }
            _logger.LogInformation("Admin {UserId} is deleting job {JobId}", GetUserId(), id);
            return NoContent();

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("export/csv")]
        public async Task<IActionResult> ExportCsv()
        {
            var csvBytes = await _jobService.ExportToCsvAsync(GetUserId(), IsAdmin());
            _logger.LogInformation("Admin {UserId} exported jobs to CSV", GetUserId());
            return File(csvBytes, "text/csv", "jobs.csv");

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("export/pdf")]
        public async Task<IActionResult> ExportPdf()
        {
            var pdfBytes = await _jobService.ExportToPdfAsync(GetUserId(), IsAdmin());
            _logger.LogInformation("Admin {UserId} exported jobs to PDF", GetUserId());
            return File(pdfBytes, "application/pdf", "jobs.pdf");
        }
    }
}
