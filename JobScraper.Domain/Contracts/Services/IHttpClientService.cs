﻿using System.Net.Http;

namespace JobScraper.Domain.Contracts.Services
{
    public interface IHttpClientService
    {
        Task<string> GetAsync(string url);
        Task<string> PostAsync(string url, HttpContent content);
        Task<string> PostAsync(HttpRequestMessage httpRequest);
    }
}
