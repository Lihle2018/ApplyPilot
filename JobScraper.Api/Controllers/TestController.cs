using Microsoft.AspNetCore.Mvc;
using JobScraper.Api.DTOs;
using JobScraper.Domain.Entities;
using JobScraper.Domain.Enums;
using JobScraper.Domain.ValueObjects;

namespace JobScraper.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Test endpoint to verify the API is working
        /// </summary>
        [HttpGet("health")]
        public ActionResult<object> TestHealth()
        {
            return Ok(new { 
                Status = "Healthy", 
                Timestamp = DateTime.UtcNow,
                Message = "JobPilot API is running"
            });
        }

        /// <summary>
        /// Mock job scraping for testing without external dependencies
        /// </summary>
        [HttpPost("mock-scrape")]
        public ActionResult<ScrapingResponseDto> MockScrape([FromBody] ScrapingRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Mock scraping for keyword: {Keyword}, location: {Location}", 
                request.Keyword, request.Location);

            // Create mock job listings
            var mockJobs = new List<JobListing>
            {
                new JobListing(
                    $"Senior {request.Keyword}",
                    new JobDescription("Excellent opportunity for a senior developer", "Lead development projects", "5+ years experience"),
                    new Company("TechCorp", "https://techcorp.com"),
                    new Location(request.Location, "USA"),
                    DateTime.UtcNow.AddDays(-2),
                    JobType.FullTime,
                    ExperienceLevel.SeniorLevel,
                    new SalaryRange(80000, 120000, "USD")
                ),
                new JobListing(
                    $"Mid-level {request.Keyword}",
                    new JobDescription("Great opportunity for career growth", "Develop and maintain applications", "3+ years experience"),
                    new Company("DevStudio", "https://devstudio.com"),
                    new Location(request.Location, "USA"),
                    DateTime.UtcNow.AddDays(-1),
                    JobType.Remote,
                    ExperienceLevel.MidLevel,
                    new SalaryRange(60000, 80000, "USD")
                ),
                new JobListing(
                    $"Junior {request.Keyword}",
                    new JobDescription("Entry-level position with training provided", "Support development team", "1+ year experience"),
                    new Company("StartupXYZ", ""),
                    new Location(request.Location, "USA"),
                    DateTime.UtcNow,
                    JobType.FullTime,
                    ExperienceLevel.EntryLevel,
                    new SalaryRange(45000, 55000, "USD")
                )
            };

            var response = new ScrapingResponseDto
            {
                Success = true,
                Message = $"Successfully found {mockJobs.Count} mock job listings",
                JobListings = mockJobs.Select(MapToDto).ToList(),
                TotalCount = mockJobs.Count,
                ScrapedAt = DateTime.UtcNow
            };

            return Ok(response);
        }

        private static JobListingDto MapToDto(JobListing jobListing)
        {
            return new JobListingDto
            {
                Id = jobListing.Id,
                Title = jobListing.Title,
                Description = new JobDescriptionDto
                {
                    Summary = jobListing.Description.Summary,
                    Responsibilities = jobListing.Description.Responsibilities,
                    Requirements = jobListing.Description.Requirements
                },
                Company = new CompanyDto
                {
                    Name = jobListing.Company.Name,
                    Website = jobListing.Company.Website
                },
                Location = new LocationDto
                {
                    City = jobListing.Location.City,
                    Country = jobListing.Location.Country
                },
                PostedDate = jobListing.PostedDate,
                JobType = jobListing.JobType,
                ExperienceLevel = jobListing.ExperienceLevel,
                SalaryRange = new SalaryRangeDto
                {
                    MinSalary = jobListing.SalaryRange.Min,
                    MaxSalary = jobListing.SalaryRange.Max,
                    Currency = jobListing.SalaryRange.Currency
                }
            };
        }
    }
}