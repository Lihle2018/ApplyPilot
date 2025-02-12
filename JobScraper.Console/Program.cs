
using JobScraper.Application.Features.Users.Commands.RegisterUser;
using JobScraper.ConsoleApp.Common;
using JobScraper.ConsoleApp.Helpers;
using JobScraper.Application.Extensions;
using JobScraper.Infrastructure.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
namespace JobScraper.ConsoleApp
{
    internal class Program
    {
        static async void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            // Configure DI
            var services = new ServiceCollection();
            ConfigureServices(services);
            var provider = services.BuildServiceProvider();


            // Resolve MediatR
            var mediator = provider.GetRequiredService<IMediator>();


            DisplayBanner.Show();

            // Get user details
            Console.Write("\nEnter your first name: ");
            string firstName = Console.ReadLine();

            Console.Write("Enter your surname: ");
            string surname = Console.ReadLine();

            Console.Write("Enter your email: ");
            string email = Validators.ValidateEmail(Console.ReadLine());

            Console.Write("Enter your phone number: ");
            string phoneNumber = Validators.ValidatePhoneNumber(Console.ReadLine());

            Console.Write("Enter your LinkedIn URL: ");
            string linkedinLink = Validators.ValidateUrl(Console.ReadLine(), "LinkedIn");

            Console.Write("Enter your GitHub URL: ");
            string githubLink = Validators.ValidateUrl(Console.ReadLine(), "GitHub");

            Console.Write("Enter your profile URL (LinkedIn or other): ");
            string profileLink = Validators.ValidateUrl(Console.ReadLine(), "Profile");

            Console.Write("Enter a short summary about yourself: ");
            string summary = Validators.ValidateSummary(Console.ReadLine());

            Console.Write("Enter your projects (comma-separated): ");
            List<string> projects = new List<string>(Console.ReadLine()?.Split(',') ?? new string[0]);

            // Create User object
            UserRequest command = new UserRequest
            {
                Name = firstName,
                Surname = surname,
                Email = email,
                PhoneNumber = phoneNumber,
                LinkedinLink = linkedinLink,
                GitHubLink = githubLink,
                Projects = projects ?? new List<string>(),
                Summary = summary
            };

            await mediator.Send(command);

            Console.WriteLine("\n✅ User profile successfully created!\n");
          

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Add application and infrastructure services
            services.AddApplicationServices();
            services.AddInfrastructureServices(
                proxyApiUrl: "https://your-proxy-url.com",
                searchUrl: "https://your-search-url.com",
                openAIApiUrl: "https://api.openai.com/completion",
                openAIToken: "your-api-token",
                mongoConnectionString: "your-mongo-connection-string",
                mongoDatabaseName: "your-database-name",
                jobsCollectionName: "Jobs",
                usersCollectionName: "Users"
            );
        }
    }
}
