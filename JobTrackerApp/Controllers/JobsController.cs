using Common.DTOs;
using Common.Interfaces;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobTrackerApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("jobs")]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobsController(IJobService jobService)
        {
            _jobService = jobService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<IActionResult> CreateJob([FromBody] JobCreateDto dto)
        {
            var jobId = await _jobService.AddJobAsync(GetUserId(), dto);
            return CreatedAtAction(nameof(GetJobById), new { id = jobId }, new { id = jobId });
        }

        [HttpGet]
        public async Task<IActionResult> GetJobs(
            [FromQuery] string? status,
            [FromQuery] string? location,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10)
        {
            var jobs = await _jobService.GetJobsAsync(GetUserId(), status, location, from, to, page, limit);
            return Ok(jobs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobById(int id)
        {
            var job = await _jobService.GetJobByIdAsync(GetUserId(), id);
            if (job == null) return NotFound();
            return Ok(job);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] JobUpdateDto dto)
        {
            var updated = await _jobService.UpdateJobAsync(GetUserId(), id, dto);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var deleted = await _jobService.DeleteJobAsync(GetUserId(), id);
            return deleted ? NoContent() : NotFound();
        }

        [HttpGet("export/csv")]
        public async Task<IActionResult> ExportCsv()
        {
            var csvBytes = await _jobService.ExportToCsvAsync(GetUserId());
            return File(csvBytes, "text/csv", "jobs.csv");
        }

        [HttpGet("export/pdf")]
        public async Task<IActionResult> ExportPdf()
        {
            var pdfBytes = await _jobService.ExportToPdfAsync(GetUserId());
            return File(pdfBytes, "application/pdf", "jobs.pdf");
        }
    }
}
