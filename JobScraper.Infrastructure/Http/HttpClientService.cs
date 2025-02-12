using JobScraper.Domain.Contracts.Services;
using System;

namespace JobScraper.Infrastructure.Http
{
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _httpClient;

        public HttpClientService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> GetAsync(string url)
        {
            return await _httpClient.GetStringAsync(url);
        }

        public async Task<string> PostAsync(string url,HttpContent content)
        {
            var response = await _httpClient.PostAsync(url, content);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> PostAsync(HttpRequestMessage httpRequest)
        {
            var response = await _httpClient.SendAsync(httpRequest);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
