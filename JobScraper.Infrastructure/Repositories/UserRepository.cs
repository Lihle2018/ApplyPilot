using JobScraper.Domain.Contracts.Repositories;
using JobScraper.Domain.Entities;
using JobScraper.Infrastructure.Data.Interfaces;

namespace JobScraper.Infrastructure.Repositories
{
    public class UserRepository(IJobContext context) : IUserRepository
    {
        public async Task SaveAsync(User entity)
        {
            await context.Users.InsertOneAsync(entity);
        }
    }
}
