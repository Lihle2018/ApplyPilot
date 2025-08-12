using JobScraper.Domain.Enums;

namespace JobScraper.Api.DTOs
{
    public class JobListingDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public JobDescriptionDto Description { get; set; } = new();
        public CompanyDto Company { get; set; } = new();
        public LocationDto Location { get; set; } = new();
        public DateTime PostedDate { get; set; }
        public JobType JobType { get; set; }
        public ExperienceLevel ExperienceLevel { get; set; }
        public SalaryRangeDto SalaryRange { get; set; } = new();
    }

    public class JobDescriptionDto
    {
        public string Summary { get; set; } = string.Empty;
        public string Responsibilities { get; set; } = string.Empty;
        public string Requirements { get; set; } = string.Empty;
    }

    public class CompanyDto
    {
        public string Name { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
    }

    public class LocationDto
    {
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    public class SalaryRangeDto
    {
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public string Currency { get; set; } = string.Empty;
    }
}