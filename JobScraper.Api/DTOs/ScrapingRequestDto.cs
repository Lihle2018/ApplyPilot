using System.ComponentModel.DataAnnotations;

namespace JobScraper.Api.DTOs
{
    public class ScrapingRequestDto
    {
        [Required]
        [MinLength(2)]
        public string Keyword { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        public string Location { get; set; } = string.Empty;
    }

    public class ScrapingResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<JobListingDto> JobListings { get; set; } = new();
        public int TotalCount { get; set; }
        public DateTime ScrapedAt { get; set; }
    }
}