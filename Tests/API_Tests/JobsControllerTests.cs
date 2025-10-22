using System.Security.Claims;
using Business.Interfaces;
using Common.DTOs;
using JobTrackerApp.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Common.Helpers;

namespace Tests.API_Tests
{
    public class JobsControllerTests
    {
        private readonly Mock<IJobService> _jobServiceMock;
        private readonly JobsController _controller;

        public JobsControllerTests()
        {
            _jobServiceMock = new Mock<IJobService>();
            _controller = new JobsController(_jobServiceMock.Object);

            // Simulate authenticated user with userId = 1
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "1")
                    }))
                }
            };
        }

        [Fact]
        public async Task GetJobs_ReturnsOkWithJobs()
        {
            // Arrange
            var jobs = new List<JobResponseDto>
            {
                new JobResponseDto { CompanyName = "TestCorp" }
            };

            _jobServiceMock
                .Setup(s => s.GetJobsAsync(1, null, null, null, null, 1, 10))
                .ReturnsAsync(jobs);

            // Act
            var result = await _controller.GetJobs(null, null, null, null, 1, 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedJobs = Assert.IsType<List<JobResponseDto>>(okResult.Value);
            Assert.Single(returnedJobs);
            Assert.Equal("TestCorp", returnedJobs[0].CompanyName);
        }

        [Fact]
        public async Task CreateJob_ReturnsCreatedAtWithJobId()
        {
            // Arrange
            var dto = new JobCreateDto
            {
                CompanyName = "TestCorp",
                PositionTitle = "Dev",
                Location = "Remote",
                AppliedDate = DateTime.Today
            };

            _jobServiceMock
                .Setup(s => s.AddJobAsync(1, dto))
                .ReturnsAsync(123);

            // Act
            var result = await _controller.CreateJob(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetJobById", createdResult.ActionName);

            var idProperty = createdResult.Value?.GetType().GetProperty("id");
            var idValue = idProperty?.GetValue(createdResult.Value, null);
            Assert.Equal(123, idValue);
        }

        [Fact]
        public async Task GetJobById_ReturnsOk_WhenJobExists()
        {
            var dto = new JobResponseDto { Id = 123, CompanyName = "TestCorp" };
            _jobServiceMock.Setup(s => s.GetJobByIdAsync(1, 123)).ReturnsAsync(dto);

            var result = await _controller.GetJobById(123);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedJob = Assert.IsType<JobResponseDto>(okResult.Value);
            Assert.Equal(123, returnedJob.Id);
        }

        [Fact]
        public async Task GetJobById_ReturnsNotFound_WhenJobMissing()
        {
            _jobServiceMock.Setup(s => s.GetJobByIdAsync(1, 999)).ReturnsAsync((JobResponseDto?)null);

            var result = await _controller.GetJobById(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateJob_ReturnsNoContent_WhenSuccessful()
        {
            var dto = new JobUpdateDto { CompanyName = "UpdatedCorp" };
            _jobServiceMock.Setup(s => s.UpdateJobAsync(1, 123, dto)).ReturnsAsync(true);

            var result = await _controller.UpdateJob(123, dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateJob_ReturnsNotFound_WhenJobMissing()
        {
            var dto = new JobUpdateDto { CompanyName = "UpdatedCorp" };
            _jobServiceMock.Setup(s => s.UpdateJobAsync(1, 999, dto)).ReturnsAsync(false);

            var result = await _controller.UpdateJob(999, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteJob_ReturnsNoContent_WhenSuccessful()
        {
            _jobServiceMock.Setup(s => s.DeleteJobAsync(1, 123)).ReturnsAsync(true);

            var result = await _controller.DeleteJob(123);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteJob_ReturnsNotFound_WhenJobMissing()
        {
            _jobServiceMock.Setup(s => s.DeleteJobAsync(1, 999)).ReturnsAsync(false);

            var result = await _controller.DeleteJob(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ExportCsv_ReturnsFileResult()
        {
            _jobServiceMock.Setup(s => s.ExportToCsvAsync(1)).ReturnsAsync(new byte[] { 1, 2, 3 });

            var result = await _controller.ExportCsv();

            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("text/csv", fileResult.ContentType);
            Assert.Equal("jobs.csv", fileResult.FileDownloadName);
        }

        [Fact]
        public async Task ExportPdf_ReturnsFileResult()
        {
            // Load native PDF library
            var context = new CustomAssemblyLoadContext();
            context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "libwkhtmltox.dll"));

            _jobServiceMock.Setup(s => s.ExportToPdfAsync(1)).ReturnsAsync(new byte[] { 1, 2, 3 });

            var result = await _controller.ExportPdf();

            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("application/pdf", fileResult.ContentType);
            Assert.Equal("jobs.pdf", fileResult.FileDownloadName);
        }


    }
}
