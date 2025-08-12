using Microsoft.AspNetCore.Mvc;
using JobScraper.Api.DTOs;
using JobScraper.Domain.Contracts.Services;
using JobScraper.Domain.Contracts.Repositories;

namespace JobScraper.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class JobListingsController : ControllerBase
    {
        private readonly IJobScraper _jobScraper;
        private readonly IJobListingRepository _jobListingRepository;
        private readonly ILogger<JobListingsController> _logger;

        public JobListingsController(
            IJobScraper jobScraper, 
            IJobListingRepository jobListingRepository,
            ILogger<JobListingsController> logger)
        {
            _jobScraper = jobScraper;
            _jobListingRepository = jobListingRepository;
            _logger = logger;
        }

        /// <summary>
        /// Scrape job listings from external sources
        /// </summary>
        /// <param name="request">Scraping parameters (keyword and location)</param>
        /// <returns>List of scraped job listings</returns>
        [HttpPost("scrape")]
        [ProducesResponseType(typeof(ScrapingResponseDto), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<ActionResult<ScrapingResponseDto>> ScrapeJobListings([FromBody] ScrapingRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Starting job scraping for keyword: {Keyword}, location: {Location}", 
                    request.Keyword, request.Location);

                var jobListings = await _jobScraper.ScrapeJobListingsAsync(request.Keyword, request.Location);

                // Save scraped jobs to database
                foreach (var job in jobListings)
                {
                    try
                    {
                        await _jobListingRepository.AddAsync(job);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to save job listing {JobId} to database", job.Id);
                    }
                }

                var response = new ScrapingResponseDto
                {
                    Success = true,
                    Message = $"Successfully scraped {jobListings.Count} job listings",
                    JobListings = jobListings.Select(MapToDto).ToList(),
                    TotalCount = jobListings.Count,
                    ScrapedAt = DateTime.UtcNow
                };

                _logger.LogInformation("Successfully scraped {Count} job listings", jobListings.Count);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while scraping job listings");
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Scraping Error",
                    Detail = "An error occurred while scraping job listings. Please try again later.",
                    Status = 500
                });
            }
        }

        /// <summary>
        /// Get all job listings from the database
        /// </summary>
        /// <returns>List of job listings</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<JobListingDto>), 200)]
        public async Task<ActionResult<List<JobListingDto>>> GetJobListings()
        {
            try
            {
                var jobListings = await _jobListingRepository.GetAllAsync();
                var dtos = jobListings.Select(MapToDto).ToList();
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving job listings");
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Database Error",
                    Detail = "An error occurred while retrieving job listings.",
                    Status = 500
                });
            }
        }

        /// <summary>
        /// Get a specific job listing by ID
        /// </summary>
        /// <param name="id">Job listing ID</param>
        /// <returns>Job listing details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JobListingDto), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        public async Task<ActionResult<JobListingDto>> GetJobListing(string id)
        {
            try
            {
                var jobListing = await _jobListingRepository.GetByIdAsync(id);
                if (jobListing == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Job Not Found",
                        Detail = $"No job listing found with ID: {id}",
                        Status = 404
                    });
                }

                return Ok(MapToDto(jobListing));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving job listing {JobId}", id);
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Database Error",
                    Detail = "An error occurred while retrieving the job listing.",
                    Status = 500
                });
            }
        }

        /// <summary>
        /// Delete a job listing by ID
        /// </summary>
        /// <param name="id">Job listing ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        public async Task<IActionResult> DeleteJobListing(string id)
        {
            try
            {
                var jobListing = await _jobListingRepository.GetByIdAsync(id);
                if (jobListing == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Job Not Found",
                        Detail = $"No job listing found with ID: {id}",
                        Status = 404
                    });
                }

                await _jobListingRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting job listing {JobId}", id);
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Database Error",
                    Detail = "An error occurred while deleting the job listing.",
                    Status = 500
                });
            }
        }

        private static JobListingDto MapToDto(Domain.Entities.JobListing jobListing)
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