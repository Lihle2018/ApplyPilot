using JobScraper.Domain.Enums;
using JobScraper.Domain.ValueObjects;

namespace JobScraper.Domain.Entities
{
    public class JobListing
    {
        public string Id { get; private set; }
        public string Title { get; private set; }
        public JobDescription Description { get; private set; }
        public Company Company { get; private set; }
        public Location Location { get; private set; }
        public DateTime PostedDate { get; private set; }
        public JobType JobType { get; private set; }
        public ExperienceLevel ExperienceLevel { get; private set; }
        public SalaryRange SalaryRange { get; private set; }

        public JobListing(string title, JobDescription description, Company company, Location location, DateTime postedDate, JobType jobType, ExperienceLevel experienceLevel, SalaryRange salaryRange)
        {
            Id = Guid.NewGuid().ToString();
            Title = title;
            Description = description;
            Company = company;
            Location = location;
            PostedDate = postedDate;
            JobType = jobType;
            ExperienceLevel = experienceLevel;
            SalaryRange = salaryRange;
        }
    }
}

