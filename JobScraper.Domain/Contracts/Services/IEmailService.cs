namespace JobScraper.Domain.Contracts.Services
{
    public interface IEmailService
    {
        Task SendEmail(string? from, string? to, string? subject, string? body);
    }
}
