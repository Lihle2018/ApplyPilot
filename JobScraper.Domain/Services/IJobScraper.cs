using JobScraper.Domain.Entities;

namespace JobScraper.Domain.Services
{
    public interface IJobScraper
    {
        Task<List<JobListing>> ScrapeJobListingsAsync(string keyword, string location);
    }
}
