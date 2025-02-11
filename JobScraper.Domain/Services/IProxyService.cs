namespace JobScraper.Domain.Services
{
    public interface IProxyService
    {
        Task<(string Ip, int Port)> GetRandomProxy();
        Task<List<(string Ip, int Port)>> GetAllProxies();
        Task<bool> ValidateProxyAsync(string ip, int port);
    }
}
