using JobScraper.Domain.Entities;

namespace JobScraper.Domain.Interfaces
{
    public interface IJobScraper
    {
        Task<List<JobListing>> ScrapeJobListingsAsync(string keyword, string location);
    }
}
