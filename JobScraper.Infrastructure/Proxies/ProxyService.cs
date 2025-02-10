using JobScraper.Domain.Interfaces;
using JobScraper.Infrastructure.Exceptions.JobScraper.Domain.Exceptions;
using System.Net;

namespace JobScraper.Infrastructure.Proxies
{
    public class ProxyService(IHttpClientService httpClient, string ApiUrl) : IProxyService
    {
        public async Task<List<(string Ip, int Port)>> GetAllProxies()
        {
            try
            {
                var proxies = new List<(string Ip, int Port)>();
                var response = await httpClient.GetAsync(ApiUrl);

                var lines = response.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    var parts = line.Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int port))
                    {
                        proxies.Add((parts[0], port));
                    }
                }

                return proxies;
            }
            catch (Exception e)
            {

                throw new ScrapingException(e.Message, e.InnerException);
            }
        }

        public async Task<(string Ip, int Port)> GetRandomProxy()
        {
            var proxies = await GetAllProxies();
            if (proxies.Count == 0)
            {
                throw new ScrapingException("No proxies available.");
            }

            var random = new Random();
            int maxRetries = proxies.Count;

            for (int i = 0; i < maxRetries; i++)
            {
                int index = random.Next(proxies.Count);
                var proxy = proxies[index];

                if (await ValidateProxyAsync(proxy.Ip, proxy.Port))
                {
                    return proxy;
                }
            }

            throw new ScrapingException("No valid proxies available.");
        }
        public async Task<bool> ValidateProxyAsync(string ip, int port)
        {
            try
            {
                var handler = new HttpClientHandler
                {
                    Proxy = new WebProxy($"{ip}:{port}"),
                    UseProxy = true
                };

                using var client = new HttpClient(handler)
                {
                    Timeout = TimeSpan.FromSeconds(5)
                };

                var response = await client.GetAsync("https://www.google.com");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
