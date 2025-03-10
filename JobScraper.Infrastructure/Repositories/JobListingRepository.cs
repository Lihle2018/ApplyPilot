﻿using JobScraper.Domain.Contracts.Repositories;
using JobScraper.Domain.Entities;
using JobScraper.Infrastructure.Data.Interfaces;

namespace JobScraper.Infrastructure.Repositories
{
    public class JobListingRepository(IJobContext context) : IJobListingRepository
    {
        public async Task<IEnumerable<JobListing>> SaveAsync(IEnumerable<JobListing> entities)
        {
            await context.JobListings.InsertManyAsync(entities);
            return entities;
        }
    }
} 
