using JobScraper.Domain.Entities;

namespace JobScraper.Domain.Contracts.Repositories
{
    public interface IJobListingRepository
    {
        Task<IEnumerable<JobListing>> SaveAsync(IEnumerable<JobListing> entities);
        Task<JobListing> AddAsync(JobListing entity);
        Task<IEnumerable<JobListing>> GetAllAsync();
        Task<JobListing?> GetByIdAsync(string id);
        Task DeleteAsync(string id);
    }
}
