using JobScraper.Infrastructure.AIProcessing;
using JobScraper.Infrastructure.Data.Interfaces;
using JobScraper.Infrastructure.Data;
using JobScraper.Infrastructure.Http;
using JobScraper.Infrastructure.Proxies;
using JobScraper.Infrastructure.Scrapers;
using Microsoft.Extensions.DependencyInjection;
using JobScraper.Domain.Contracts.Services;
using JobScraper.Infrastructure.Repositories;
using JobScraper.Domain.Contracts.Repositories;


namespace JobScraper.Infrastructure.Extensions
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
                                                                string jobsCollectionName,
                                                                string usersCollectionName)
        {
            services.AddScoped<IProxyService>(provider =>
                new ProxyService(provider.GetRequiredService<IHttpClientService>(), proxyApiUrl));

            services.AddScoped<IHttpClientService, HttpClientService>();

            services.AddScoped<IJobScraper>(provider =>
                new GoogleJobScraper(provider.GetRequiredService<IProxyService>(), searchUrl));

            services.AddScoped<IAIProcessingService>(provider =>
                new OpenAIClient(provider.GetRequiredService<IHttpClientService>(), openAIApiUrl, openAIToken));

            services.AddScoped<IJobContext>(provider =>
                new JobContext(mongoConnectionString, mongoDatabaseName, jobsCollectionName, usersCollectionName));

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IJobListingRepository, JobListingRepository>();

            return services;
        }

    }


}
