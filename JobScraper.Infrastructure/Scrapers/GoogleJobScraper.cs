using JobScraper.Domain.Entities;
using JobScraper.Domain.Enums;
using JobScraper.Domain.Services;
using JobScraper.Domain.ValueObjects;
using PuppeteerSharp;
using System.Globalization;

namespace JobScraper.Infrastructure.Scrapers
{
    internal class GoogleJobScraper(IProxyService proxyService, string searchUrl) : IJobScraper
    {
        public async Task<List<JobListing>> ScrapeJobListingsAsync(string keyword, string location)
        {
            var jobListings = new List<JobListing>();

            // Get a random proxy
            var proxy = await proxyService.GetRandomProxy();
            var proxyUrl = $"http://{proxy.Ip}:{proxy.Port}";

            // Configure Puppeteer to use the proxy
            var launchOptions = new LaunchOptions
            {
                Headless = true,
                Args = new[] { $"--proxy-server={proxyUrl}" }
            };

            // Launch the browser with the proxy
            await using var browser = await Puppeteer.LaunchAsync(launchOptions);
            await using var page = await browser.NewPageAsync();

            // Navigate to the search URL
            var url = string.Format(searchUrl, Uri.EscapeDataString(keyword), Uri.EscapeDataString(location));
            await page.GoToAsync(url, WaitUntilNavigation.Networkidle2);

            // Wait for job listings to load
            await page.WaitForSelectorAsync(".BjJfJf");

            // Extract job postings
            var jobElements = await page.QuerySelectorAllAsync(".BjJfJf"); // job cards container
            foreach (var jobElement in jobElements)
            {
                var title = await ExtractTextAsync(jobElement, ".BjJfJf");
                var company = await ExtractTextAsync(jobElement, ".vNEEBe");
                var locationText = await ExtractTextAsync(jobElement, ".Qk80Jf");
                var postedDateText = await ExtractTextAsync(jobElement, ".date");
                var jobTypeText = await ExtractTextAsync(jobElement, ".job-type");
                var salaryText = await ExtractTextAsync(jobElement, ".salary");

            
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(company))
                {
                    DateTime postedDate = ParsePostedDate(postedDateText);
                    var jobType = ParseJobType(jobTypeText);
                    var experienceLevel = ParseExperienceLevel(title);
                    var salaryRange = ParseSalaryRange(salaryText);

                    var jobDescription = new JobDescription("No description available", "", "");
                    var companyObj = new Company(company, "");
                    var locationObj = new Location(locationText ?? "Remote", "");

                    var jobListing = new JobListing(
                        title,
                        jobDescription,
                        companyObj,
                        locationObj,
                        postedDate,
                        jobType,
                        experienceLevel,
                        salaryRange
                    );

                    jobListings.Add(jobListing);
                }
            }

            await browser.CloseAsync();
            return jobListings;
        }

        private async Task<string> ExtractTextAsync(IElementHandle parentElement, string selector)
        {
            var element = await parentElement.QuerySelectorAsync(selector);
            return element is not null ? await element.EvaluateFunctionAsync<string>("el => el.innerText") : string.Empty;
        }


        private static DateTime ParsePostedDate(string postedDateText)
        {
            if (string.IsNullOrEmpty(postedDateText))
                return DateTime.UtcNow;

            // Handle relative dates (e.g., "2 days ago")
            if (postedDateText.Contains("day", StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(postedDateText.Split(' ')[0], out int daysAgo))
                {
                    return DateTime.UtcNow.AddDays(-daysAgo);
                }
            }

            // Handle absolute dates (e.g., "2023-10-01")
            if (DateTime.TryParse(postedDateText, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }

            return DateTime.UtcNow; // Default to current time
        }

        private static JobType ParseJobType(string jobTypeText)
        {
            if (string.IsNullOrEmpty(jobTypeText)) return JobType.FullTime;
            if (jobTypeText.Contains("Part", StringComparison.OrdinalIgnoreCase)) return JobType.PartTime;
            if (jobTypeText.Contains("Contract", StringComparison.OrdinalIgnoreCase)) return JobType.Contract;
            if (jobTypeText.Contains("Intern", StringComparison.OrdinalIgnoreCase)) return JobType.Internship;
            if (jobTypeText.Contains("Remote", StringComparison.OrdinalIgnoreCase)) return JobType.Remote;
            return JobType.FullTime;
        }

        private static ExperienceLevel ParseExperienceLevel(string title)
        {
            if (string.IsNullOrEmpty(title)) return ExperienceLevel.EntryLevel;
            if (title.Contains("Senior", StringComparison.OrdinalIgnoreCase)) return ExperienceLevel.SeniorLevel;
            if (title.Contains("Mid", StringComparison.OrdinalIgnoreCase) || title.Contains("Intermediate", StringComparison.OrdinalIgnoreCase))
                return ExperienceLevel.MidLevel;
            return ExperienceLevel.EntryLevel;
        }

        private static SalaryRange ParseSalaryRange(string salaryText)
        {
            if (string.IsNullOrEmpty(salaryText)) return new SalaryRange(0, 0, "Rand");

            // Example: "$50,000 - $70,000 per year"
            var parts = salaryText.Split(new[] { '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2 && decimal.TryParse(parts[0].Replace("R", "").Replace(",", ""), out decimal min) &&
                decimal.TryParse(parts[1].Replace("$", "").Replace(",", ""), out decimal max))
            {
                return new SalaryRange(min, max, "Rand");
            }
            return new SalaryRange(0, 0, "Rand");
        }
    }
}
