
namespace JobScraper.Domain.Interfaces
{
    public interface IHttpClientService
    {
        Task<string> GetAsync(string url);
        Task<string> PostAsync(string url, HttpContent content);
    }
}
