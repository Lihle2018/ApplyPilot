namespace JobScraper.Application.Contracts
{
    public interface IEmailService
    {
        Task SendEmail(string? from, string? to, string? subject, string? body);
    }
}
