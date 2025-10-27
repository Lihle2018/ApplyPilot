using JobScraper.Domain.Contracts.Repositories;
using JobScraper.Domain.Entities;
using JobScraper.Infrastructure.Data.Interfaces;
using MongoDB.Driver;

namespace JobScraper.Infrastructure.Repositories
{
    public class JobListingRepository(IJobContext context) : IJobListingRepository
    {
        public async Task<IEnumerable<JobListing>> SaveAsync(IEnumerable<JobListing> entities)
        {
            await context.JobListings.InsertManyAsync(entities);
            return entities;
        }

        public async Task<JobListing> AddAsync(JobListing entity)
        {
            await context.JobListings.InsertOneAsync(entity);
            return entity;
        }

        public async Task<IEnumerable<JobListing>> GetAllAsync()
        {
            return await context.JobListings.Find(_ => true).ToListAsync();
        }

        public async Task<JobListing?> GetByIdAsync(string id)
        {
            return await context.JobListings.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task DeleteAsync(string id)
        {
            await context.JobListings.DeleteOneAsync(x => x.Id == id);
        }
    }
} 
