
namespace JobScraper.Domain.Services
{
    public interface IEmailService
    {
        Task SendEmail(string? from, string? to, string? subject, string? body);
    }
}
