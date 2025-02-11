using JobScraper.Domain.Entities;
using MongoDB.Driver;

namespace JobScraper.Infrastructure.Data.Interfaces
{
    public interface IJobContext
    {
      IMongoCollection<JobListing> JobListings { get; }
    }
}
