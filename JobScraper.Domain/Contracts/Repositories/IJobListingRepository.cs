using JobScraper.Domain.Entities;

namespace JobScraper.Domain.Contracts.Repositories
{
    public interface IJobListingRepository
    {
        Task<IEnumerable<JobListing>> SaveAsync(IEnumerable<JobListing> entities);
    }
}
