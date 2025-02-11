using JobScraper.Domain.Services;
using JobScraper.Infrastructure.AIProcessing;
using JobScraper.Infrastructure.Data.Interfaces;
using JobScraper.Infrastructure.Data;
using JobScraper.Infrastructure.Http;
using JobScraper.Infrastructure.Proxies;
using JobScraper.Infrastructure.Scrapers;
using Microsoft.Extensions.DependencyInjection;


namespace JobScraper.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
                                                                 this IServiceCollection services,
                                                                 string proxyApiUrl,
                                                                 string searchUrl,
                                                                 string openAIApiUrl,
                                                                 string openAIToken,
                                                                 string mongoConnectionString, 
                                                                 string mongoDatabaseName, 
                                                                 string mongoCollectionName) 
        {
            services.AddScoped<IProxyService>(provider =>
                new ProxyService(provider.GetRequiredService<IHttpClientService>(), proxyApiUrl));

            services.AddScoped<IHttpClientService, HttpClientService>();

            services.AddScoped<IJobScraper>(provider =>
                new GoogleJobScraper(provider.GetRequiredService<IProxyService>(), searchUrl));

            services.AddScoped<IAIProcessingService>(provider =>
                new OpenAIClient(provider.GetRequiredService<IHttpClientService>(), openAIApiUrl, openAIToken));

            services.AddScoped<IJobContext>(provider =>
                new JobContext(mongoConnectionString, mongoDatabaseName, mongoCollectionName));

            return services;
        }

    }


}
