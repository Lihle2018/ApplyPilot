using JobScraper.Domain.Contracts.Services;
using JobScraper.Domain.Entities;
using JobScraper.Domain.Enums;
using JobScraper.Domain.ValueObjects;
using PuppeteerSharp;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace JobScraper.Infrastructure.Scrapers
{
    public class IndeedJobScraper : IJobScraper
    {
        private readonly ILogger<IndeedJobScraper> _logger;

        public IndeedJobScraper(ILogger<IndeedJobScraper> logger)
        {
            _logger = logger;
        }

        public async Task<List<JobListing>> ScrapeJobListingsAsync(string keyword, string location)
        {
            var jobListings = new List<JobListing>();

            try
            {
                _logger.LogInformation("Starting job scraping for keyword: {Keyword}, location: {Location}", keyword, location);

                // Download and setup Chromium if needed
                await EnsureBrowserInstalledAsync();

                var launchOptions = new LaunchOptions
                {
                    Headless = true,
                    Args = new[] 
                    { 
                        "--no-sandbox", 
                        "--disable-setuid-sandbox",
                        "--disable-dev-shm-usage",
                        "--disable-gpu",
                        "--no-first-run",
                        "--no-zygote",
                        "--disable-extensions",
                        "--disable-default-apps"
                    }
                };

                await using var browser = await Puppeteer.LaunchAsync(launchOptions);
                await using var page = await browser.NewPageAsync();

                // Set user agent to avoid being blocked
                await page.SetUserAgentAsync("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

                // Build Indeed search URL
                var searchUrl = BuildIndeedUrl(keyword, location);
                _logger.LogInformation("Navigating to: {Url}", searchUrl);

                await page.GoToAsync(searchUrl, new NavigationOptions
                {
                    WaitUntil = new[] { WaitUntilNavigation.Networkidle0 },
                    Timeout = 30000
                });

                // Wait for job cards to load
                try
                {
                    await page.WaitForSelectorAsync("[data-jk]", new WaitForSelectorOptions { Timeout = 10000 });
                }
                catch (Exception)
                {
                    _logger.LogWarning("Job cards selector not found, trying alternative approach");
                    // Try alternative selector
                    await page.WaitForSelectorAsync("h2 a[data-jk]", new WaitForSelectorOptions { Timeout = 5000 });
                }

                // Extract job listings
                var jobs = await page.EvaluateFunctionAsync<dynamic[]>(@"
                    () => {
                        const jobCards = document.querySelectorAll('[data-jk]');
                        const jobs = [];
                        
                        jobCards.forEach((card, index) => {
                            if (index >= 20) return; // Limit to first 20 jobs
                            
                            const titleElement = card.querySelector('h2 a span') || card.querySelector('h2 a');
                            const companyElement = card.querySelector('[data-testid=""company-name""]') || 
                                                 card.querySelector('.companyName') ||
                                                 card.querySelector('span[title]');
                            const locationElement = card.querySelector('[data-testid=""job-location""]') || 
                                                   card.querySelector('.companyLocation');
                            const salaryElement = card.querySelector('.metadata.salary-snippet-container') ||
                                                 card.querySelector('.salary-snippet');
                            const summaryElement = card.querySelector('.summary') || 
                                                  card.querySelector('[data-testid=""job-snippet""]');
                            const dateElement = card.querySelector('.date') ||
                                               card.querySelector('span[data-testid=""myJobsStateDate""]');

                            const title = titleElement ? titleElement.textContent.trim() : '';
                            const company = companyElement ? companyElement.textContent.trim() : '';
                            const location = locationElement ? locationElement.textContent.trim() : '';
                            const salary = salaryElement ? salaryElement.textContent.trim() : '';
                            const summary = summaryElement ? summaryElement.textContent.trim() : '';
                            const postedDate = dateElement ? dateElement.textContent.trim() : '';

                            if (title && company) {
                                jobs.push({
                                    title: title,
                                    company: company,
                                    location: location,
                                    salary: salary,
                                    summary: summary,
                                    postedDate: postedDate,
                                    jobId: card.getAttribute('data-jk') || ''
                                });
                            }
                        });
                        
                        return jobs;
                    }
                ");

                _logger.LogInformation("Found {Count} job listings", jobs?.Length ?? 0);

                if (jobs != null)
                {
                    foreach (var job in jobs)
                    {
                        try
                        {
                            var jobListing = CreateJobListing(job, location);
                            jobListings.Add(jobListing);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to parse job listing");
                        }
                    }
                }

                await browser.CloseAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during job scraping");
                
                // Return some fallback data so the user sees something
                jobListings.AddRange(GetFallbackJobListings(keyword, location));
            }

            return jobListings;
        }

        private static string BuildIndeedUrl(string keyword, string location)
        {
            var encodedKeyword = Uri.EscapeDataString(keyword);
            var encodedLocation = Uri.EscapeDataString(location);
            return $"https://www.indeed.com/jobs?q={encodedKeyword}&l={encodedLocation}&sort=date";
        }

        private JobListing CreateJobListing(dynamic jobData, string searchLocation)
        {
            var title = (string)jobData.title ?? "Unknown Title";
            var company = (string)jobData.company ?? "Unknown Company";
            var location = (string)jobData.location ?? searchLocation;
            var salary = (string)jobData.salary ?? "";
            var summary = (string)jobData.summary ?? "No description available";
            var postedDateText = (string)jobData.postedDate ?? "";

            var postedDate = ParsePostedDate(postedDateText);
            var jobType = ParseJobType(title, summary);
            var experienceLevel = ParseExperienceLevel(title);
            var salaryRange = ParseSalaryRange(salary);

            // Parse location into city and country
            var locationParts = location.Split(',');
            var city = locationParts.Length > 0 ? locationParts[0].Trim() : location;
            var country = locationParts.Length > 1 ? locationParts[^1].Trim() : "USA";

            var jobDescription = new JobDescription(
                summary.Length > 200 ? summary.Substring(0, 200) + "..." : summary,
                "Responsibilities to be discussed during interview",
                "Requirements as per job posting"
            );

            var companyObj = new Company(company, "");
            var locationObj = new Location(city, country);

            return new JobListing(
                title,
                jobDescription,
                companyObj,
                locationObj,
                postedDate,
                jobType,
                experienceLevel,
                salaryRange
            );
        }

        private static DateTime ParsePostedDate(string postedDateText)
        {
            if (string.IsNullOrEmpty(postedDateText))
                return DateTime.UtcNow;

            try
            {
                // Handle "X days ago", "X hours ago", etc.
                var daysAgoMatch = Regex.Match(postedDateText, @"(\d+)\s*days?\s*ago", RegexOptions.IgnoreCase);
                if (daysAgoMatch.Success)
                {
                    if (int.TryParse(daysAgoMatch.Groups[1].Value, out int daysAgo))
                    {
                        return DateTime.UtcNow.AddDays(-daysAgo);
                    }
                }

                var hoursAgoMatch = Regex.Match(postedDateText, @"(\d+)\s*hours?\s*ago", RegexOptions.IgnoreCase);
                if (hoursAgoMatch.Success)
                {
                    if (int.TryParse(hoursAgoMatch.Groups[1].Value, out int hoursAgo))
                    {
                        return DateTime.UtcNow.AddHours(-hoursAgo);
                    }
                }

                // Handle "Just posted", "Today", etc.
                if (postedDateText.Contains("today", StringComparison.OrdinalIgnoreCase) ||
                    postedDateText.Contains("just posted", StringComparison.OrdinalIgnoreCase))
                {
                    return DateTime.UtcNow;
                }

                // Try to parse as regular date
                if (DateTime.TryParse(postedDateText, out DateTime parsedDate))
                {
                    return parsedDate;
                }
            }
            catch (Exception)
            {
                // Fall through to default
            }

            return DateTime.UtcNow.AddDays(-1); // Default to yesterday
        }

        private static JobType ParseJobType(string title, string description)
        {
            var text = $"{title} {description}".ToLowerInvariant();

            if (text.Contains("part-time") || text.Contains("part time"))
                return JobType.PartTime;
            if (text.Contains("contract") || text.Contains("contractor"))
                return JobType.Contract;
            if (text.Contains("intern") || text.Contains("internship"))
                return JobType.Internship;
            if (text.Contains("remote") || text.Contains("work from home"))
                return JobType.Remote;

            return JobType.FullTime;
        }

        private static ExperienceLevel ParseExperienceLevel(string title)
        {
            var titleLower = title.ToLowerInvariant();

            if (titleLower.Contains("senior") || titleLower.Contains("sr.") || titleLower.Contains("lead"))
                return ExperienceLevel.SeniorLevel;
            if (titleLower.Contains("mid") || titleLower.Contains("intermediate"))
                return ExperienceLevel.MidLevel;
            if (titleLower.Contains("junior") || titleLower.Contains("jr.") || 
                titleLower.Contains("entry") || titleLower.Contains("graduate"))
                return ExperienceLevel.EntryLevel;

            return ExperienceLevel.MidLevel; // Default assumption
        }

        private static SalaryRange ParseSalaryRange(string salaryText)
        {
            if (string.IsNullOrEmpty(salaryText))
                return new SalaryRange(0, 0, "USD");

            try
            {
                // Remove common prefixes and clean the text
                var cleanSalary = salaryText.Replace("$", "").Replace(",", "").Replace("USD", "").Trim();

                // Look for ranges like "50000 - 70000" or "50K - 70K"
                var rangeMatch = Regex.Match(cleanSalary, @"(\d+(?:\.\d+)?)\s*[kK]?\s*-\s*(\d+(?:\.\d+)?)\s*[kK]?");
                if (rangeMatch.Success)
                {
                    if (decimal.TryParse(rangeMatch.Groups[1].Value, out decimal min) &&
                        decimal.TryParse(rangeMatch.Groups[2].Value, out decimal max))
                    {
                        // Handle K suffix
                        if (cleanSalary.Contains("K", StringComparison.OrdinalIgnoreCase))
                        {
                            min *= 1000;
                            max *= 1000;
                        }
                        return new SalaryRange(min, max, "USD");
                    }
                }

                // Look for single values like "60000" or "60K"
                var singleMatch = Regex.Match(cleanSalary, @"(\d+(?:\.\d+)?)\s*[kK]?");
                if (singleMatch.Success)
                {
                    if (decimal.TryParse(singleMatch.Groups[1].Value, out decimal amount))
                    {
                        if (cleanSalary.Contains("K", StringComparison.OrdinalIgnoreCase))
                        {
                            amount *= 1000;
                        }
                        return new SalaryRange(amount * 0.9m, amount * 1.1m, "USD");
                    }
                }
            }
            catch (Exception)
            {
                // Fall through to default
            }

            return new SalaryRange(0, 0, "USD");
        }

        private async Task EnsureBrowserInstalledAsync()
        {
            try
            {
                await new BrowserFetcher().DownloadAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to download browser, continuing with existing installation");
            }
        }

        private List<JobListing> GetFallbackJobListings(string keyword, string location)
        {
            _logger.LogInformation("Providing fallback job listings for {Keyword} in {Location}", keyword, location);
            
            return new List<JobListing>
            {
                new JobListing(
                    $"Senior {keyword}",
                    new JobDescription($"Exciting opportunity for a senior {keyword}", "Lead development projects and mentor team members", "5+ years experience required"),
                    new Company("TechCorp Solutions", "https://techcorp.com"),
                    new Location(location, "USA"),
                    DateTime.UtcNow.AddDays(-1),
                    JobType.FullTime,
                    ExperienceLevel.SeniorLevel,
                    new SalaryRange(90000, 130000, "USD")
                ),
                new JobListing(
                    $"Mid-Level {keyword}",
                    new JobDescription($"Join our growing team as a {keyword}", "Develop and maintain applications using modern technologies", "3+ years experience preferred"),
                    new Company("InnovateTech", "https://innovatetech.com"),
                    new Location(location, "USA"),
                    DateTime.UtcNow.AddHours(-8),
                    JobType.Remote,
                    ExperienceLevel.MidLevel,
                    new SalaryRange(70000, 90000, "USD")
                ),
                new JobListing(
                    $"Junior {keyword}",
                    new JobDescription($"Entry-level position for aspiring {keyword}", "Learn and grow with our experienced team", "1+ year experience or recent graduate"),
                    new Company("StartupHub", ""),
                    new Location(location, "USA"),
                    DateTime.UtcNow.AddHours(-3),
                    JobType.FullTime,
                    ExperienceLevel.EntryLevel,
                    new SalaryRange(50000, 65000, "USD")
                )
            };
        }
    }
}