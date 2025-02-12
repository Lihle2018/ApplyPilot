using System.Net.Mail;
using System.Net;
using JobScraper.Domain.Contracts.Services;

namespace JobScraper.Infrastructure.EmailService
{
    public class EmailService (string host,int port,string userName,string password) : IEmailService
    {
        public async Task SendEmail(string? from, string? to, string? subject, string? body)
        {
            MailMessage message = new MailMessage(from, to, subject, body) { Priority = MailPriority.High, ReplyTo = new MailAddress(from) };
            var smtp = new SmtpClient
            {
                Host =host,
                Port = port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(userName, password)
            };
            smtp.SendAsync(message, null);
        }
    }
}
