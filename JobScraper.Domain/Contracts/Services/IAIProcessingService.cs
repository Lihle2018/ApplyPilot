namespace JobScraper.Domain.Contracts.Services
{
    public interface IAIProcessingService
    {
        Task<string> UpdateCvAsync(string jobDescription, string currentCv);
        Task<(string Subject, string Body)> GenerateEmailAsync(string jobDescription);
    }

}
