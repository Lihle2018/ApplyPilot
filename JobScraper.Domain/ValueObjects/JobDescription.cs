namespace JobScraper.Domain.ValueObjects
{
    public class JobDescription
    {
        public string Summary { get; private set; }
        public string Responsibilities { get; private set; }
        public string Requirements { get; private set; }

        public JobDescription(string summary, string responsibilities, string requirements)
        {
            Summary = summary;
            Responsibilities = responsibilities;
            Requirements = requirements;
        }
    }
}