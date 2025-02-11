using JobScraper.Domain.Entities;
using JobScraper.Infrastructure.Data.Interfaces;
using MongoDB.Driver;

namespace JobScraper.Infrastructure.Data
{
    internal class JobContext : IJobContext
    {
        public JobContext(string connectionString,string databaseName,string collectionName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            JobListings = database.GetCollection<JobListing>(collectionName);
        }
        public IMongoCollection<JobListing> JobListings { get; }
    }
}
