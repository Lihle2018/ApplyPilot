using JobScraper.Domain.Entities;

namespace JobScraper.Domain.Repositories
{
    public interface IJobListingRepository
    {
        Task<IEnumerable<JobListing>> SaveAsync(IEnumerable<JobListing> entities);
    }
}
