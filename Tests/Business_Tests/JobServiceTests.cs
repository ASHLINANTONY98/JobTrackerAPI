using AutoMapper;
using Business.Services;
using Common.DTOs;
using Common.Enums;
using DataAccess.Entities;
using DataAccess.Interfaces;
using Moq;
using Xunit;

namespace Tests.Business_Tests
{
    public class JobServiceTests
    {
        private readonly Mock<IJobRepository> _jobRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly JobService _jobService;

        public JobServiceTests()
        {
            _jobRepoMock = new Mock<IJobRepository>();
            _mapperMock = new Mock<IMapper>();
            _jobService = new JobService(_jobRepoMock.Object, _mapperMock.Object);
        }

        private User CreateDummyUser(int userId) => new User
        {
            Id = userId,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashed",
            Role = "User"
        };

        [Fact]
        public async Task GetJobsAsync_ReturnsEmpty_WhenNoMatch()
        {
            var userId = 1;
            var jobs = new List<JobApplication>
            {
                new JobApplication
                {
                    CompanyName = "TestCorp",
                    Status = JobStatus.Rejected,
                    Location = "Onsite",
                    AppliedDate = DateTime.Today,
                    UserId = userId,
                    User = CreateDummyUser(userId)
                }
            };

            _jobRepoMock.Setup(r => r.GetAllByUserAsync(userId)).ReturnsAsync(jobs);
            _mapperMock.Setup(m => m.Map<List<JobResponseDto>>(It.IsAny<List<JobApplication>>()))
                       .Returns(new List<JobResponseDto>());

            var result = await _jobService.GetJobsAsync(userId, "Applied", "Remote", null, null, 1, 10);

            Assert.Empty(result);
        }

        [Fact]
        public async Task AddJobAsync_ReturnsJobId()
        {
            var userId = 1;
            var dto = new JobCreateDto { CompanyName = "TestCorp" };
            var jobEntity = new JobApplication
            {
                Id = 123,
                CompanyName = "TestCorp",
                UserId = userId,
                User = CreateDummyUser(userId)
            };

            _mapperMock.Setup(m => m.Map<JobApplication>(dto)).Returns(jobEntity);
            _jobRepoMock.Setup(r => r.AddAsync(jobEntity)).Returns(Task.CompletedTask);

            var result = await _jobService.AddJobAsync(userId, dto);

            Assert.Equal(123, result);
        }

        [Fact]
        public async Task UpdateJobAsync_ReturnsTrue_WhenJobExists()
        {
            var userId = 1;
            var jobId = 123;
            var dto = new JobUpdateDto { CompanyName = "UpdatedCorp" };
            var jobEntity = new JobApplication
            {
                Id = jobId,
                UserId = userId,
                User = CreateDummyUser(userId)
            };

            _jobRepoMock.Setup(r => r.GetByIdAsync(jobId, userId)).ReturnsAsync(jobEntity);
            _jobRepoMock.Setup(r => r.UpdateAsync(jobEntity)).Returns(Task.CompletedTask);

            var result = await _jobService.UpdateJobAsync(userId, jobId, dto);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateJobAsync_ReturnsFalse_WhenJobNotFound()
        {
            _jobRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync((JobApplication?)null);

            var result = await _jobService.UpdateJobAsync(1, 999, new JobUpdateDto());

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteJobAsync_ReturnsTrue_WhenJobExists()
        {
            var job = new JobApplication
            {
                Id = 123,
                UserId = 1,
                User = CreateDummyUser(1)
            };

            _jobRepoMock.Setup(r => r.GetByIdAsync(123, 1)).ReturnsAsync(job);
            _jobRepoMock.Setup(r => r.DeleteAsync(job)).Returns(Task.CompletedTask);

            var result = await _jobService.DeleteJobAsync(1, 123);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteJobAsync_ReturnsFalse_WhenJobNotFound()
        {
            _jobRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync((JobApplication?)null);

            var result = await _jobService.DeleteJobAsync(1, 999);

            Assert.False(result);
        }

        [Fact]
        public async Task GetJobByIdAsync_ReturnsJob_WhenExists()
        {
            var job = new JobApplication
            {
                Id = 123,
                CompanyName = "TestCorp",
                UserId = 1,
                User = CreateDummyUser(1)
            };
            var dto = new JobResponseDto { Id = 123, CompanyName = "TestCorp" };

            _jobRepoMock.Setup(r => r.GetByIdAsync(123, 1)).ReturnsAsync(job);
            _mapperMock.Setup(m => m.Map<JobResponseDto>(job)).Returns(dto);

            var result = await _jobService.GetJobByIdAsync(1, 123);

            Assert.NotNull(result);
            Assert.Equal("TestCorp", result.CompanyName);
        }

        [Fact]
        public async Task GetJobByIdAsync_ReturnsNull_WhenNotFound()
        {
            _jobRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync((JobApplication?)null);

            var result = await _jobService.GetJobByIdAsync(1, 999);

            Assert.Null(result);
        }

        [Fact]
        public async Task ExportToCsvAsync_ReturnsByteArray()
        {
            var jobs = new List<JobApplication>
            {
                new JobApplication
                {
                    CompanyName = "TestCorp",
                    UserId = 1,
                    User = CreateDummyUser(1)
                }
            };

            _jobRepoMock.Setup(r => r.GetAllByUserAsync(1)).ReturnsAsync(jobs);

            var result = await _jobService.ExportToCsvAsync(1);

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task ExportToPdfAsync_ReturnsByteArray()
        {
            var jobs = new List<JobApplication>
            {
                new JobApplication
                {
                    CompanyName = "TestCorp",
                    UserId = 1,
                    User = CreateDummyUser(1)
                }
            };

            _jobRepoMock.Setup(r => r.GetAllByUserAsync(1)).ReturnsAsync(jobs);

            var result = await _jobService.ExportToPdfAsync(1);

            Assert.NotEmpty(result);
        }
    }
}
