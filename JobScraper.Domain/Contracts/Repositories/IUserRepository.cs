using JobScraper.Domain.Entities;

namespace JobScraper.Domain.Contracts.Repositories
{
    public interface IUserRepository
    {
        Task SaveAsync(User entity);
    }
}
