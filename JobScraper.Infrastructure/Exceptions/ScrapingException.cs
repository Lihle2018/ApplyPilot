namespace JobScraper.Infrastructure.Exceptions
{
    namespace JobScraper.Domain.Exceptions
    {
        public class ScrapingException : Exception
        {
            public ScrapingException(string message) : base(message) { }
            public ScrapingException(string message, Exception innerException) : base(message, innerException) { }
        }
    }
}
