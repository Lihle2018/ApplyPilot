using JobScraper.Domain.Interfaces;
using JobScraper.Infrastructure.Http;
using JobScraper.Infrastructure.Proxies;
using JobScraper.Infrastructure.Scrapers;
using Microsoft.Extensions.DependencyInjection;


namespace JobScraper.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string proxyApiUrl,string searchUrl)
        {
            services.AddScoped<IProxyService>(provider =>
            new ProxyService(provider.GetRequiredService<IHttpClientService>(), proxyApiUrl));

            services.AddScoped<IHttpClientService, HttpClientService>();

            services.AddScoped<IJobScraper>(provider =>
                new GoogleJobScraper(provider.GetRequiredService<IProxyService>(), searchUrl));

            return services;
        }
    }

}
